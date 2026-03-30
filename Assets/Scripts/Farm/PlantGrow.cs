using UnityEngine;

/// <summary>
/// Manages a plant's growth state and updates its sprite based on PlantDefinition.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlantGrow : MonoBehaviour
{
    public enum GrowthStage { Graine, Starting, Baby, Growing, Mature, Flowering, Seedling }

    [SerializeField] private PlantDefinition plantDefinition;
    [SerializeField] private GrowthStage initialStage = GrowthStage.Starting;

    private SpriteRenderer spriteRenderer;
    private GrowthStage currentStage;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || plantDefinition == null)
            return;

        SetStage(initialStage);
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
        GrowthStage.Graine    => plantDefinition.spriteGraine,
        GrowthStage.Starting  => plantDefinition.spriteStarting,
        GrowthStage.Baby      => plantDefinition.spriteBaby,
        GrowthStage.Growing   => plantDefinition.spriteGrowing,
        GrowthStage.Mature    => plantDefinition.spriteMature,
        GrowthStage.Flowering => plantDefinition.spriteFlowering,
        GrowthStage.Seedling  => plantDefinition.spriteSeedling,
        _                     => null
    };
}
