using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class DialogueManager : MonoBehaviour
{

    public TextMeshProUGUI commandLineOutput;
    public InputField inputField;
    private string inputSubmitted = string.Empty;
    private bool inputCorrect = false;
    private string currentCorrectInput;
    private int currentWrongInputResponseIndex = 0;


    private void Start()
    {
        inputField.onSubmit.AddListener(OnSubmit);
    }

    private void OnSubmit(string input)
    {
        inputSubmitted = input;
    }



    private void PrintToTerminal(string input)
    {
        commandLineOutput.text += input;
    }


    public IEnumerator RunDialogueSegment(SODialogueSequence dialogue)
    {

        for (int i = 0; i < dialogue.entries.Count; i++)
        {
            DialogueEntry entry = dialogue.entries[i];
            yield return new WaitForSeconds(entry.delayBefore);
            string textToAdd = string.Empty;
            switch (entry.type)
            {
                case DialogueEntryType.UserMessage:
                    textToAdd = entry.sender + ": " + entry.message;
                    break;
                case DialogueEntryType.SystemMessage:
                    var match = Regex.Match(entry.message, @"<([\d.]+)>");
                    if (match.Success)
                        yield return new WaitForSeconds(float.Parse(match.Groups[1].Value));
                    textToAdd = entry.message;
                    break;
                case DialogueEntryType.InputPrompt:
                    inputField.ActivateInputField();
                    currentCorrectInput = entry.inputPrompt.correctInput;
                    yield return new WaitUntil(() => inputSubmitted != string.Empty);
                    if (inputSubmitted != currentCorrectInput)
                    {
                        i--;
                        textToAdd = entry.inputPrompt.wrongInputResponses[currentWrongInputResponseIndex];
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

            PrintToTerminal(textToAdd);

            if (entry.dialogueManagerFunction != string.Empty)
            {
                Invoke(entry.dialogueManagerFunction, 0f);
            } 
        }

        
    }

}
