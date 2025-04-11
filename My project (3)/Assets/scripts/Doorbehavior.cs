using UnityEngine;

public class DoorDeletion : MonoBehaviour
{
    public GameObject door; 
    public KeyBehavior keyCollector;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Check if the player has the key
            if (keyCollector != null && keyCollector.HasKey())
            {
                // Deletes the door
                if (door != null)
                {
                    Destroy(door);
                    Debug.Log("Door deleted!");

                    // Trigger the regeneration and fade transition
                    DungeonGenerator dungeonGen = FindObjectOfType<DungeonGenerator>();
                    if (dungeonGen != null)
                    {
                        StartCoroutine(dungeonGen.FadeAndRegenerate());
                    }
                }
                else
                {
                    Debug.LogWarning("No door assigned to delete.");
                }
            }
            else
            {
                Debug.Log("You need a key to open this door!");
            }
        }
    }
}
