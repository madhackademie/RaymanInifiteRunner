using UnityEngine;

[CreateAssetMenu(menuName = "Game/Plants/Plant Definition", fileName = "PlantDefinition")]
public class PlantDefinition : ScriptableObject
{
    [Header("Identity")]
    public string plantId;           // ex: "lettuce"
    public string displayName;       // ex: "Laitue"

    [Header("Growth")]
    public float growthDurationSeconds = 300f;

    [Header("Harvest")]
    public string harvestItemId;     // ex: "lettuce_item"
    public int harvestAmountMin = 1;
    public int harvestAmountMax = 1;
    public int maxHarvestCount = 1;  // récolte unique ou multiple

    [Header("Stage Sprites (2D)")]
    public Sprite seedlingSprite;
    public Sprite babyLeafSprite;
    public Sprite growingSprite;
    public Sprite matureSprite;
    public Sprite boltingSprite;
}