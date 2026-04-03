using UnityEngine;

/// <summary>
/// Manages a plant's growth state and updates its sprite based on PlantDefinition.
/// Automatically advances to the next stage after the configured duration.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlantGrow : MonoBehaviour
{
    public enum GrowthStage { Graine, Starting, Baby, Growing, Mature, Flowering, Seedling }

    private static readonly GrowthStage[] StageOrder =
    {
        GrowthStage.Graine,
        GrowthStage.Starting,
        GrowthStage.Baby,
        GrowthStage.Growing,
        GrowthStage.Mature,
        GrowthStage.Flowering,
        GrowthStage.Seedling,
    };

    [SerializeField] private PlantDefinition plantDefinition;
    [SerializeField] private GrowthStage initialStage = GrowthStage.Graine;

    private SpriteRenderer spriteRenderer;
    private GrowthStage currentStage;
    private float stageTimer;
    private float currentStageDuration;

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

        // Only update the sprite preview — skip timer logic in editor
        spriteRenderer.sprite = GetSpriteForStage(initialStage);
    }

    private void Start()
    {
        SetStage(initialStage);
    }

    private void Update()
    {
        if (currentStageDuration <= 0f)
            return;

        stageTimer += Time.deltaTime;

        if (stageTimer >= currentStageDuration)
            AdvanceToNextStage();
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

        stageTimer = 0f;
        currentStageDuration = plantDefinition.GetDuration(stage);
    }

    /// <summary>Returns the current growth stage.</summary>
    public GrowthStage CurrentStage => currentStage;

    /// <summary>Progress within the current stage, from 0 to 1.</summary>
    public float StageProgress => currentStageDuration > 0f
        ? Mathf.Clamp01(stageTimer / currentStageDuration)
        : 1f;

    private void AdvanceToNextStage()
    {
        int currentIndex = System.Array.IndexOf(StageOrder, currentStage);
        int nextIndex = currentIndex + 1;

        if (nextIndex >= StageOrder.Length)
        {
            stageTimer = currentStageDuration;
            return;
        }

        SetStage(StageOrder[nextIndex]);
    }

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
