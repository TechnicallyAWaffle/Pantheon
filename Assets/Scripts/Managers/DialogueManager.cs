using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEngine.EventSystems.EventTrigger;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI commandLineOutput;
    public TMP_InputField inputField;
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
        inputField.onSubmit.AddListener(OnSubmit);
        StartCoroutine(RunDialogueSegment(introDialogue));
    }

    private void OnSubmit(string input)
    {
        inputSubmitted = input;
    }



    private IEnumerator PrintToTerminal(string[] input, float waitTime)
    {
        Debug.Log(input[0] + input[1]);
        GameObject systemMessage = Instantiate(systemMessagePrefab, commandLineContainer.transform);
        TextMeshProUGUI text = systemMessage.GetComponentInChildren<TextMeshProUGUI>();
        text.text = input[0];

        if (input[1] != string.Empty)
        {
            yield return new WaitForSeconds(waitTime);
            text.text += input[1];
        }

        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }

public IEnumerator RunDialogueSegment(SODialogueSequence dialogue)
    {
        for (int i = 0; i < dialogue.entries.Count; i++)
        {
            DialogueEntry entry = dialogue.entries[i];
            yield return new WaitForSeconds(entry.delayBefore);
            string[] textToAdd = {string.Empty, string.Empty };
            float waitTime = 0f;
            switch (entry.type)
            {
                case DialogueEntryType.UserMessage:
                    textToAdd[0] = entry.sender + ": " + entry.message;
                    break;
                case DialogueEntryType.SystemMessage:
                    textToAdd[0] = entry.message;
                    break;
                case DialogueEntryType.InputPrompt:
                    Instantiate(userInputPrefab, commandLineContainer.transform);
                    inputField.ActivateInputField();
                    currentCorrectInput = entry.inputPrompt.correctInput;
                    yield return new WaitUntil(() => inputSubmitted != string.Empty);
                    if (inputSubmitted != currentCorrectInput)
                    {
                        i--;
                        textToAdd[0] = entry.inputPrompt.wrongInputResponses[currentWrongInputResponseIndex];
                        if (currentWrongInputResponseIndex < entry.inputPrompt.wrongInputResponses.Count)
                            currentWrongInputResponseIndex++;
                    }
                    else
                    { 
                        currentCorrectInput = string.Empty;
                        inputCorrect = false;
                        currentWrongInputResponseIndex = 0;
                    }
                        break;
            }

            yield return StartCoroutine(PrintToTerminal(textToAdd, waitTime));

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
