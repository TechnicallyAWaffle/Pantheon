using System;
using UnityEngine;

public class TutorialSequence : MonoBehaviour
{
    TerminalUIManager terminalUIManager;
    public int currentTutorialIndex = 0;
    [TextArea]
    [SerializeField] string[] tutorialMessages;
    void Start()
    {
        terminalUIManager = ReferenceManager.Instance.terminalUIManager;
        TutorialIntroMessage();
        GlobalEventBus.OnTutorialSubmit += TutorialPlayerSubmitted;
    }

    private void TutorialIntroMessage()
    {
        terminalUIManager.Print("Welcome to Uploaded Intelligence combat training module version 12. " +
            "Terminal format workspaces are being deprecated in the upcoming EXODIA 6 updates. " +
            "Please switch to integrated 3D environment workspaces as soon as possible.");
        terminalUIManager.Print("Input \" cd next\" during calibration to advance to the next portion");
    }

    public void PrintTutorialMessage()
    {
        if (currentTutorialIndex < tutorialMessages.Length)
        {
            terminalUIManager.Print(tutorialMessages[currentTutorialIndex]);
            currentTutorialIndex++;
        }
    }

    private void TutorialPlayerSubmitted(string input)
    {
        Debug.Log(input + " at index: " + currentTutorialIndex);
        if (input == "run scry" && currentTutorialIndex == 5) //This is +1 the current index because i hate meowself
        {
            Debug.Log("YAY");
            Invoke(nameof(PrintTutorialMessage), 3f);
        }
    }

}
