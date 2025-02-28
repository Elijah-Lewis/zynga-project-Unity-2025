using System;
using UnityEngine;

public class CollisionBehavior : MonoBehaviour
{
    public float bounceForce = 5f; // Adjust this value to control the bounce strength.
    public float stopThreshold = 0.1f; // Adjust this to control when objects stop.

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing on " + gameObject.name);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb == null) return;

        // Bounce logic
        Vector3 collisionNormal = collision.contacts[0].normal;
        rb.AddForce(collisionNormal * bounceForce, ForceMode.Impulse);

        // Optional: Stop logic (check velocity magnitude)
        CheckForStop();
    }

    void FixedUpdate()
    {
        //Optional: Check for stopping every frame, incase the object slows down without a collision.
        CheckForStop();
    }

    private void CheckForStop()
    {
        if (rb != null && rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}