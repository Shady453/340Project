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

        // Make sure agent is configured for 2D top-down
        agent.updateRotation = false;
        agent.updateUpAxis   = false;
    }

    private void Start()
    {
        // You can assign the player in the inspector instead if you prefer.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("ZombieAiMove: No GameObject with tag 'Player' found in scene.");
        }
    }

    private void Update()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            // Chase the player
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            // Reached attack range, stop moving
            agent.isStopped = true;
        }
    }
}