using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class CommandLine : MonoBehaviour
{
    private TextField _inputField;
    private VisualElement _consoleOutput;
    private VisualElement _commandHelp;
    public readonly UnityEvent<string> OnCommand = new();

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _inputField = root.Q<TextField>("CommandLineField"); 
        _consoleOutput = root.Q<VisualElement>("CommandOutput");
        _commandHelp = root.Q<VisualElement>("CommandInfo");

        _inputField.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        _inputField.RegisterValueChangedCallback(OnInputChanged);
    }

    private void OnDisable()
    {
        _inputField.UnregisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        _inputField.UnregisterValueChangedCallback(OnInputChanged);
    }

    void Start()
    {
        ShowConsoleOutput();
    }

    private void OnInputChanged(ChangeEvent<string> e)
    {
        string current = e.newValue;

        if (current.Length > 0)
        {
            ShowCommandHelp();
        }
        else
        {
            ShowConsoleOutput();
        }
    }

    private void ShowConsoleOutput()
    {
        _consoleOutput.EnableInClassList("hidden", false);  // adds the class
        _commandHelp.EnableInClassList("hidden", true); // removes the class
    }

    private void ShowCommandHelp()
    {
        _consoleOutput.EnableInClassList("hidden", true);  // adds the class
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

        _inputField.SetValueWithoutNotify(string.Empty);
        _inputField.schedule.Execute(() => _inputField.Focus());

        e.StopPropagation();
    }

    private void HandleCommand(string command)
    {
        Debug.Log($"Command Submitted: {command}");
        OnCommand.Invoke(command);
    }
}