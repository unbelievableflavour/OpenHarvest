using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Shared movement/AI base for farm animals. Drives a <see cref="NavMeshAgent"/> to either
/// wander randomly around a spawn anchor or follow a target transform (e.g. the player).
///
/// Requires a baked NavMesh in the scene. The agent is auto-added via
/// <see cref="RequireComponent"/>, so existing prefabs only need to be re-saved once the
/// NavMesh is baked for the farm terrain.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class AnimalNavAgent : MonoBehaviour
{
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
    [Tooltip("If set, the animal will follow this transform instead of wandering.")]
    public Transform followTarget;
    [Tooltip("How close to the follow target before stopping.")]
    public float followStopDistance = 2f;
    [Tooltip("How often (seconds) the follow destination is refreshed.")]
    public float followRefreshInterval = 0.25f;

    [Header("Activity Culling")]
    [Tooltip("Only move when the main camera is further away than this distance. Set to 0 to disable culling.")]
    public float movesWhenFurtherAwayThen = 5f;

    NavMeshAgent agent;
    Transform mainCam;
    Vector3 spawnAnchor;
    float nextWanderAt;
    float nextFollowRefreshAt;
    bool isActive = true;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // NavMeshAgent writes to transform directly; any attached Rigidbody must be kinematic
        // to avoid fighting the agent's position updates.
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
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
        UpdateActiveState();
        if (!isActive)
        {
            return;
        }

        if (followTarget != null)
        {
            TickFollow();
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
        nextFollowRefreshAt = Time.time + followRefreshInterval;

        float distanceToTarget = Vector3.Distance(transform.position, followTarget.position);
        if (distanceToTarget <= followStopDistance)
        {
            StopMoving();
            return;
        }

        StartMoving();
        agent.stoppingDistance = followStopDistance;
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
    }

    /// <summary>Stop following and resume wandering around the current position.</summary>
    public void StopFollowing()
    {
        followTarget = null;
        spawnAnchor = transform.position;
        ScheduleNextWander();
        StopMoving();
    }
}
