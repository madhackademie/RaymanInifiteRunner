using UnityEngine;

[CreateAssetMenu(menuName = "Game/Plants/Plant Definition", fileName = "PlantDefinition")]
public class PlantDefinition : ScriptableObject
{
    [Header("Identity")]
    public string plantId;
    public string displayName;

    [Header("Growth")]
    public float growthDurationSeconds = 300f;

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
}