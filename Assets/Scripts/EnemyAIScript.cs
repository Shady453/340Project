using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAIScript : MonoBehaviour
{
    [Tooltip("How close to the player before we stop moving (e.g., for attack).")]
    public float stopDistance = 0.5f;

    private NavMeshAgent agent;
    private Transform target;  // Player

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Top-down unity navmesh 2D requirements
        agent.updateRotation = false;
        agent.updateUpAxis   = false;

        // --- CRITICAL MULTI-ZOMBIE SETTINGS ---
        agent.radius = 0.1f;                         // default is ~0.5 → way too large for your game
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.avoidancePriority = 50;                 // all zombies equal
        agent.autoBraking = false;                    // smoother approach
        agent.stoppingDistance = stopDistance;
        // --------------------------------------
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("EnemyAIScript: No GameObject with tag 'Player' found!");
        }
    }

    private void Update()
    {
        if (target == null || agent == null || !agent.isOnNavMesh)
            return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            agent.isStopped = false;

            // Clamp destination to nearest navmesh point
            Vector3 desiredPos = target.position;
            if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                // If player is off-mesh, we stop moving until they come back within range
                agent.isStopped = true;
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    // Expose navmesh speed cleanly
    public float speed
    {
        get => agent != null ? agent.speed : 0f;
        set
        {
            if (agent != null)
                agent.speed = value;
        }
    }
}
