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
    [SerializeField] private TextMeshProUGUI inputText;
    private ReferenceManager referenceManager;
    private TerminalUIManager terminalUIManager;
    private ProcessManager processManager;
    private QueueManager queueManager;
    private Entity player;

    //Tuning
    readonly char[] trimParams = {'_', ' '};
    [SerializeField] private float inputSuffixBlinkRate = 0.5f;

    //Runtime Vars
    SOCommand lastInputtedCommand;
    private string currentText;
    private float timer;
    private bool inputSuffixVisible = true;

    //DEV DEBUG 
    private void Update()
    {
        //Keep focus on input field for now
        if (Input.GetMouseButtonDown(0))
        {
            inputField.ActivateInputField();
        }

        if (timer >= inputSuffixBlinkRate)
        {
            inputSuffixVisible = !inputSuffixVisible;
            timer = 0;
            UpdateInputFieldDisplay();
        }
        else
            timer += Time.deltaTime;

    }

    void Start()
    {
        RegisterCommands();
        inputField.onSubmit.AddListener(OnSubmit);
        inputField.text = "";
        inputField.ActivateInputField();
        //inputField.onValueChanged.AddListener(SetText);

        referenceManager = ReferenceManager.Instance;
        terminalUIManager = referenceManager.terminalUIManager;
        processManager = referenceManager.processManager;
        player = referenceManager.player;
        queueManager = referenceManager.queueManager;

    }

    private void SetText(string value)
    {
        currentText = value;
        inputSuffixVisible = true;
        timer = 0f;
        UpdateInputFieldDisplay();
    }

    private void UpdateInputFieldDisplay()
    {
        inputText.text = currentText + (inputSuffixVisible ? '_' : ' ');
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
                { "ping",   args => CmdPing(args)   },
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

        string trimmedInput = input.Trim(trimParams);
        terminalUIManager.Print("> " + trimmedInput);
        
        ParseInput(trimmedInput);

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
    }

    void CmdRunServer(string[] args)
    {
        if (args.Length == 0) { terminalUIManager.Print("Usage: srun <program> <-arguments>"); return; }

        processManager.TryRunProcess(args, player, queueManager.serverQueue, true);
    }


    void CmdSuspend(string[] args)
    {
        if (args.Length == 0) { terminalUIManager.Print("Usage: stop <program>"); return; }

    }

    void CmdKill(string[] args) { /* ... */ }

    void CmdHelp(string[] args) 
    {
        if (args.Length == 0) { terminalUIManager.Print("Usage: help <process>"); return; }
        //TODO: Genericize description getting
        if (!GetCommandDataByString(args[0]))
        {
            terminalUIManager.Print("Command not reconized");
            return;
        }
        else
            terminalUIManager.Print(GetCommandDataByString(args[0]).description);
        if (!processManager.GetProcessByName(args[0]))
        {
            terminalUIManager.Print("Process not recognized");
            return;
        }
        else
            terminalUIManager.Print(processManager.GetProcessByName(args[0]).description);
    }

    void CmdPing(string[] args)
    { 
    }

    void CmdOverclock(string[] args) { }
}