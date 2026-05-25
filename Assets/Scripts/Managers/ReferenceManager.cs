using UnityEngine;

public class ReferenceManager : MonoBehaviour
{

    [SerializeField] public CommandManager commandManager;
    [SerializeField] public TerminalUIManager terminalUIManager;
    [SerializeField] public ProcessManager processManager;
    [SerializeField] public Entity player;

    public static ReferenceManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
