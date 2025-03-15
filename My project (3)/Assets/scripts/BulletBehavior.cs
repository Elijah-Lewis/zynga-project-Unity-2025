using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float maxDistance = 100f; // Maximum distance the bullet can travel
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate distance traveled
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);

        // Destroy the bullet if it exceeds the maximum distance
        if (distanceTraveled > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet on collision
        Destroy(gameObject);
    }
}