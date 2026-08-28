using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contrôle l'ordre de progression d'une plante à travers ses différents stades de croissance.
/// Leafy   : Growing → Mature (récolte) → Flowering → Seedling  (laitue, épinard…)
/// Fruiting: Growing → Flowering → Mature (récolte) → Seedling  (tomate, poivron…)
/// </summary>
/// <remarks>
/// Le pattern de croissance détermine le type de plante.
/// - Leafy : plante qui produit des feuilles.
/// - Fruiting : plante qui produit des fruits.
/// </remarks>
public enum PlantGrowthPattern
{
    Leafy    = 0, // valeur par défaut — préserve le comportement des assets existants
    Fruiting = 1,
}

[CreateAssetMenu(menuName = "Game/Data/Ferme/Plante (définition)", fileName = "Plante_")]
public class PlantDefinition : ScriptableObject
{
    [Header("Identity")]
    public string plantId;
    public string displayName;

    [Header("Growth Pattern")]
    [Tooltip("Leafy   : … Growing → Mature (récolte) → Flowering → Seedling\n" +
             "Fruiting: … Growing → Flowering → Mature (récolte) → Seedling")]
    [SerializeField] private PlantGrowthPattern growthPattern = PlantGrowthPattern.Leafy;

    /// <summary>Profil de progression utilisé par PlantGrow pour ordonner la séquence de croissance.</summary>
    public PlantGrowthPattern GrowthPattern => growthPattern;

    [Header("Harvest")]
    [Tooltip("Une entrée par stade où le joueur peut récolter. À un instant T, seul le stade courant (PlantGrow) est actif : " +
             "ex. récolter tôt à Mature ou attendre Seedling pour un autre item — pas deux récoltes d'affilée sur la même plante.\n" +
             "Chaque entrée : stage, harvestItemId, quantités min/max.")]
    public HarvestStageConfig[] harvestStages = Array.Empty<HarvestStageConfig>();

    public int maxHarvestCount = 1;

    /// <summary>
    /// Retourne la config de récolte pour le stade donné, ou null si ce stade n'est pas récoltable.
    /// </summary>
    public HarvestStageConfig? GetHarvestConfig(PlantGrow.GrowthStage stage)
    {
        foreach (HarvestStageConfig config in harvestStages)
        {
            if (config.stage == stage)
                return config;
        }
        return null;
    }

    [Header("Stage Sprites (2D)")]
    [Tooltip("Slot Graine — identique pour les deux profils.")]
    public Sprite spriteGraine;
    [Tooltip("Slot Starting — identique pour les deux profils.")]
    public Sprite spriteStarting;
    [Tooltip("Slot Baby — identique pour les deux profils.")]
    public Sprite spriteBaby;
    [Tooltip("Slot Growing — identique pour les deux profils.")]
    public Sprite spriteGrowing;
    [Tooltip("Slot Mature — Feuille : stade récolte. Fruit : stade récolte après floraison.")]
    public Sprite spriteMature;
    [Tooltip("Slot Flowering — Feuille : après récolte. Fruit : avant maturation (récolte).")]
    public Sprite spriteFlowering;
    [Tooltip("Slot Seedling — dernier stade pour les deux profils (fin de cycle / production de graines).")]
    public Sprite spriteSeedling;

    /// <summary>Sprite configuré pour le stade donné, ou null si aucun. Source unique du mapping stade→sprite.</summary>
    public Sprite GetSprite(PlantGrow.GrowthStage stage) => stage switch
    {
        PlantGrow.GrowthStage.Graine    => spriteGraine,
        PlantGrow.GrowthStage.Starting  => spriteStarting,
        PlantGrow.GrowthStage.Baby      => spriteBaby,
        PlantGrow.GrowthStage.Growing   => spriteGrowing,
        PlantGrow.GrowthStage.Mature    => spriteMature,
        PlantGrow.GrowthStage.Flowering => spriteFlowering,
        PlantGrow.GrowthStage.Seedling  => spriteSeedling,
        _                               => null
    };

    [Header("Grid Placement")]
    [Tooltip("Relative offsets from the placement origin. Must always include (0,0).")]
    public Vector2Int[] footprint = { Vector2Int.zero };

    [Tooltip("World-space offset applied to the sprite GameObject after placement. " +
             "Use this to align sprites whose pivot does not match the anchor cell center.")]
    public Vector2 spriteWorldOffset = Vector2.zero;

    /// <summary>Returns all absolute grid cells occupied by this plant given a placement origin.</summary>
    public IEnumerable<Vector2Int> GetOccupiedCells(Vector2Int origin)
    {
        foreach (Vector2Int offset in footprint)
            yield return origin + offset;
    }

    /// <summary>
    /// Ancres candidates lorsque le joueur clique une cellule <paramref name="targetCell"/> :
    /// pour chaque offset du footprint, anchor = targetCell - offset.
    /// </summary>
    public IEnumerable<Vector2Int> EnumerateAnchorCandidatesForCell(Vector2Int targetCell)
    {
        if (footprint == null || footprint.Length == 0)
            yield break;

        foreach (Vector2Int offset in footprint)
            yield return targetCell - offset;
    }

    [Header("Insecte Flowering")]
    [Tooltip("None = pas d'insecte. Bee / Butterfly = espèce fixe. RandomBeeOrButterfly = 50/50 au démarrage Flowering.")]
    [SerializeField] private InsectKind insectKind = InsectKind.None;

    [Tooltip("Vitesse de vol monde (0 = défaut script ~0.8).")]
    [SerializeField] private float insectMoveSpeed;

    [Tooltip("Durée min butinage (0 = défaut script).")]
    [SerializeField] private float forageDurationMin;

    [Tooltip("Durée max butinage (0 = défaut script).")]
    [SerializeField] private float forageDurationMax;

    /// <summary>Config Inspector (peut être RandomBeeOrButterfly).</summary>
    public InsectKind InsectKind => insectKind;

    /// <summary>
    /// Espèce runtime pour ce cycle Flowering : résout RandomBeeOrButterfly en Bee ou Butterfly.
    /// </summary>
    public InsectKind ResolveRuntimeInsectKind()
    {
        if (insectKind == InsectKind.None)
            return InsectKind.None;

        if (insectKind == InsectKind.RandomBeeOrButterfly)
            return UnityEngine.Random.value < 0.5f ? InsectKind.Bee : InsectKind.Butterfly;

        return insectKind;
    }

    /// <summary>Vitesse vol override (0 = défaut follower).</summary>
    public float InsectMoveSpeed => insectMoveSpeed;

    /// <summary>Butinage min override (0 = défaut follower).</summary>
    public float ForageDurationMin => forageDurationMin;

    /// <summary>Butinage max override (0 = défaut follower).</summary>
    public float ForageDurationMax => forageDurationMax;

    [Header("Stage Durations (seconds)")]
    [Tooltip("Duration for each stage before automatically advancing to the next one. Leave at 0 to skip auto-advance for that stage.")]
    public StageDuration[] stageDurations = Array.Empty<StageDuration>();

    /// <summary>Returns the duration configured for a given stage, or 0 if not found.</summary>
    public float GetDuration(PlantGrow.GrowthStage stage)
    {
        if (stageDurations == null)
            return 0f;

        foreach (StageDuration entry in stageDurations)
        {
            if (entry.stage == stage)
                return entry.durationSeconds;
        }
        return 0f;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (footprint == null || footprint.Length == 0 ||
            !Array.Exists(footprint, offset => offset == Vector2Int.zero))
        {
            Debug.LogWarning($"[PlantDefinition] '{plantId}' : le footprint doit contenir (0,0) comme cellule d'ancrage.", this);
        }
    }
#endif
}

[Serializable]
public struct HarvestStageConfig
{
    [Tooltip("Stade de croissance auquel cette récolte est disponible.")]
    public PlantGrow.GrowthStage stage;

    [Tooltip("Id de l'item récolté à ce stade (doit exister dans l'ItemDatabase).")]
    public string harvestItemId;

    [Min(1)] public int harvestAmountMin;
    [Min(1)] public int harvestAmountMax;
}

[Serializable]
public struct StageDuration
{
    public PlantGrow.GrowthStage stage;
    [Min(0f)] public float durationSeconds;
}