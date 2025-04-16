using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public GameObject projectilePrefab;
    public Transform shootPoint; // Where the projectile will spawn from
    public float sightRange = 10f;
    public float stoppingDistance = 0.5f;
    public float shootRange = 5f; // Distance at which the enemy will shoot
    public float shootInterval = 2f; // Time between shots
    public EnemyHealth enemyHealth;

   
    private float lastShootTime;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        // Ensure stopping distance is reasonable
        agent.stoppingDistance = stoppingDistance;
        //hp 
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            enemyHealth = gameObject.AddComponent<EnemyHealth>();
        }
    }

    private void Update()
    {
        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only chase if the player is within sight range
        if (distanceToPlayer <= sightRange)
        {
            // Update the path every frame to keep tracking
            if (IsPathValid(player.position))
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                Debug.Log("Tracking player...");

                // If the agent stops moving unexpectedly, force move
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.ResetPath(); // Refresh path calculation
                    agent.SetDestination(player.position);
                }

                // Shoot if the player is within shooting range and enough time has passed
                if (distanceToPlayer <= shootRange && Time.time - lastShootTime >= shootInterval)
                {
                    ShootProjectile();
                    lastShootTime = Time.time; // Update last shoot time
                }
            }
            else
            {
                Debug.LogError("Path to player is NOT valid! Is the player on the NavMesh?");
            }
        }
        else
        {
            agent.isStopped = true;
        }

        DebugAgentStatus();
    }

    private bool IsPathValid(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPosition, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    private void ShootProjectile()
    {
        // Instantiate the projectile at the shoot point
        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            // Get the direction to the player and apply it to the projectile
            Vector3 direction = (player.position - shootPoint.position).normalized;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * 10f; // Set the projectile speed (10f is just an example)
            }

            Debug.Log("Projectile shot at player!");
        }
        else
        {
            Debug.LogWarning("Projectile Prefab or Shoot Point is missing!");
        }
    }

    private void DebugAgentStatus()
    {
        Debug.Log($"Agent Status: {agent.pathStatus}, Speed: {agent.speed}, " +
                  $"Velocity: {agent.velocity}, Remaining Distance: {agent.remainingDistance}");
    }


}