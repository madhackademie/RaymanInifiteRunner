using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Manages a plant's growth state and updates its sprite based on PlantDefinition.
/// Automatically advances to the next stage after the configured duration.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlantGrow : MonoBehaviour
{
    /// <summary>
    /// Ordre d’index C# : <b>0 = Graine</b>, 1 = Starting, 2 = Baby, … — la progression ne « commence pas à 1 » dans le code,
    /// sauf si le prefab avait <see cref="initialStage"/> réglé sur Starting (ancien défaut) ou si un autre script force un stade.
    /// À la pose, <see cref="BiofiltreManager"/> appelle <see cref="SetStage"/>(Graine) juste après l’instanciation.
    /// </summary>
    public enum GrowthStage { Graine, Starting, Baby, Growing, Mature, Flowering, Seedling }

    private static readonly GrowthStage[] StageOrderLeafy =
    {
        GrowthStage.Graine,
        GrowthStage.Starting,
        GrowthStage.Baby,
        GrowthStage.Growing,
        GrowthStage.Mature,    // récolte feuilles
        GrowthStage.Flowering,
        GrowthStage.Seedling,
    };

    // Fruiting : floraison avant maturation (tomate, poivron…)
    private static readonly GrowthStage[] StageOrderFruiting =
    {
        GrowthStage.Graine,
        GrowthStage.Starting,
        GrowthStage.Baby,
        GrowthStage.Growing,
        GrowthStage.Flowering,
        GrowthStage.Mature,    // récolte fruit
        GrowthStage.Seedling,
    };

    /// <summary>Séquence de stades active, déterminée par le GrowthPattern de la PlantDefinition assignée.</summary>
    private GrowthStage[] ActiveStageOrder => plantDefinition != null && plantDefinition.GrowthPattern == PlantGrowthPattern.Fruiting
        ? StageOrderFruiting
        : StageOrderLeafy;

    [SerializeField] private PlantDefinition plantDefinition;
    [SerializeField] private GrowthStage initialStage = GrowthStage.Graine;

    private SpriteRenderer spriteRenderer;
    private GrowthStage currentStage;
    private float stageTimer;
    private float currentStageDuration;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialisation du stade ici (et pas dans Start) : sinon, après Instantiate,
        // Unity appelle Start sur PlantGrow *après* le Start de BiofiltreManager, ce qui
        // réapplique initialStage et écrase SetStage(Graine) / SetStageWithElapsed (save JSON).
        if (plantDefinition != null)
            SetStage(initialStage);
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || plantDefinition == null)
            return;

        // Delay sprite preview update to avoid SendMessage warning during OnValidate.
#if UNITY_EDITOR
        EditorApplication.delayCall -= ApplyEditorPreviewSpriteSafely;
        EditorApplication.delayCall += ApplyEditorPreviewSpriteSafely;
#endif
    }

#if UNITY_EDITOR
    private void ApplyEditorPreviewSpriteSafely()
    {
        if (this == null || spriteRenderer == null || plantDefinition == null)
            return;

        spriteRenderer.sprite = GetSpriteForStage(initialStage);
    }
#endif

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
        WarnIfStageHasZeroDurationButNotTerminal(stage);
    }

    /// <summary>
    /// Une durée 0 = pas d'avance auto (Update ignore). OK pour le dernier stade ; sinon la plante semble "gelée".
    /// </summary>
    private void WarnIfStageHasZeroDurationButNotTerminal(GrowthStage stage)
    {
        if (currentStageDuration > 0f || plantDefinition == null)
            return;

        GrowthStage[] order = ActiveStageOrder;
        int idx = System.Array.IndexOf(order, stage);
        if (idx >= 0 && idx < order.Length - 1)
        {
            Debug.LogWarning(
                $"[PlantGrow] '{gameObject.name}' : durée 0s pour le stade {stage} dans '{plantDefinition.name}'. " +
                "La croissance automatique est bloquée sur ce stade — vérifie PlantDefinition.stageDurations dans l'Inspector.",
                plantDefinition);
        }
    }

    /// <summary>Returns the current growth stage.</summary>
    public GrowthStage CurrentStage => currentStage;

    /// <summary>Progress within the current stage, from 0 to 1.</summary>
    public float StageProgress => currentStageDuration > 0f
        ? Mathf.Clamp01(stageTimer / currentStageDuration)
        : 1f;

    /// <summary>Temps ecoule (secondes) dans le stade courant.</summary>
    public float CurrentStageElapsedSeconds => stageTimer;

    /// <summary>Force un stade et un temps ecoule dans ce stade.</summary>
    public void SetStageWithElapsed(GrowthStage stage, float elapsedSeconds)
    {
        SetStage(stage);
        stageTimer = Mathf.Clamp(elapsedSeconds, 0f, currentStageDuration);
    }

    /// <summary>
    /// Fait avancer la plante d'une duree donnee en secondes (reprise hors ligne).
    /// </summary>
    public void AdvanceBySeconds(float deltaSeconds)
    {
        if (deltaSeconds <= 0f)
            return;

        float remaining = deltaSeconds;
        int safety = 0;

        while (remaining > 0f && safety < 64)
        {
            safety++;

            if (currentStageDuration <= 0f)
                break;

            float toEnd = currentStageDuration - stageTimer;
            if (remaining < toEnd)
            {
                stageTimer += remaining;
                remaining = 0f;
                break;
            }

            remaining -= toEnd;
            AdvanceToNextStage();

            if (StageProgress >= 1f && currentStageDuration > 0f)
                break;
        }
    }

    private void AdvanceToNextStage()
    {
        GrowthStage[] order = ActiveStageOrder;
        int currentIndex = System.Array.IndexOf(order, currentStage);
        int nextIndex    = currentIndex + 1;

        if (nextIndex >= order.Length)
        {
            stageTimer = currentStageDuration;
            return;
        }

        SetStage(order[nextIndex]);
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
