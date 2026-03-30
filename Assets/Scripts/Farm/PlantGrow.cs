using UnityEngine;

/// <summary>
/// Manages a plant's growth state and updates its sprite based on PlantDefinition.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlantGrow : MonoBehaviour
{
    public enum GrowthStage { Seedling, Baby, Growing, Mature, Bolting }

    [SerializeField] private PlantDefinition plantDefinition;
    [SerializeField] private GrowthStage initialStage = GrowthStage.Baby;

    private SpriteRenderer spriteRenderer;
    private GrowthStage currentStage;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetStage(initialStage);
    }

    /// <summary>Sets the plant to a specific growth stage and updates its sprite.</summary>
    public void SetStage(GrowthStage stage)
    {
        if (plantDefinition == null)
        {
            Debug.LogWarning($"[PlantGrow] No PlantDefinition assigned on {gameObject.name}.");
            return;
        }

        currentStage = stage;
        spriteRenderer.sprite = GetSpriteForStage(stage);
    }

    /// <summary>Returns the current growth stage.</summary>
    public GrowthStage CurrentStage => currentStage;

    private Sprite GetSpriteForStage(GrowthStage stage) => stage switch
    {
        GrowthStage.Seedling => plantDefinition.seedlingSprite,
        GrowthStage.Baby     => plantDefinition.babyLeafSprite,
        GrowthStage.Growing  => plantDefinition.growingSprite,
        GrowthStage.Mature   => plantDefinition.matureSprite,
        GrowthStage.Bolting  => plantDefinition.boltingSprite,
        _                    => null
    };
}
