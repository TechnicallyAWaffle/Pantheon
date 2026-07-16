using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CommandLine : MonoBehaviour
{
    //Refs
    ReferenceManager referenceManager;
    CommandManager commandManager;

    [SerializeField] TerminalUIManager terminalUI;
    private TextField _inputField;
    private VisualElement _output;
    private VisualElement _commandHelp;
    private Label _commandOutput;
    public readonly UnityEvent<string> OnCommand = new();

    //Autocomplete stuff
    [SerializeField] private VisualTreeAsset processTemplate;
    private VisualElement autoCompleteProcessUI;
    private Dictionary<SOProcessData, AutoCompleteUI> autoCompleteProcesses = new();
    SOProcessData activeAutoCompleteProcess;
    private Label processNameUI;
    private Label processMemoryUsageUI;
    private Label processExecutionTimeUI;
    private Label processEncryptionUI;
    private Label processDescriptionUI;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        autoCompleteProcessUI = root.Q<VisualElement>("Commands");
        _inputField = root.Q<TextField>("CommandLineField");
        _output = root.Q<VisualElement>("Output");
        _commandHelp = root.Q<VisualElement>("CommandInfo");
        _commandOutput = root.Q<Label>("CommandOutput");
        _commandOutput.dataSource = terminalUI;

        processNameUI = root.Q<Label>("ProcessName");
        processMemoryUsageUI = root.Q<Label>("ProcessMemoryUsage");
        processExecutionTimeUI = root.Q<Label>("ProcessExecutionTime");
        processEncryptionUI = root.Q<Label>("ProcessEncryptionText");
        processDescriptionUI = root.Q<Label>("ProcessDescriptionText");


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
        referenceManager = ReferenceManager.Instance;
        commandManager = referenceManager.commandManager;
        _inputField.Focus();
        ShowConsoleOutput();
    }

    private void OnInputChanged(ChangeEvent<string> e)
    {
        string current = e.newValue;
        UpdateAutoComplete(current);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (activeAutoCompleteProcess)
            {
                string[] splitString = _inputField.text.Split(' ');
                _inputField.value = splitString[0] + " " + activeAutoCompleteProcess.processName;
                _inputField.SelectRange(_inputField.text.Length, _inputField.text.Length);
            }
        }
    }

    private void ShowProcessHelp(SOProcessData processData)
    {
        activeAutoCompleteProcess = processData;
        processNameUI.text = processData.processName;
        processMemoryUsageUI.text = processData.memoryUsage.ToString();
        processExecutionTimeUI.text = processData.baseExecutionTime.ToString();
        processEncryptionUI.text = processData.encryption.ToString();
        processDescriptionUI.text = processData.description;
    }

    private void UpdateAutoComplete(string input)
    {
        HashSet<SOProcessData> processesToKeep = new();
        bool showedFirstAutoCompleteUI = false;
        
        List<SOProcessData> autoCompleteList = commandManager.BuildAutoCompleteList(input);
        if (autoCompleteList.Count > 0)
            ShowCommandHelp();
        else
        {
            ShowConsoleOutput();
            activeAutoCompleteProcess = null;
        }
        foreach (SOProcessData processData in autoCompleteList)
        {
            TemplateContainer uiInstance = processTemplate.Instantiate();
            var instance = new AutoCompleteUI
            {
                Root = uiInstance,
                processName = processData.processName,
                memoryUsage = processData.memoryUsage,
                executionTime = processData.baseExecutionTime,
                encryption = processData.encryption,
                processDescription = processData.description
            };

            if (!showedFirstAutoCompleteUI)
            {
                showedFirstAutoCompleteUI = true;
                ShowProcessHelp(processData);
            }

            //Check if process is in the ui Dictionary. If not, add it
            if (!autoCompleteProcesses.ContainsKey(processData))
            {
                //Debug.Log("Adding to list of autocomplete processes: " + processData);
                autoCompleteProcesses.Add(processData, instance);
                uiInstance.Q<Label>("Command").text = processData.processName;
                autoCompleteProcessUI.Add(uiInstance);
            }
            processesToKeep.Add(processData);
        }

        List<SOProcessData> processesToRemove = new();
        foreach (KeyValuePair<SOProcessData, AutoCompleteUI> processUI in autoCompleteProcesses)
        { 
            if(!processesToKeep.Contains(processUI.Key))
                processesToRemove.Add(processUI.Key);
        }
        foreach (SOProcessData process in processesToRemove)
        {
            Debug.Log("Removing process from autocomplete list: " + process);
            autoCompleteProcessUI.Remove(autoCompleteProcesses[process].Root);
            autoCompleteProcesses.Remove(process);
        }
    }

    public class AutoCompleteUI
    {
        public TemplateContainer Root;
        public string processName;
        public int memoryUsage;
        public float executionTime;
        public int encryption;
        public string processDescription;
        public int uiIndex;
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

        GlobalEventBus.TutorialSubmit(submitted);
        
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