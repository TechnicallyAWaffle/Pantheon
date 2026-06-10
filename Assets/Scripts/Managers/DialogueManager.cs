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
                    //open input prompt
                    

                    //yield return WaitUntil();

                    break;

            }

            PrintToTerminal(textToAdd);

            if (entry.dialogueManagerFunction != string.Empty)
            {

            }
        }

        
    }

}
