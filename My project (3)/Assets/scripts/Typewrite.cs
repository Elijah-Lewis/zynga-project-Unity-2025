using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Typewrite : MonoBehaviour
{
    [Header("Typing Settings")]
    [SerializeField] private float delay = 0.1f;
    [SerializeField] private List<string> dialogueLines = new List<string>();
    [SerializeField] private TMP_Text textComponent;

    [Header("UI Elements")]
    [SerializeField] private GameObject continuePrompt; // "Press Enter" prompt
    [SerializeField] private Button endButton; // Button that appears when dialogue finishes
    [SerializeField] private string buttonText = "Continue";

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        // Initialize components
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        if (continuePrompt != null)
            continuePrompt.SetActive(false);

        if (endButton != null)
        {
            endButton.gameObject.SetActive(false);
            // You can customize the button text if you want
            if (endButton.GetComponentInChildren<TMP_Text>() != null)
                endButton.GetComponentInChildren<TMP_Text>().text = buttonText;
        }

        StartTyping(dialogueLines[currentLineIndex]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                NextLine();
            }
        }
    }

    IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        textComponent.text = "";

        for (int i = 0; i <= textToType.Length; i++)
        {
            textComponent.text = textToType.Substring(0, i);
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;

        // Show "press enter" prompt if there are more lines
        if (currentLineIndex < dialogueLines.Count - 1)
        {
            if (continuePrompt != null)
                continuePrompt.SetActive(true);
        }
        else
        {
            // All dialogue is finished - show the button
            if (endButton != null)
                endButton.gameObject.SetActive(true);
        }
    }

    public void StartTyping(string newText)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Hide button when new typing starts
        if (endButton != null)
            endButton.gameObject.SetActive(false);

        typingCoroutine = StartCoroutine(TypeText(newText));
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textComponent.text = dialogueLines[currentLineIndex];
        isTyping = false;

        if (continuePrompt != null)
            continuePrompt.SetActive(currentLineIndex < dialogueLines.Count - 1);

        // If this was the last line, show button
        if (currentLineIndex >= dialogueLines.Count - 1 && endButton != null)
            endButton.gameObject.SetActive(true);
    }

    private void NextLine()
    {
        if (continuePrompt != null)
            continuePrompt.SetActive(false);

        if (currentLineIndex < dialogueLines.Count - 1)
        {
            currentLineIndex++;
            StartTyping(dialogueLines[currentLineIndex]);
        }
    }

    // Call this to set a new sequence of dialogue lines
    public void SetDialogueLines(List<string> newLines)
    {
        dialogueLines = newLines;
        currentLineIndex = 0;

        // Hide button when starting new dialogue
        if (endButton != null)
            endButton.gameObject.SetActive(false);

        StartTyping(dialogueLines[currentLineIndex]);
    }

    // Call this method from the button's OnClick event
    public void OnEndButtonClicked()
    {
        Debug.Log("Dialogue finished - button clicked!");
        // Add your custom behavior here, like:
        // - Closing the dialogue box
        // - Loading a new scene
        // - Triggering an event
    }
}