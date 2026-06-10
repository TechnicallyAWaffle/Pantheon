using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue_New", menuName = "Terminal/Dialogue Sequence")]
public class SODialogueSequence : ScriptableObject
{
    public List<DialogueEntry> entries = new();
}

[System.Serializable]
public class DialogueEntry
{
    public DialogueEntryType type;

    [Tooltip("Sender name")]
    public string sender;

    [TextArea(2, 6)]
    public string message;

    public float delayBefore = 0f;      // seconds to wait before this entry plays

    public string dialogueManagerFunction; // name of a method to call on DialogueManager

    // only used when type == InputPrompt
    public InputPromptData inputPrompt;
}

public enum DialogueEntryType
{
    UserMessage,        // sender + short message
    SystemMessage,      // formatted, no sender
    InputPrompt,        // pauses and waits for player input
    FunctionCall,       // fires a DialogueManager method, no visible message
}

[System.Serializable]
public class InputPromptData
{
    [Tooltip("The exact string the player must type to continue")]
    public string correctInput;

    [Tooltip("Shown in the terminal to tell the player what to type")]
    public string promptHint;

    [Tooltip("Responses shown when the player types something wrong")]
    public List<string> wrongInputResponses = new();
}