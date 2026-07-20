using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

//README
//Anything that interacts with visual elements should delegate their logic here to avoid clutter


public class TerminalUIManager : MonoBehaviour
{
    public const int MaxCharacters = 500;

    [CreateProperty]
    public string ConsoleOutput => _consoleOutput;
    string _consoleOutput = "";
    private ScrollView commandOutputScroll;

    //Refs
    [SerializeField] UIDocument uIDocument;
    ReferenceManager referenceManager;
    //[SerializeField] private ScrollRect scrollRect;

    private void OnEnable()
    {
        var root = uIDocument.rootVisualElement;
        commandOutputScroll = root.Q<ScrollView>("CommandOutputScroll");
        root.dataSource = this;

        commandOutputScroll.contentContainer.RegisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);
    }

    void OnDisable()
    {
        commandOutputScroll?.contentContainer.UnregisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);
    }

    private Dictionary<int, string> encryptionIntToDisplayName = new()
    {
        {1, "light"},
        {2, "medium"},
        {3, "heavy"},
        {4, "extreme"},
        {5, "convoluted"},
        {6, "incongruent"},
        {7, "entangled"},
        {8, "unquantifiable"},
    };

    private Dictionary<int, string> authorityIntToDisplayName = new()
    {
        {0, "user"},
        {1, "admin"},
        {2, "root"},
        {3, "kernel"},
    };

    private string ReturnEncryptionName(int encryptionLevel)
    {
        if (encryptionLevel > 8)
            return "paradoxical";
        else return encryptionIntToDisplayName[encryptionLevel];
    }

    public void Print(string output)
    {
        output = output.Trim();
        _consoleOutput = $"{_consoleOutput}\n<line-height=115%>{output}</line-height>";
        /*if (_consoleOutput.Length > MaxCharacters)
            _consoleOutput = _consoleOutput.Substring(_consoleOutput.Length - MaxCharacters);*/
    }

    void OnContentGeometryChanged(GeometryChangedEvent e)
    {
        var label = commandOutputScroll.Q<Label>("CommandOutput");
        label.MarkDirtyRepaint();

        commandOutputScroll.schedule.Execute(() =>
        {   
            if (commandOutputScroll.contentContainer.layout.height > commandOutputScroll.layout.height)
                commandOutputScroll.verticalScroller.value = commandOutputScroll.verticalScroller.highValue;
        }).ExecuteLater(1);
    }

}
