using System.Collections;
using UnityEngine;

public class IntroSequenceScript : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private SODialogueSequence introDialogue;
    [SerializeField] private SODialogueSequence directiveDialogue;
    [SerializeField] private SODialogueSequence elegyIntroDialogue;
    [SerializeField] private SODialogueSequence elegyNegotiateDialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        yield return StartCoroutine(dialogueManager.RunDialogueSegment(introDialogue));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(dialogueManager.RunDialogueSegment(directiveDialogue, true));
        yield return StartCoroutine(dialogueManager.RunDialogueSegment(elegyIntroDialogue, true));
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
