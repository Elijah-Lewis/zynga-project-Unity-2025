using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public string sceneToLoad = "SampleScene";

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detects left mouse click
        {
            Debug.Log("Screen clicked. Cleaning up title screen and loading scene...");
            CleanupTitleScreen();
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    void CleanupTitleScreen()
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            Destroy(obj);
        }
    }
}