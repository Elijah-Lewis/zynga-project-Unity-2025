using UnityEngine;

public class DoorDeletion : MonoBehaviour
{
    public GameObject door;
    public KeyBehavior keyCollector;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (keyCollector != null && keyCollector.HasKey())
            {
                if (door != null)
                {
                    Destroy(door);
                    Debug.Log("Door deleted! Progressing to the next gameplay scene.");

                    // Find the SceneCycleManager and tell it to advance
                    SceneCycleManager sceneCycleManager = FindObjectOfType<SceneCycleManager>();
                    if (sceneCycleManager != null)
                    {
                        sceneCycleManager.LoadNextScene();
                    }
                    else
                    {
                        Debug.LogError("SceneCycleManager not found in the scene!");
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