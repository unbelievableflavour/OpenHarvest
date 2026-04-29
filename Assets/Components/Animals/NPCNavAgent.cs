using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Shared NavMesh wandering/follow behaviour for farm animals, store NPCs, or any character
/// that should idle, roam inside an area, or follow a transform.
///
/// Requires a baked NavMesh in the scene. The agent is auto-added via
/// <see cref="RequireComponent"/>, so existing prefabs only need to be re-saved once the
/// NavMesh is baked for the farm terrain.
///
/// For NPCs in enclosed spaces, set <see cref="movesWhenFurtherAwayThen"/> to 0 so they keep
/// wandering while the player is nearby.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NPCNavAgent : MonoBehaviour
{
    const float FollowStopDistance = 2f;
    const float FollowRefreshInterval = 0.25f;
    const float FollowMoveSpeed = 5f;
    const float FollowAcceleration = 10f;

    [Header("Animation")]
    public Animator animator;
    [Tooltip("Animator state name to play while moving.")]
    public string walkStateName = "WalkingAnimation";
    [Tooltip("Animator state name to play while idle.")]
    public string idleStateName = "IdleAnimation";

    [Header("Wander")]
    [Tooltip("Radius around the spawn anchor in which to pick random wander targets.")]
    public float wanderRadius = 6f;
    [Tooltip("Min seconds to idle between wander destinations.")]
    public float wanderMinInterval = 10f;
    [Tooltip("Max seconds to idle between wander destinations.")]
    public float wanderMaxInterval = 15f;
    [Tooltip("Delay before the first wander destination is picked after spawn. Keep short for fast feedback; the normal idle intervals kick in afterwards.")]
    public float firstWanderDelay = 1f;

    [Header("Follow")]
    [Tooltip("If set, the animal will follow this transform instead of wandering (speed/stop/refresh are fixed in code).")]
    public Transform followTarget;

    [Header("Activity Culling")]
    [Tooltip("Only move when the main camera is further away than this distance. Set to 0 to disable culling.")]
    public float movesWhenFurtherAwayThen = 5f;

    [Header("Interaction")]
    [Tooltip("How fast the NPC yaws toward the player while the interaction menu is open.")]
    [SerializeField] private float interactionAimRotationSpeed = 8f;

    NavMeshAgent agent;
    Transform mainCam;
    Vector3 spawnAnchor;
    float nextWanderAt;
    float nextFollowRefreshAt;
    bool isActive = true;
    bool movementSuppressed;
    Transform interactionAimTarget;
    static NPCNavAgent s_activeInteractionNav;
    bool _restoreNavAgentUpdateRotation;
    float _defaultNavAgentSpeed;
    float _defaultNavAgentAcceleration;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _defaultNavAgentSpeed = agent.speed;
        _defaultNavAgentAcceleration = agent.acceleration;

        // NavMeshAgent writes to transform directly; any attached Rigidbody must be kinematic
        // to avoid fighting the agent's position updates.
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }
    }

    private void OnDestroy()
    {
        if (s_activeInteractionNav == this)
        {
            s_activeInteractionNav = null;
            if (agent != null)
            {
                agent.updateRotation = _restoreNavAgentUpdateRotation;
            }
        }
    }

    protected virtual void Start()
    {
        if (Camera.main != null)
        {
            mainCam = Camera.main.transform;
        }

        // Capture the anchor AFTER the agent has had a chance to snap onto the NavMesh;
        // sampling a valid point avoids anchoring wander around a wall-edge snap position.
        Vector3 seed = transform.position;
        if (NavMesh.SamplePosition(seed, out var hit, 5f, NavMesh.AllAreas))
        {
            spawnAnchor = hit.position;
        }
        else
        {
            spawnAnchor = seed;
            Debug.LogWarning($"{name}: spawned off the NavMesh — wander may behave oddly. Re-bake the NavMesh or move the spawn onto it.", this);
        }

        nextWanderAt = Time.time + firstWanderDelay;
    }

    protected virtual void Update()
    {
        if (interactionAimTarget != null)
        {
            TickInteractionAim();
        }

        if (movementSuppressed)
        {
            return;
        }

        // Follow must not be blocked by activity culling (wander) or the agent never
        // receives destinations / StartMoving() and the walk anim won't play while close.
        if (followTarget != null)
        {
            TickFollow();
            return;
        }

        UpdateActiveState();
        if (!isActive)
        {
            return;
        }

        TickWander();
    }

    void UpdateActiveState()
    {
        bool shouldBeActive = true;
        if (movesWhenFurtherAwayThen > 0f && mainCam != null)
        {
            shouldBeActive = Vector3.Distance(transform.position, mainCam.position) >= movesWhenFurtherAwayThen;
        }

        if (shouldBeActive == isActive)
        {
            return;
        }

        isActive = shouldBeActive;
        if (!isActive)
        {
            StopMoving();
        }
    }

    void TickWander()
    {
        if (agent.pathPending)
        {
            return;
        }

        bool arrived = agent.remainingDistance <= agent.stoppingDistance + 0.05f;
        if (arrived)
        {
            if (agent.hasPath || agent.velocity.sqrMagnitude > 0.01f)
            {
                StopMoving();
            }

            if (Time.time < nextWanderAt)
            {
                return;
            }

            PickNewWanderDestination();
        }
    }

    void TickFollow()
    {
        if (Time.time < nextFollowRefreshAt)
        {
            return;
        }
        nextFollowRefreshAt = Time.time + FollowRefreshInterval;

        float distanceToTarget = Vector3.Distance(transform.position, followTarget.position);
        if (distanceToTarget <= FollowStopDistance)
        {
            StopMoving();
            return;
        }

        StartMoving();
        agent.stoppingDistance = FollowStopDistance;
        agent.SetDestination(followTarget.position);
    }

    void PickNewWanderDestination()
    {
        Vector2 offset = Random.insideUnitCircle * wanderRadius;
        Vector3 candidate = spawnAnchor + new Vector3(offset.x, 0f, offset.y);

        if (NavMesh.SamplePosition(candidate, out var hit, wanderRadius, NavMesh.AllAreas))
        {
            StartMoving();
            agent.stoppingDistance = 0f;
            agent.SetDestination(hit.position);
        }

        ScheduleNextWander();
    }

    void ScheduleNextWander()
    {
        nextWanderAt = Time.time + Random.Range(wanderMinInterval, wanderMaxInterval);
    }

    void StartMoving()
    {
        if (agent.isOnNavMesh && agent.isStopped)
        {
            agent.isStopped = false;
        }

        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName(walkStateName))
        {
            animator.Play(walkStateName);
        }
    }

    void StopMoving()
    {
        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName(idleStateName))
        {
            animator.Play(idleStateName);
        }
    }

    /// <summary>Start following a target transform (e.g. the player).</summary>
    public void Follow(Transform target)
    {
        followTarget = target;
        nextFollowRefreshAt = 0f;
        if (agent != null)
        {
            agent.speed = FollowMoveSpeed;
            agent.acceleration = FollowAcceleration;
        }
    }

    /// <summary>Stop following and resume wandering around the current position.</summary>
    public void StopFollowing()
    {
        followTarget = null;
        if (agent != null)
        {
            agent.speed = _defaultNavAgentSpeed;
            agent.acceleration = _defaultNavAgentAcceleration;
        }

        spawnAnchor = transform.position;
        ScheduleNextWander();
        StopMoving();
    }

    /// <summary>
    /// When true, navigation and wander/follow logic are frozen (e.g. while the player is
    /// interacting). The agent stops on the NavMesh until released.
    /// </summary>
    public void SetMovementSuppressed(bool suppressed)
    {
        if (movementSuppressed == suppressed)
        {
            return;
        }

        movementSuppressed = suppressed;
        if (suppressed)
        {
            StopMoving();
        }
        else
        {
            nextWanderAt = Time.time + 0.25f;
        }
    }

    void TickInteractionAim()
    {
        Transform t = interactionAimTarget;
        if (t == null)
        {
            return;
        }

        Vector3 d = t.position - transform.position;
        d.y = 0f;
        if (d.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion look = Quaternion.LookRotation(d.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            look,
            Time.deltaTime * interactionAimRotationSpeed);
    }

    /// <summary>
    /// Face the player (or a view target) and stop moving while the interaction UI is open.
    /// Replaces any other NPC currently in this state.
    /// </summary>
    public void BeginInteractionAim(Transform lookAt)
    {
        if (s_activeInteractionNav != null && s_activeInteractionNav != this)
        {
            s_activeInteractionNav.EndInteractionAim();
        }

        s_activeInteractionNav = this;
        interactionAimTarget = lookAt;
        if (agent != null)
        {
            _restoreNavAgentUpdateRotation = agent.updateRotation;
            agent.updateRotation = false;
        }

        SetMovementSuppressed(true);
    }

    /// <summary>Stop aiming and resume normal navigation.</summary>
    public void EndInteractionAim()
    {
        if (s_activeInteractionNav == this)
        {
            s_activeInteractionNav = null;
        }

        interactionAimTarget = null;
        if (agent != null)
        {
            agent.updateRotation = _restoreNavAgentUpdateRotation;
        }

        SetMovementSuppressed(false);
    }

    public static void EndAnyInteractionAim()
    {
        s_activeInteractionNav?.EndInteractionAim();
    }

    public static Transform ResolvePlayerLookAtTransform()
    {
        if (Camera.main != null)
        {
            return Camera.main.transform;
        }

        return GameState.Instance != null ? GameState.Instance.currentPlayerPosition : null;
    }

    /// <summary>Rig to follow: prefers <see cref="CurrentGameState.currentPlayerPosition"/>, then <c>Camera.main</c>.</summary>
    public static Transform ResolvePlayerFollowTarget()
    {
        return GameState.Instance?.currentPlayerPosition ?? Camera.main?.transform;
    }
}
