using UnityEngine;

public class KeyBehavior : MonoBehaviour
{
    private bool hasKey = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with the key
        if (other.CompareTag("Key"))
        {
            // Collect the key
            hasKey = true;
            Destroy(other.gameObject); // Destroys the key
            Debug.Log("Key collected!");
        }
    }

    public bool HasKey()
    {
        return hasKey;
    }
}