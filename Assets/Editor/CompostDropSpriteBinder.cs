using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Bind CompostDrop sprites on ShopItemPopup drop trash + sprite flipbook in DropToTrash clip.
/// Menu: Rayman/UI/Bind Compost Drop Sprites
/// </summary>
public static class CompostDropSpriteBinder
{
    private const string PrefabPath = "Assets/Prefabs/Ui/ShopItemPopup.prefab";
    private const string SheetPath = "Assets/Art/Sprites/UI/Inventory/DropCompost/CompostDrop.png";
    private const string DropClipPath = "Assets/Animations/UI/ShopItemPopup_DropToTrash.anim";
    private const string TrashBinChildName = "TrashBin";
    private const string DropTrashRootName = "DropTrashRoot";
    private const float DropDurationSeconds = 0.85f;
    private const float TrashBinWidth = 180f;
    private const float TrashBinHeight = 140f;

    [MenuItem("Rayman/UI/Bind Compost Drop Sprites")]
    public static void Bind()
    {
        EnsureFullRect(SheetPath);
        AssetDatabase.ImportAsset(SheetPath, ImportAssetOptions.ForceUpdate);

        Sprite[] frames = LoadDropFramesSorted(SheetPath);
        Sprite idle = LoadSpriteByName(SheetPath, "CompostDrop_Idle");
        if (frames.Length == 0 || idle == null)
        {
            Debug.LogError(
                "[CompostDrop] Sprites introuvables — vérifier Multiple + slices sur CompostDrop.png.");
            return;
        }

        BindPrefab(idle);
        BindDropClip(frames, idle);
        AssetDatabase.SaveAssets();
        Debug.Log(
            $"[CompostDrop] OK — frames={frames.Length}, idle={idle.name}, " +
            $"prefab={PrefabPath}, clip={DropClipPath}.");
    }

    private static void BindPrefab(Sprite idle)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
        try
        {
            Transform trashRoot = FindDeep(root.transform, DropTrashRootName);
            if (trashRoot == null)
            {
                Debug.LogError($"[CompostDrop] '{DropTrashRootName}' introuvable dans le prefab.");
                return;
            }

            Transform trashBin = trashRoot.Find(TrashBinChildName);
            if (trashBin == null)
            {
                Debug.LogError($"[CompostDrop] '{TrashBinChildName}' introuvable sous DropTrashRoot.");
                return;
            }

            Image image = trashBin.GetComponent<Image>();
            if (image == null)
            {
                Debug.LogError("[CompostDrop] Image manquante sur TrashBin.");
                return;
            }

            image.sprite = idle;
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;

            RectTransform rect = trashBin as RectTransform;
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(TrashBinWidth, TrashBinHeight);
                rect.anchoredPosition = new Vector2(0f, -20f);
            }

            ShopItemPopupView view = root.GetComponentInChildren<ShopItemPopupView>(true);
            if (view != null)
            {
                SerializedObject so = new SerializedObject(view);
                SerializedProperty duration = so.FindProperty("dropTrashDuration");
                if (duration != null)
                {
                    duration.floatValue = DropDurationSeconds;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void BindDropClip(Sprite[] frames, Sprite idle)
    {
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(DropClipPath);
        if (clip == null)
        {
            Debug.LogError($"[CompostDrop] Clip introuvable: {DropClipPath}");
            return;
        }

        // TrashBin sprite flipbook (drop frames then idle).
        EditorCurveBinding spriteBinding = EditorCurveBinding.PPtrCurve(
            TrashBinChildName,
            typeof(Image),
            "m_Sprite");

        ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[frames.Length + 1];
        float step = DropDurationSeconds / Mathf.Max(1, frames.Length);
        for (int i = 0; i < frames.Length; i++)
        {
            keys[i] = new ObjectReferenceKeyframe
            {
                time = i * step,
                value = frames[i]
            };
        }

        keys[frames.Length] = new ObjectReferenceKeyframe
        {
            time = DropDurationSeconds,
            value = idle
        };

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keys);

        // Align clip end with dropTrashDuration.
        SerializedObject clipSo = new SerializedObject(clip);
        SerializedProperty settings = clipSo.FindProperty("m_AnimationClipSettings");
        if (settings != null)
        {
            SerializedProperty stop = settings.FindPropertyRelative("m_StopTime");
            if (stop != null)
                stop.floatValue = DropDurationSeconds;
            clipSo.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(clip);
    }

    private static Sprite[] LoadDropFramesSorted(string assetPath)
    {
        return AssetDatabase.LoadAllAssetsAtPath(assetPath)
            .OfType<Sprite>()
            .Where(s => Regex.IsMatch(s.name, @"^CompostDrop_\d{2}$"))
            .OrderBy(s => s.name, System.StringComparer.Ordinal)
            .ToArray();
    }

    private static Sprite LoadSpriteByName(string assetPath, string spriteName)
    {
        return AssetDatabase.LoadAllAssetsAtPath(assetPath)
            .OfType<Sprite>()
            .FirstOrDefault(s => s.name == spriteName);
    }

    private static void EnsureFullRect(string texturePath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null)
            return;

        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        if (settings.spriteMeshType == SpriteMeshType.FullRect)
            return;

        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
        importer.SaveAndReimport();
    }

    private static Transform FindDeep(Transform root, string name)
    {
        if (root.name == name)
            return root;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindDeep(root.GetChild(i), name);
            if (found != null)
                return found;
        }

        return null;
    }
}
