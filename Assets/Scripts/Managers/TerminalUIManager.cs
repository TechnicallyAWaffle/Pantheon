using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;

//README
//Anything that interacts with visual elements should delegate their logic here to avoid clutter


public class TerminalUIManager : MonoBehaviour
{
    public const int MaxLines = 15;

    [CreateProperty]
    public string ConsoleOutput => _consoleOutput;
    string _consoleOutput = "";

    //Refs
    ReferenceManager referenceManager;

    [SerializeField] private TextMeshProUGUI outputText;
    //[SerializeField] private ScrollRect scrollRect;


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
        _consoleOutput = $"{output}\n{_consoleOutput}";

        string[] lines = _consoleOutput.Split('\n');

        if (lines.Length > MaxLines)
        {
            _consoleOutput = string.Join("\n", lines, 0, MaxLines);
        }
    }
}
