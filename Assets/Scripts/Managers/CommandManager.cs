using System;
using System.Buffers;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CommandManager : MonoBehaviour
{
    //References
    [SerializeField] private TMP_InputField inputField;
    private ReferenceManager referenceManager;
    private TerminalUIManager terminalUIManager;
    private ProcessManager processManager;
    private QueueManager queueManager;
    private Entity player;

    //Runtime Vars
    SOCommand lastInputtedCommand;


    //DEV DEBUG 
    private void Update()
    {
        //Keep focus on input field for now
        if (Input.GetMouseButtonDown(0))
        {
            inputField.ActivateInputField();
        }
    }

    void Start()
    {
        RegisterCommands();
        inputField.onSubmit.AddListener(OnSubmit);
        inputField.text = "meowemeowmeoemwe";
        inputField.ActivateInputField();

        referenceManager = ReferenceManager.Instance;
        terminalUIManager = referenceManager.terminalUIManager;
        processManager = referenceManager.processManager;
        player = referenceManager.player;
        queueManager = referenceManager.queueManager;

    }

    [SerializeField] private SOCommand[] baseCommandData;
    private Dictionary<string, (SOCommand data, Action<string[]> handler)> commands;

    private Dictionary<string, Action<string[]>> BuildHandlerMap()
    {
        return
            new Dictionary<string, Action<string[]>>
            {
                { "run",    args => CmdRun(args)    },
                { "srun",    args => CmdRunServer(args)    },
                { "suspend",   args => CmdSuspend(args)   },
                { "kill", args => CmdKill(args) },
                { "help",   args => CmdHelp(args)   },
                { "overclock",   args => CmdOverclock(args)   },
            };
    }

    public void RegisterCommands()
    {
        var handlerMap = BuildHandlerMap();

        commands = new Dictionary<string, (SOCommand, Action<string[]>)>();

        foreach (var so in baseCommandData)
        {
            if (handlerMap.TryGetValue(so.commandName, out var handler))
            {
                commands[so.commandName] = (so, handler);
            }
            else
            {
                Debug.LogWarning($"CommandDataSO '{so.commandName}' has no matching handler.");
            }
        }

        // Check in reverse just to make sure we didn't add anything in the wrong order
        foreach (var key in handlerMap.Keys)
        {
            if (!commands.ContainsKey(key))
                Debug.LogWarning($"Handler '{key}' has no matching CommandDataSO.");
        }
    }

    void OnSubmit(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        terminalUIManager.Print("> " + input);
        ParseInput(input.Trim());

        inputField.text = "";
        inputField.ActivateInputField(); // keep focus after submitting
    }

    SOCommand GetCommandDataByString(string commandName)
    {
        if (commands.TryGetValue(commandName, out var entry))
        {
            return entry.data;
        }
        else
        {
            Debug.Log("Could not find command with name: " + commandName);
            return null;
        }
    }

    void ParseInput(string input)
    {
        string[] tokens = input.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length < 1)
            return;
        string verb = tokens[0].ToLower();
        string[] args = tokens.Length > 1
            ? tokens[1..]
            : new string[0];


        if (commands.TryGetValue(verb, out var entry))
        {
            lastInputtedCommand = entry.data;
            entry.handler(args);
        }
        else
            terminalUIManager.Print($"Unknown command: '{verb}'. Type 'help' for a list.");
    }

    // --- Command Handlers ---

    void CmdRun(string[] args)
    {
        if (args.Length == 0) { terminalUIManager.Print("Usage: run <program> <-arguments>"); return; }

        processManager.TryRunProcess(args, player, queueManager.playerLocalQueue, false);

        string programName = args[0];

        terminalUIManager.Print($"");
        terminalUIManager.Print($"Process '{programName}' added to LOCALHOST scheduler queue");   
    }

    void CmdRunServer(string[] args)
    {
        if (args.Length == 0) { terminalUIManager.Print("Usage: srun <program> <-arguments>"); return; }

        processManager.TryRunProcess(args, player, queueManager.serverQueue, true);

        string programName = args[0];

        terminalUIManager.Print($"");
        terminalUIManager.Print($"Process '{programName}' added to SERVER CONNECTION scheduler queue");
    }


    void CmdSuspend(string[] args)
    {
        if (args.Length == 0) { terminalUIManager.Print("Usage: stop <program>"); return; }

    }

    void CmdKill(string[] args) { /* ... */ }

    void CmdHelp(string[] args) 
    {
        if (args.Length == 0) { terminalUIManager.Print("Usage: help <process>"); return; }
        terminalUIManager.Print(GetCommandDataByString(args[0]).description);
    }
    void CmdOverclock(string[] args) { }
}