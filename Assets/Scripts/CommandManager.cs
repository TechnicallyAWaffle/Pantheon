using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CommandManager : MonoBehaviour, ICommandIndex
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI outputText;


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
    }

    [SerializeField] private SOCommand[] baseCommandData;
    private Dictionary<string, (SOCommand data, Action<string[]> handler)> commands;

    private Dictionary<string, Action<string[]>> BuildHandlerMap()
    {
        return
            new Dictionary<string, Action<string[]>>
            {
                { "run",    args => CmdRun(args)    },
                { "stop",   args => CmdStop(args)   },
                { "delete", args => CmdDelete(args) },
                { "list",   args => CmdList(args)   },
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

        // Check the reverse — handlers with no matching SO
        foreach (var key in handlerMap.Keys)
        {
            if (!commands.ContainsKey(key))
                Debug.LogWarning($"Handler '{key}' has no matching CommandDataSO.");
        }
    }

    void OnSubmit(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        Print("> " + input);
        ParseInput(input.Trim());

        inputField.text = "";
        inputField.ActivateInputField(); // keep focus
    }

    void ParseInput(string input)
    {
        string[] tokens = input.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        string verb = tokens[0].ToLower();
        string[] args = tokens.Length > 1
            ? tokens[1..]
            : new string[0];

        if (commands.TryGetValue(verb, out var entry))
            entry.handler(args);
        else
            Print($"Unknown command: '{verb}'. Type 'help' for a list.");
    }

    // --- Command Handlers ---

    void CmdRun(string[] args)
    {
        if (args.Length == 0) { Print("Usage: run <program>"); return; }
        string programName = args[0];
        Print($"Running '{programName}'...");
        
    }

    void CmdStop(string[] args)
    {
        if (args.Length == 0) { Print("Usage: stop <program>"); return; }
        Print($"Stopping '{args[0]}'...");
    }

    void CmdDelete(string[] args) { /* ... */ }
    void CmdList(string[] args) { Print("Programs: program1, program2"); }

    void Print(string message)
    {
        outputText.text += message + "\n";
    }
}