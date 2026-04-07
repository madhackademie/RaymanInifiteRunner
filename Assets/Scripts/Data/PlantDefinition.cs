using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Plants/Plant Definition", fileName = "PlantDefinition")]
public class PlantDefinition : ScriptableObject
{
    [Header("Identity")]
    public string plantId;
    public string displayName;

    [Header("Harvest")]
    public string harvestItemId;
    public int harvestAmountMin = 1;
    public int harvestAmountMax = 1;
    public int maxHarvestCount = 1;

    [Header("Stage Sprites (2D)")]
    public Sprite spriteGraine;       // 0 - 0_GraineGermé
    public Sprite spriteStarting;     // 1 - 01_StartingPlant
    public Sprite spriteBaby;         // 2 - 02_BabyLaituce
    public Sprite spriteGrowing;      // 3 - 03_GrowingLaituce
    public Sprite spriteMature;       // 4 - 04_MatureLaituce
    public Sprite spriteFlowering;    // 5 - 05_FlowerLaituce
    public Sprite spriteSeedling;     // 6 - 06_SeedlingLaituce

    [Header("Grid Placement")]
    [Tooltip("Relative offsets from the placement origin. Must always include (0,0).")]
    public Vector2Int[] footprint = { Vector2Int.zero };

    [Tooltip("World-space offset applied to the sprite GameObject after placement. " +
             "Use this to align sprites whose pivot does not match the anchor cell center. " +
             "Example: a 2x2 plant with pivot at bottom-center needs offset (+0.5, 0) with cell size 1.")]
    public Vector2 spriteWorldOffset = Vector2.zero;

    /// <summary>Returns all absolute grid cells occupied by this plant given a placement origin.</summary>
    public IEnumerable<Vector2Int> GetOccupiedCells(Vector2Int origin)
    {
        foreach (Vector2Int offset in footprint)
            yield return origin + offset;
    }

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
public struct StageDuration
{
    public PlantGrow.GrowthStage stage;
    [Min(0f)] public float durationSeconds;
}