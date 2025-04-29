using UnityEngine;
using UnityEngine.AI;

public class EnemyPursuit : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public float chaseRange = 1000f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 40f; // Set chase speed
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= chaseRange)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.ResetPath(); // Stops moving if player too far
            }
        }
    }
}
