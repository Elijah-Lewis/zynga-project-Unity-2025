using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneCycleManager : MonoBehaviour
{
    public List<string> sceneOrder = new List<string>() { "title scene", "SampleScene", "Story1", "SampleScene" };
    private int currentSceneIndex = 0;
    private bool canAdvance = true; // To prevent accidental rapid scene changes

    void Start()
    {
        // Ensure the list has scenes
        if (sceneOrder.Count == 0)
        {
            Debug.LogError("Scene Order list is empty! Please add scene names in the Inspector.");
            enabled = false; // Disable the script to prevent errors
            return;
        }

        // Load the first scene in the order
        SceneManager.LoadScene(sceneOrder[currentSceneIndex]);
    }

    void Update()
    {
        // Check for input to advance to the next scene
        if (canAdvance && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Screen clicked. Loading next scene...");
            LoadNextScene();
            canAdvance = false; // Prevent immediate further advancement
        }

        // You might want to reset the 'canAdvance' flag after a short delay
        // or based on some other game state to allow the next click to register.
        // For a simple implementation, you can just re-enable it in LoadNextScene.
    }

    public void LoadNextScene()
    {
        currentSceneIndex++;

        // Loop back to the beginning if we've reached the end of the list
        if (currentSceneIndex >= sceneOrder.Count)
        {
            Debug.Log("Reached the end of the scene order. Looping back to the start.");
            currentSceneIndex = 0;
        }

        SceneManager.LoadScene(sceneOrder[currentSceneIndex]);

        // Re-enable advancement for the next click
        canAdvance = true;
    }
}