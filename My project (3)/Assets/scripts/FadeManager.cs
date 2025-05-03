using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance; // Global access
    public Image fadeImage;
    public float fadeSpeed = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Survives scene changes
        }
        else
        {
            Destroy(gameObject); // Prevents duplicates
        }
    }

    // Call this to fade OUT (e.g., at scene start)
    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(1f, 0f)); // Alpha 1 → 0
    }

    // Call this to fade IN (e.g., before loading next scene)
    public void FadeIn()
    {
        StartCoroutine(FadeRoutine(0f, 1f)); // Alpha 0 → 1
    }

    IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        fadeImage.color = new Color(0, 0, 0, startAlpha);
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(startAlpha, targetAlpha, progress));
            yield return null;
        }
    }
}
