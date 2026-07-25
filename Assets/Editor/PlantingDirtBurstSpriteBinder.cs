using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Rebind Texture Sheet Animation sprites on PlantingDirtBurst (Inspector + Full Rect).
/// Menu: Rayman/VFX/Bind PlantingDirtBurst Sprites
/// </summary>
public static class PlantingDirtBurstSpriteBinder
{
    private const string PrefabPath = "Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab";
    private const string DirtSheetPath = "Assets/Art/Sprites/VFX/Planting/PlantationDirtParticules.png";
    private const string WormSheetPath = "Assets/Art/Sprites/VFX/Planting/wurmParticleFarmPlantation.png";
    private const string DirtMatPath = "Assets/Art/Sprites/VFX/Planting/M_PlantingDirtParticles.mat";
    private const string WormMatPath = "Assets/Art/Sprites/VFX/Planting/M_PlantingWormParticles.mat";

    [MenuItem("Rayman/VFX/Bind PlantingDirtBurst Sprites")]
    public static void Bind()
    {
        EnsureFullRect(DirtSheetPath);
        EnsureFullRect(WormSheetPath);
        AssetDatabase.ImportAsset(DirtSheetPath);
        AssetDatabase.ImportAsset(WormSheetPath);

        var dirtSprites = LoadSpritesSorted(DirtSheetPath);
        var wormSprites = LoadSpritesSorted(WormSheetPath);
        var dirtMat = AssetDatabase.LoadAssetAtPath<Material>(DirtMatPath);
        var wormMat = AssetDatabase.LoadAssetAtPath<Material>(WormMatPath);

        if (dirtSprites.Length == 0 || wormSprites.Length == 0)
        {
            Debug.LogError("[PlantingDirtBurst] Sprites introuvables — vérifier import Multiple + slices.");
            return;
        }

        if (dirtMat == null || wormMat == null)
        {
            Debug.LogError("[PlantingDirtBurst] Materials manquants.");
            return;
        }

        var root = PrefabUtility.LoadPrefabContents(PrefabPath);
        try
        {
            var dirt = root.transform.Find("DirtBurst");
            var worm = root.transform.Find("WormBurst");
            if (dirt == null || worm == null)
            {
                Debug.LogError("[PlantingDirtBurst] Enfants DirtBurst / WormBurst introuvables.");
                return;
            }

            BindPs(dirt.GetComponent<ParticleSystem>(), dirt.GetComponent<ParticleSystemRenderer>(),
                dirtSprites, dirtMat, randomStartFrame: true);
            BindPs(worm.GetComponent<ParticleSystem>(), worm.GetComponent<ParticleSystemRenderer>(),
                wormSprites, wormMat, randomStartFrame: false);

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            AssetDatabase.SaveAssets();
            Debug.Log(
                $"[PlantingDirtBurst] OK — Dirt sprites={dirtSprites.Length}, Worm sprites={wormSprites.Length}. " +
                "Ouvre le prefab → Texture Sheet Animation doit lister les sprites.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void BindPs(
        ParticleSystem ps,
        ParticleSystemRenderer renderer,
        Sprite[] sprites,
        Material mat,
        bool randomStartFrame)
    {
        if (ps == null || renderer == null)
        {
            return;
        }

        renderer.sharedMaterial = mat;

        var tsa = ps.textureSheetAnimation;
        tsa.enabled = true;
        tsa.mode = ParticleSystemAnimationMode.Sprites;

        // Clear then set (Unity keeps stale None slots otherwise).
        while (tsa.spriteCount > 0)
        {
            tsa.RemoveSprite(0);
        }

        for (var i = 0; i < sprites.Length; i++)
        {
            tsa.AddSprite(sprites[i]);
        }

        if (randomStartFrame)
        {
            tsa.startFrame = new ParticleSystem.MinMaxCurve(0f, 0.9999f);
        }
        else
        {
            tsa.startFrame = new ParticleSystem.MinMaxCurve(0f);
        }
    }

    private static Sprite[] LoadSpritesSorted(string texturePath)
    {
        var trailingNumber = new Regex(@"_(\d+)$");
        return AssetDatabase.LoadAllAssetsAtPath(texturePath)
            .OfType<Sprite>()
            .OrderBy(s =>
            {
                var match = trailingNumber.Match(s.name);
                return match.Success && int.TryParse(match.Groups[1].Value, out var n) ? n : int.MaxValue;
            })
            .ToArray();
    }

    private static void EnsureFullRect(string texturePath)
    {
        var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        if (importer.spriteImportMode != SpriteImportMode.Multiple)
        {
            Debug.LogWarning($"[PlantingDirtBurst] {texturePath} n'est pas en Sprite Mode Multiple.");
        }

        // Unity 6: spriteMeshType est sur TextureImporterSettings, plus sur TextureImporter.
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        if (settings.spriteMeshType == SpriteMeshType.FullRect)
        {
            return;
        }

        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
        importer.SaveAndReimport();
    }
}
