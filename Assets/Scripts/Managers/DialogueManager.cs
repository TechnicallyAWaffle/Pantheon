using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private BasicTerminalUIManager terminalUIManager;
    [HideInInspector] public string inputSubmitted = string.Empty;
    private bool inputCorrect = false;
    private string currentCorrectInput;
    private int currentWrongInputResponseIndex = 0;


    private void Start()
    {
        terminalUIManager = GetComponent<BasicTerminalUIManager>();
    }


    public IEnumerator RunDialogueSegment(SODialogueSequence dialogue, bool clearBefore = false)
    {
        if(clearBefore)
            terminalUIManager.Clear();

        for (int i = 0; i < dialogue.entries.Count; i++)
        {
            DialogueEntry entry = dialogue.entries[i];
            yield return new WaitForSeconds(entry.delayBefore);
            string textToAdd = string.Empty;
            //float waitTime = 0f;
            if (entry.dialogueManagerFunction != string.Empty)
            {
                Type type = this.GetType();
                if (type.GetMethod(entry.dialogueManagerFunction) != null)
                    Invoke(entry.dialogueManagerFunction, 0f);
                else
                    Debug.LogError("Function: " + entry.dialogueManagerFunction + " does not exist in DialogueManager!");
            }

            switch (entry.type)
            {
                case DialogueEntryType.UserMessage:
                    textToAdd = entry.sender + ": " + entry.message;
                    terminalUIManager.Print(textToAdd, "terminal-line-user");
                    break;
                case DialogueEntryType.SystemMessage:
                    float delay = ExtractDelay(entry.message, out string beforeDelayOutput, out string afterDelayOuput);
                    if (delay == 0f)
                    {
                        terminalUIManager.Print(entry.message, "terminal-line-system");
                        break;
                    }
                    terminalUIManager.Print(beforeDelayOutput, "terminal-line-system");
                    yield return new WaitForSeconds(delay);
                    terminalUIManager.Print(afterDelayOuput, "terminal-line-system", true);
                    break;
                case DialogueEntryType.InputPrompt:
                    terminalUIManager.ShowInputField();
                    currentCorrectInput = entry.inputPrompt.correctInput;
                    yield return new WaitUntil(() => inputSubmitted != string.Empty);
                    if (inputSubmitted != currentCorrectInput) //Incorrect Input
                    {
                        inputSubmitted = string.Empty;
                        i--;
                        textToAdd = entry.inputPrompt.wrongInputResponses[currentWrongInputResponseIndex];
                        if (currentWrongInputResponseIndex < entry.inputPrompt.wrongInputResponses.Count - 1)
                            currentWrongInputResponseIndex++;
                    }
                    else //Correct Input
                    {
                        terminalUIManager.HideInputField();
                        inputSubmitted = string.Empty;
                        currentCorrectInput = string.Empty;
                        inputCorrect = false;
                        currentWrongInputResponseIndex = 0;
                        textToAdd = terminalUIManager.GetInputFieldText();
                    }
                    terminalUIManager.Print(textToAdd, "terminal-line-user");
                    break;
            }

            if (entry.dialogueManagerFunction != string.Empty)
            {
                Invoke(entry.dialogueManagerFunction, 0f);
            } 
        }
    }

    private static float ExtractDelay(string input, out string before, out string after)
    {
        var match = Regex.Match(input, @"<([\d.]+)>");

        if (!match.Success)
        {
            before = input;
            after = string.Empty;
            return 0f;
        }

        float delay = float.Parse(match.Groups[1].Value);
        before = input[..match.Index];
        after = input[(match.Index + match.Length)..];

        return delay;
    }

    private void Function()
    { 
        
    }


}
