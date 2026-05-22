using UnityEngine;

public class ReferenceManager : MonoBehaviour
{

    [SerializeField] CommandManager commandManager;
    [SerializeField] TerminalUIManager terminalUIManager;

    public static ReferenceManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
