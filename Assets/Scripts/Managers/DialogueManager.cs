using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private BasicTerminalUIManager terminalUIManager;
    private string inputSubmitted = string.Empty;
    private bool inputCorrect = false;
    private string currentCorrectInput;
    private int currentWrongInputResponseIndex = 0;

    [SerializeField] private SODialogueSequence introDialogue;
    [SerializeField] private SODialogueSequence directiveDialogue;
    [SerializeField] private SODialogueSequence elegyIntroDialogue;
    [SerializeField] private SODialogueSequence elegyNegotiateDialogue;

    [SerializeField] GameObject commandLineContainer;
    [SerializeField] private GameObject systemMessagePrefab;
    [SerializeField] private GameObject userInputPrefab;
    [SerializeField] private ScrollRect scrollRect;


    private void Start()
    {
        if (!terminalUIManager)
            Debug.LogError("Terminal UI Manager not found!");
        StartCoroutine(RunDialogueSegment(introDialogue));
    }


    public IEnumerator RunDialogueSegment(SODialogueSequence dialogue)
    {
        for (int i = 0; i < dialogue.entries.Count; i++)
        {
            DialogueEntry entry = dialogue.entries[i];
            yield return new WaitForSeconds(entry.delayBefore);
            string textToAdd = string.Empty;
            //float waitTime = 0f;
            switch (entry.type)
            {
                case DialogueEntryType.UserMessage:
                    textToAdd = entry.sender + ": " + entry.message;
                    terminalUIManager.Print(textToAdd, "terminal-line-user");
                    break;
                case DialogueEntryType.SystemMessage:
                    textToAdd = entry.message;
                    terminalUIManager.Print(textToAdd, "terminal-line-system");
                    break;
                case DialogueEntryType.InputPrompt:
                    terminalUIManager.ShowInputField();
                    currentCorrectInput = entry.inputPrompt.correctInput;
                    yield return new WaitUntil(() => inputSubmitted != string.Empty);
                    if (inputSubmitted != currentCorrectInput) //Incorrect Input
                    {
                        i--;
                        textToAdd = entry.inputPrompt.wrongInputResponses[currentWrongInputResponseIndex];
                        if (currentWrongInputResponseIndex < entry.inputPrompt.wrongInputResponses.Count)
                            currentWrongInputResponseIndex++;
                    }
                    else //Correct Input
                    {
                        terminalUIManager.HideInputField();
                        currentCorrectInput = string.Empty;
                        inputCorrect = false;
                        currentWrongInputResponseIndex = 0;
                        textToAdd = terminalUIManager.GetInputFieldText();
                    }
                    terminalUIManager.Print(textToAdd, "terminal-line-user");
                    break;
            }

            

            //yield return StartCoroutine(PrintToTerminal(textToAdd, waitTime));

            if (entry.dialogueManagerFunction != string.Empty)
            {
                Invoke(entry.dialogueManagerFunction, 0f);
            } 
        }
    }

    private void Function()
    { 
        
    }


}
