using UnityEngine;

public class ReferenceManager : MonoBehaviour
{

    [SerializeField] public CommandManager commandManager;
    [SerializeField] public TerminalUIManager terminalUIManager;
    [SerializeField] public ProcessManager processManager;
    [SerializeField] public QueueManager queueManager;
    [SerializeField] public DaemonManager daemonManager;
    [SerializeField] public SuspensionManager suspensionManager;
    [SerializeField] public DialogueManager dialogueManager;
    [SerializeField] public Entity player;
    [SerializeField] public Entity opponent;
    public ProcessQueue serverProcessQueue;

    public static ReferenceManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
