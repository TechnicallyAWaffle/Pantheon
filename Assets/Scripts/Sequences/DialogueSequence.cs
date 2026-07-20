using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class DialogueSequence : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] SODialogueSequence[] sequences;

    void Start()
    {
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        foreach(SODialogueSequence sequence in sequences)
            yield return StartCoroutine(dialogueManager.RunDialogueSegment(sequence, sequence.clearBeforeRunning));
    }
}
