using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public EnemyHealth enemyHealth;

    [Header("Chase Settings")]
    public float sightRange = 15f;
    public float stoppingDistance = 1f;
    public float moveSpeed = 5f;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootRange = 7f;
    public float shootInterval = 2f;

    private float lastShootTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found on Enemy!");
            return;
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("Player not found! Tag your player as 'Player'.");

        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth == null)
            enemyHealth = gameObject.AddComponent<EnemyHealth>();

        agent.stoppingDistance = stoppingDistance;
        agent.speed = moveSpeed;

        // Ensure shootPoint is found on instantiated enemy
        if (shootPoint == null)
        {
            shootPoint = transform.Find("ShootPoint");
            if (shootPoint == null)
                Debug.LogWarning("ShootPoint not assigned and not found in children!");
        }
    }

    private void Update()
    {
        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= sightRange)
        {
            ChasePlayer();

            if (distanceToPlayer <= shootRange && Time.time >= lastShootTime + shootInterval)
            {
                ShootProjectile();
                lastShootTime = Time.time;
            }
        }
        else
        {
            agent.ResetPath(); // Stop moving if player is too far
        }

        DebugDrawPath();
    }

    private void ChasePlayer()
    {
        if (IsPathValid(player.position))
        {
            agent.SetDestination(player.position);
        }
        else
        {
            // Instead of stopping immediately, just don't set a new destination
            Debug.LogWarning("Invalid path to player, keeping current position.");
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null || shootPoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (player.position - shootPoint.position).normalized;
            rb.linearVelocity = direction * 10f; // Fixed from linearVelocity to velocity
        }

        Debug.Log("Enemy shot at player!");
    }

    private bool IsPathValid(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPosition, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    private void DebugDrawPath()
    {
        if (agent.path != null && agent.path.corners.Length > 1)
        {
            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.red);
            }
        }
    }
}
