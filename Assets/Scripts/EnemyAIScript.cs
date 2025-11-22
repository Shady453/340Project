using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAIScript : MonoBehaviour
{
    [Tooltip("How close to the target before we stop moving (for attack).")]
    public float stopDistance = 0.5f;

    [Header("Attack")]
    public float attackInterval = 1.0f;
    public float barricadeDamage = 10f;

    private NavMeshAgent agent;
    private Transform playerTarget;
    private Barricade currentBarricade;

    private float attackTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Top-down 2D settings
        agent.updateRotation = false;
        agent.updateUpAxis   = false;

        agent.radius = 0.1f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.stoppingDistance = stopDistance;
        agent.autoBraking = false;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
        }
        else
        {
            Debug.LogWarning("EnemyAIScript: No GameObject with tag 'Player' found in scene.");
        }
    }

    private void Update()
    {
        if (!RoundSystem.IsGameActive || agent == null || !agent.isOnNavMesh)
        {
            if (agent != null) agent.isStopped = true;
            return;
        }

        // If we don't have a barricade, or it's broken, try to find the closest one
        if (currentBarricade == null || currentBarricade.IsBroken)
        {
            currentBarricade = FindClosestBarricade();
        }

        // Decide what to chase:
        //  - if we have a live barricade: chase that
        //  - otherwise: chase the player
        Transform target = null;

        if (currentBarricade != null && !currentBarricade.IsBroken)
        {
            target = currentBarricade.transform;
        }
        else
        {
            target = playerTarget;
        }

        if (target == null)
        {
            agent.isStopped = true;
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            // Move toward target
            agent.isStopped = false;

            Vector3 desired = target.position;
            if (NavMesh.SamplePosition(desired, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            // Not in range to attack
            attackTimer = attackInterval;
        }
        else
        {
            // In attack range
            agent.isStopped = true;

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                attackTimer = attackInterval;

                if (currentBarricade != null && !currentBarricade.IsBroken)
                {
                    // Attack barricade only
                    currentBarricade.TakeDamage(barricadeDamage);
                }
                else
                {
                    // TODO: attack the player when you add a player health script
                    // e.g. playerTarget.GetComponent<PlayerHealth>()?.TakeDamage(x);
                }
            }
        }
    }

    // Pick the closest unbroken barricade from Barricade.All
    private Barricade FindClosestBarricade()
    {
        Barricade closest = null;
        float closestDistSq = float.MaxValue;

        foreach (var b in Barricade.All)
        {
            if (b == null || b.IsBroken) continue;

            float dSq = (b.transform.position - transform.position).sqrMagnitude;
            if (dSq < closestDistSq)
            {
                closestDistSq = dSq;
                closest = b;
            }
        }

        return closest;
    }

    // Expose navmesh speed if you want to scale it per round
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

