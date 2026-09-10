using UnityEngine;

/// <summary>
/// FSM insecte : Fly le long des edges, puis Forage sur chaque node.
/// Au RestartCircuit : rand sens de parcours (+1 / -1) ; l'espèce est appliquée via <see cref="ApplyVisualKind"/>.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class InsectPathFollower : MonoBehaviour
{
    private const string AnimStateFly = "Fly";
    private const string AnimStateForage = "Forage";
    private const float DefaultMoveSpeed = 0.8f;
    private const float DefaultForageMin = 0.5f;
    private const float DefaultForageMax = 1.5f;
    private const float DefaultFlipDeadZone = 0.01f;
    /// <summary>L’insecte passe juste devant sa plante, sans sauter le tri iso des autres plants.</summary>
    private const int SortingOrderBoostVsPlant = 1;
    /// <summary>Z local vers la caméra 2D (z négatif) pour la Scene view / tri Default.</summary>
    private const float LocalZInFront = -0.05f;

    private enum State
    {
        FlyAlongEdge,
        Forage,
    }

    [Header("Refs")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Visuals (espèce)")]
    [SerializeField] private RuntimeAnimatorController beeController;
    [SerializeField] private RuntimeAnimatorController butterflyController;

    [Header("Motion")]
    [SerializeField] private float moveSpeed = DefaultMoveSpeed;
    [SerializeField] private float forageDurationMin = DefaultForageMin;
    [SerializeField] private float forageDurationMax = DefaultForageMax;
    [SerializeField] private float flipDeadZone = DefaultFlipDeadZone;

    private InsectPathAnchor path;
    private State state;
    private int currentNodeIndex;
    private int targetNodeIndex;
    private int pathDirection = 1;
    private float forageTimer;
    private float forageDuration;
    private Vector3 flyStart;
    private float flyProgress;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        SyncFromParentPlant();
    }

    /// <summary>Lie ce follower à un path (appelé par InsectPathAnchor).</summary>
    public void BindPath(InsectPathAnchor pathAnchor)
    {
        path = pathAnchor;
    }

    /// <summary>Applique vitesses / durées depuis une PlantDefinition.</summary>
    public void ApplyDefinitionOverrides(float speed, float forageMin, float forageMax)
    {
        if (speed > 0f)
            moveSpeed = speed;
        if (forageMin > 0f)
            forageDurationMin = forageMin;
        if (forageMax >= forageDurationMin)
            forageDurationMax = forageMax;
    }

    /// <summary>
    /// Aligne le sprite insecte sur le <paramref name="plantRenderer"/> (ordre iso runtime).
    /// Le prefab Bee est à order 5 ; la plante posée est à 20+cell, donc sans sync l’insecte passe derrière.
    /// </summary>
    public void SyncSortingOrderWithPlant(SpriteRenderer plantRenderer)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || plantRenderer == null)
            return;

        spriteRenderer.sortingLayerID = plantRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = plantRenderer.sortingOrder + SortingOrderBoostVsPlant;
        ApplyLocalZInFront();
    }

    private void SyncFromParentPlant()
    {
        SpriteRenderer plantRenderer = FindAncestorPlantRenderer();
        if (plantRenderer != null)
            SyncSortingOrderWithPlant(plantRenderer);
    }

    private SpriteRenderer FindAncestorPlantRenderer()
    {
        Transform ancestor = transform.parent;
        while (ancestor != null)
        {
            if (ancestor.TryGetComponent(out SpriteRenderer plantRenderer))
                return plantRenderer;
            ancestor = ancestor.parent;
        }

        return null;
    }

    private void ApplyLocalZInFront()
    {
        Vector3 local = transform.localPosition;
        local.z = LocalZInFront;
        transform.localPosition = local;
    }

    /// <summary>Swap le controller Animator (Bee / Butterfly). Appeler avant RestartCircuit.</summary>
    public void ApplyVisualKind(InsectKind kind)
    {
        if (animator == null)
            return;

        RuntimeAnimatorController chosen = kind switch
        {
            InsectKind.Butterfly => butterflyController != null ? butterflyController : beeController,
            _ => beeController != null ? beeController : butterflyController,
        };

        if (chosen == null)
            return;

        if (animator.runtimeAnimatorController != chosen)
            animator.runtimeAnimatorController = chosen;
    }

    /// <summary>
    /// Relance le circuit : rand sens (+1 / -1), départ node 0, premier vol vers le voisin.
    /// </summary>
    public void RestartCircuit()
    {
        if (path == null || path.Nodes == null || path.Nodes.Count < 2)
        {
            enabled = false;
            return;
        }

        enabled = true;
        pathDirection = Random.value < 0.5f ? 1 : -1;
        currentNodeIndex = 0;

        Transform start = path.Nodes[0];
        if (start != null)
        {
            transform.position = start.position;
            ApplyLocalZInFront();
        }

        BeginFlyTo(GetNextNodeIndex(currentNodeIndex));
    }

    private void Update()
    {
        if (path == null || path.Nodes == null || path.Nodes.Count < 2)
            return;

        if (state == State.FlyAlongEdge)
            TickFly();
        else
            TickForage();
    }

    private int GetNextNodeIndex(int fromIndex)
    {
        int count = path.Nodes.Count;
        return (fromIndex + pathDirection + count) % count;
    }

    private void BeginFlyTo(int nextIndex)
    {
        targetNodeIndex = nextIndex;
        Transform target = path.Nodes[targetNodeIndex];
        if (target == null)
            return;

        flyStart = transform.position;
        flyProgress = 0f;
        state = State.FlyAlongEdge;
        PlayAnim(AnimStateFly);
        UpdateFlip(target.position - flyStart);
    }

    private void TickFly()
    {
        Transform target = path.Nodes[targetNodeIndex];
        if (target == null)
            return;

        float distance = Vector3.Distance(flyStart, target.position);
        float step = distance > 0.001f ? (moveSpeed * Time.deltaTime) / distance : 1f;
        flyProgress = Mathf.Clamp01(flyProgress + step);
        transform.position = Vector3.Lerp(flyStart, target.position, flyProgress);
        ApplyLocalZInFront();

        Vector3 delta = target.position - transform.position;
        UpdateFlip(delta);

        if (flyProgress < 1f)
            return;

        currentNodeIndex = targetNodeIndex;
        BeginForage();
    }

    private void BeginForage()
    {
        state = State.Forage;
        forageDuration = Random.Range(forageDurationMin, forageDurationMax);
        forageTimer = 0f;
        PlayAnim(AnimStateForage);
    }

    private void TickForage()
    {
        forageTimer += Time.deltaTime;
        if (forageTimer < forageDuration)
            return;

        BeginFlyTo(GetNextNodeIndex(currentNodeIndex));
    }

    private void UpdateFlip(Vector3 direction)
    {
        if (spriteRenderer == null || Mathf.Abs(direction.x) <= flipDeadZone)
            return;

        spriteRenderer.flipX = direction.x < 0f;
    }

    private void PlayAnim(string stateName)
    {
        if (animator == null || !animator.isActiveAndEnabled)
            return;

        if (animator.runtimeAnimatorController == null)
            return;

        animator.Play(stateName, 0, 0f);
    }
}
