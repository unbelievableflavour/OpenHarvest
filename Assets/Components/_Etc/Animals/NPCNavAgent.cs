using System.Collections;
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
    public enum NpcNavAgentState
    {
        Idle,
        Follow,
        Interact,
    }

    const float FollowStopDistance = 2f;
    const float FollowRefreshInterval = 0.25f;
    const float FollowMoveSpeed = 5f;
    const float FollowAcceleration = 10f;
    const float FollowSpawnBehindDistance = 2f;

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
    [Tooltip("While following, warp near the player when horizontal distance exceeds this (e.g. after a scene load).")]
    public float followCatchUpDistance = 30f;

    [Header("Activity Culling")]
    [Tooltip("Only move when the main camera is further away than this distance. Set to 0 to disable culling.")]
    public float movesWhenFurtherAwayThen = 5f;

    [Header("Interaction")]
    [Tooltip("How fast the NPC yaws toward the player while the interaction menu is open.")]
    [SerializeField] private float interactionAimRotationSpeed = 8f;

    public NpcNavAgentState State { get; private set; } = NpcNavAgentState.Idle;

    NavMeshAgent agent;
    Transform mainCam;
    Vector3 spawnAnchor;
    float nextWanderAt;
    float nextFollowRefreshAt;
    bool isActive = true;
    Quaternion? lockedInteractionAim;
    bool interactionAimSettled;
    static NPCNavAgent s_activeInteractionNav;
    bool _savedAgentRotationPreference;
    bool _restoreNavAgentUpdateRotation = true;
    float _defaultNavAgentSpeed;
    float _defaultNavAgentAcceleration;

    protected virtual void Awake()
    {
        EnsureAgent();

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }
    }

    void EnsureAgent()
    {
        if (agent != null)
        {
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            return;
        }

        _defaultNavAgentSpeed = agent.speed;
        _defaultNavAgentAcceleration = agent.acceleration;
    }

    bool CanUseNavMeshPath()
    {
        EnsureAgent();
        return agent != null && agent.isOnNavMesh;
    }

    private void OnDestroy()
    {
        if (s_activeInteractionNav == this)
        {
            s_activeInteractionNav = null;
        }

        RestoreAgentRotation();
    }

    protected virtual void Start()
    {
        if (Camera.main != null)
        {
            mainCam = Camera.main.transform;
        }

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

        if (ShouldBeFollowing())
        {
            StartCoroutine(ResumeFollowOnStart());
        }
    }

    IEnumerator ResumeFollowOnStart()
    {
        yield return null;

        Transform player = ResolvePlayerFollowTarget();
        if (player != null)
        {
            Follow(player);
        }
    }

    protected virtual void Update()
    {
        switch (State)
        {
            case NpcNavAgentState.Interact:
                TickInteract();
                return;
            case NpcNavAgentState.Follow:
                TickFollowState();
                return;
            case NpcNavAgentState.Idle:
                TickIdle();
                return;
        }
    }

    void TickInteract()
    {
        if (interactionAimSettled || !lockedInteractionAim.HasValue)
        {
            return;
        }

        Quaternion look = lockedInteractionAim.Value;

        if (Quaternion.Angle(transform.rotation, look) < 5f)
        {
            transform.rotation = look;
            interactionAimSettled = true;
            return;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            look,
            Time.deltaTime * interactionAimRotationSpeed);
    }

    void TickFollowState()
    {
        if (followTarget == null)
        {
            return;
        }

        if (GetHorizontalDistanceToFollowTarget() > followCatchUpDistance)
        {
            RespawnNearFollowTarget();
            return;
        }

        EnsureAgent();
        if (agent == null)
        {
            return;
        }

        if (!CanUseNavMeshPath())
        {
            return;
        }

        if (Time.time < nextFollowRefreshAt)
        {
            return;
        }
        nextFollowRefreshAt = Time.time + FollowRefreshInterval;

        float distanceToTarget = GetHorizontalDistanceToFollowTarget();
        if (distanceToTarget <= FollowStopDistance)
        {
            StopMoving();
            return;
        }

        StartMoving();
        agent.stoppingDistance = FollowStopDistance;
        agent.SetDestination(followTarget.position);
    }

    void TickIdle()
    {
        EnsureAgent();
        if (agent == null || !CanUseNavMeshPath())
        {
            return;
        }

        UpdateActiveState();
        if (!isActive)
        {
            return;
        }

        if (agent.pathPending)
        {
            return;
        }

        bool arrived = agent.remainingDistance <= agent.stoppingDistance + 0.05f;
        if (!arrived)
        {
            return;
        }

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
        EnsureAgent();
        if (agent != null && agent.isOnNavMesh && agent.isStopped)
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
        EnsureAgent();
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName(idleStateName))
        {
            animator.Play(idleStateName);
        }
    }

    void SetState(NpcNavAgentState newState)
    {
        if (State == newState)
        {
            return;
        }

        OnExitState(State);
        State = newState;
        OnEnterState(newState);
    }

    void OnExitState(NpcNavAgentState exiting)
    {
        if (exiting != NpcNavAgentState.Interact)
        {
            return;
        }

        lockedInteractionAim = null;
        interactionAimSettled = false;
        RestoreAgentRotation();
    }

    void OnEnterState(NpcNavAgentState entering)
    {
        EnsureAgent();
        if (agent == null)
        {
            return;
        }

        switch (entering)
        {
            case NpcNavAgentState.Interact:
                DisableAgentRotation();
                StopMoving();
                break;
            case NpcNavAgentState.Follow:
                RestoreAgentRotation();
                agent.speed = FollowMoveSpeed;
                agent.acceleration = FollowAcceleration;
                nextFollowRefreshAt = 0f;
                break;
            case NpcNavAgentState.Idle:
                RestoreAgentRotation();
                agent.speed = _defaultNavAgentSpeed;
                agent.acceleration = _defaultNavAgentAcceleration;
                break;
        }
    }

    void DisableAgentRotation()
    {
        EnsureAgent();
        if (agent == null || _savedAgentRotationPreference)
        {
            return;
        }

        _restoreNavAgentUpdateRotation = agent.updateRotation;
        _savedAgentRotationPreference = true;
        agent.updateRotation = false;
    }

    void RestoreAgentRotation()
    {
        if (!_savedAgentRotationPreference)
        {
            return;
        }

        EnsureAgent();
        if (agent == null)
        {
            _savedAgentRotationPreference = false;
            return;
        }

        agent.updateRotation = _restoreNavAgentUpdateRotation;
        _savedAgentRotationPreference = false;
    }

    public string GetNpcId()
    {
        string id = GetComponentInChildren<NpcProximityInteractable>(true)?.Definition?.npcId;
        return string.IsNullOrWhiteSpace(id) ? gameObject.name : id;
    }

    private string GetPlateauInstanceId()
    {
        var animal = GetComponentInParent<AnimalInformation>(true);
        if (animal != null) return animal.plateauInstanceId;
        return GetComponentInParent<DetermineModelByAge>(true)?.plateauInstanceId;
    }

    float GetHorizontalDistanceToFollowTarget()
    {
        if (followTarget == null)
        {
            return float.MaxValue;
        }

        Vector3 from = transform.position;
        Vector3 to = followTarget.position;
        from.y = 0f;
        to.y = 0f;
        return Vector3.Distance(from, to);
    }

    void RespawnNearFollowTarget()
    {
        if (followTarget == null)
        {
            return;
        }

        Vector3 spawnPosition = followTarget.position;
        Quaternion spawnRotation = followTarget.rotation;

        Vector3 flatForward = followTarget.forward;
        flatForward.y = 0f;
        if (flatForward.sqrMagnitude > 0.0001f)
        {
            flatForward.Normalize();
            Vector3 behind = followTarget.position - flatForward * FollowSpawnBehindDistance;
            if (NavMesh.SamplePosition(behind, out NavMeshHit behindHit, 5f, NavMesh.AllAreas))
            {
                spawnPosition = behindHit.position;
            }
            else
            {
                spawnPosition = behind;
            }
        }

        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        spawnAnchor = spawnPosition;

        EnsureAgent();
        if (agent == null || !agent.isOnNavMesh)
        {
            nextFollowRefreshAt = 0f;
            return;
        }

        Vector3 warpPosition = spawnPosition;
        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            warpPosition = hit.position;
            spawnAnchor = hit.position;
        }

        agent.Warp(warpPosition);
        agent.ResetPath();
        agent.isStopped = false;
        nextFollowRefreshAt = 0f;
    }

    private bool ShouldBeFollowing()
    {
        if (GameState.Instance?.followingNpcId != GetNpcId()) return false;

        string trackedInstanceId = GameState.Instance?.followingNpcInstanceId;
        if (string.IsNullOrEmpty(trackedInstanceId))
        {
            return true;
        }

        string myPlateauId = GetPlateauInstanceId();
        return !string.IsNullOrEmpty(myPlateauId) && myPlateauId == trackedInstanceId;
    }

    /// <summary>Start following a target transform (e.g. the player).
    /// Any other NPC that is currently following is automatically stopped first.</summary>
    public void Follow(Transform target)
    {
        StopAnyOtherFollowingNpc();

        followTarget = target;
        if (GameState.Instance != null)
        {
            GameState.Instance.followingNpcId = GetNpcId();
            string myPlateauId = GetPlateauInstanceId();
            if (!string.IsNullOrEmpty(myPlateauId))
            {
                GameState.Instance.followingNpcInstanceId = myPlateauId;
            }
        }

        SetState(NpcNavAgentState.Follow);
    }

    /// <summary>Stop following and resume wandering around the current position.</summary>
    public void StopFollowing()
    {
        followTarget = null;

        if (GameState.Instance != null && GameState.Instance.followingNpcId == GetNpcId())
        {
            GameState.Instance.followingNpcId = null;
            GameState.Instance.followingNpcInstanceId = null;
        }

        spawnAnchor = transform.position;
        ScheduleNextWander();
        StopMoving();
        SetState(NpcNavAgentState.Idle);
    }

    private void StopAnyOtherFollowingNpc()
    {
        var all = FindObjectsByType<NPCNavAgent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var other in all)
        {
            if (other != this && other.followTarget != null)
            {
                other.StopFollowing();
            }
        }
    }

    static bool TryComputeHorizontalLookRotation(Vector3 from, Vector3 lookAtPosition, out Quaternion rotation)
    {
        Vector3 d = lookAtPosition - from;
        d.y = 0f;
        if (d.sqrMagnitude < 0.0001f)
        {
            rotation = Quaternion.identity;
            return false;
        }

        rotation = Quaternion.LookRotation(d.normalized, Vector3.up);
        return true;
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
        interactionAimSettled = false;
        lockedInteractionAim = lookAt != null
            && TryComputeHorizontalLookRotation(transform.position, lookAt.position, out Quaternion aim)
            ? aim
            : null;
        if (!lockedInteractionAim.HasValue)
        {
            interactionAimSettled = true;
        }

        SetState(NpcNavAgentState.Interact);
    }

    /// <summary>Stop aiming and resume follow or idle navigation.</summary>
    public void EndInteractionAim()
    {
        if (s_activeInteractionNav == this)
        {
            s_activeInteractionNav = null;
        }

        if (State != NpcNavAgentState.Interact)
        {
            return;
        }

        NpcNavAgentState resumeState = followTarget != null
            ? NpcNavAgentState.Follow
            : NpcNavAgentState.Idle;

        if (resumeState == NpcNavAgentState.Idle)
        {
            nextWanderAt = Time.time + 0.25f;
        }

        SetState(resumeState);
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
