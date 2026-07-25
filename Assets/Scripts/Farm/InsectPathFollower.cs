using UnityEngine;

/// <summary>
/// FSM insecte : Fly le long des edges → Forage (butinage) sur chaque node → suivant.
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

    private enum State
    {
        FlyAlongEdge,
        Forage,
    }

    [Header("Refs")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Motion")]
    [SerializeField] private float moveSpeed = DefaultMoveSpeed;
    [SerializeField] private float forageDurationMin = DefaultForageMin;
    [SerializeField] private float forageDurationMax = DefaultForageMax;
    [SerializeField] private float flipDeadZone = DefaultFlipDeadZone;

    private InsectPathAnchor path;
    private State state;
    private int currentNodeIndex;
    private int targetNodeIndex;
    private float forageTimer;
    private float forageDuration;
    private Vector3 flyStart;
    private float flyProgress;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
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

    /// <summary>Relance le circuit depuis le premier node.</summary>
    public void RestartCircuit()
    {
        if (path == null || path.Nodes == null || path.Nodes.Count < 2)
        {
            enabled = false;
            return;
        }

        enabled = true;
        currentNodeIndex = 0;
        Transform start = path.Nodes[0];
        if (start != null)
            transform.position = start.position;

        BeginFlyTo((currentNodeIndex + 1) % path.Nodes.Count);
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

        int next = (currentNodeIndex + 1) % path.Nodes.Count;
        BeginFlyTo(next);
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

        animator.Play(stateName, 0, 0f);
    }
}
