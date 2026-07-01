using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class CommandLine : MonoBehaviour
{
    [SerializeField] TerminalUIManager terminalUI;
    private TextField _inputField;
    private VisualElement _output;
    private VisualElement _commandHelp;
    private Label _commandOutput;
    public readonly UnityEvent<string> OnCommand = new();
    private CommandManager commandManager;

    //Autocomplete visuals
    [SerializeField] private VisualTreeAsset processTemplate;
    private List<SOProcessData> autoCompletableProcesses = new();
    private List<VisualTreeAsset> autoCompleteUIProcessObjects = new();
    [SerializeField] private Label helpProcessName;
    [SerializeField] private Label helpProcessDescrption;
    [SerializeField] private Label helpProcessMemoryUsage;
    [SerializeField] private Label helpProcessEncryption;
    [SerializeField] private Label helpProcessExecutionTime;

    private void OnEnable()
    {


        var root = GetComponent<UIDocument>().rootVisualElement;
        _inputField = root.Q<TextField>("CommandLineField"); 
        _output = root.Q<VisualElement>("Output");
        _commandHelp = root.Q<VisualElement>("CommandInfo");
        _commandOutput = root.Q<Label>("CommandOutput");
        _commandOutput.dataSource = terminalUI;

        _inputField.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        _inputField.RegisterValueChangedCallback(OnInputChanged);

        _inputField.RegisterCallback<FocusOutEvent>(evt =>
        {
            // Re-schedule to next frame to avoid focus conflicts
            _inputField.schedule.Execute(() => _inputField.Focus());
        });
    }

    private void OnDisable()
    {
        _inputField.UnregisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        _inputField.UnregisterValueChangedCallback(OnInputChanged);
    }

    void Start()
    {
        commandManager = ReferenceManager.Instance.commandManager;
        _inputField.Focus();
        ShowConsoleOutput();
    }

    private void OnInputChanged(ChangeEvent<string> e)
    {
        string current = e.newValue;

        if (current.Length > 0)
        {
            UpdateCommandHelp();
            ShowCommandHelp();
        }
        else
        {
            ShowConsoleOutput();
        }
    }

    private void UpdateCommandHelp()
    {
        autoCompletableProcesses  = commandManager.BuildAutoCompleteList(_inputField.text);
        foreach (SOProcessData process in autoCompletableProcesses)
        {
            Debug.Log("ADDING UI: " + process.processName);
            TemplateContainer instance = processTemplate.Instantiate();
            instance.Q<Label>("Process").text = process.processName;
        }
    }

    private void ShowConsoleOutput()
    {
        _output.EnableInClassList("hidden", false);  // adds the class
        _commandHelp.EnableInClassList("hidden", true); // removes the class
    }

    private void ShowCommandHelp()
    {


        _output.EnableInClassList("hidden", true);  // adds the class
        _commandHelp.EnableInClassList("hidden", false); // removes the class
    }

    private void OnKeyDown(KeyDownEvent e)
    {

        if (e.keyCode != KeyCode.Return && e.keyCode != KeyCode.KeypadEnter)
        {
            return;
        }

        string submitted = _inputField.value;

        if (string.IsNullOrWhiteSpace(submitted))
        {
            return;
        }

        HandleCommand(submitted);

        _inputField.value = string.Empty;
        _inputField.schedule.Execute(() => _inputField.Focus());

        e.StopPropagation();
    }

    private void HandleCommand(string command)
    {
        Debug.Log($"Command Submitted: {command}");
        OnCommand.Invoke(command);
    }
}