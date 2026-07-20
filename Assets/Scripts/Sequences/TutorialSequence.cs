using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSequence : MonoBehaviour
{
    ReferenceManager referenceManager;
    TerminalUIManager terminalUIManager;
    public int currentTutorialIndex = 0;
    public Entity tutorialOpponent;
    [TextArea]
    [SerializeField] string[] tutorialMessages;

    //Stupid stuff
    private bool hasRanSuspension = false;
    private bool hasRanKill = false;
    void Start()
    {
        referenceManager = ReferenceManager.Instance;
        terminalUIManager = referenceManager.terminalUIManager;
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
        if (currentTutorialIndex == 10)
        {
            string[] dummyProcess = { "dummy" };
            referenceManager.processManager.TryRunProcess(dummyProcess, tutorialOpponent, referenceManager.serverProcessQueue, true);
            referenceManager.processManager.TryRunProcess(dummyProcess, tutorialOpponent, referenceManager.serverProcessQueue, true);
        }

        if (currentTutorialIndex < tutorialMessages.Length)
        {
            terminalUIManager.Print(tutorialMessages[currentTutorialIndex]);
            currentTutorialIndex++;
        }
        else
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }
    }

    private void TutorialPlayerSubmitted(string input)
    {
        Debug.Log(input + " at index: " + currentTutorialIndex);
        if (input == "run scry" && currentTutorialIndex == 5) //This is +1 the current index because i hate meowself
        {
            Invoke(nameof(PrintTutorialMessage), 3f);
        }
        if (input.Contains("suspend") && currentTutorialIndex == 11 && !hasRanSuspension)
        {
            hasRanSuspension = true;
        }
        if (input.Contains("kill") && currentTutorialIndex == 11 && !hasRanKill)
        {
            hasRanKill = true;
        }
        if (currentTutorialIndex == 11 && hasRanKill && hasRanSuspension)
        {
            Invoke(nameof(PrintTutorialMessage), 10f);
        }
    }

}
