using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//README
//Anything that interacts with visual elements should delegate their logic here to avoid clutter


public class TerminalUIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private ScrollRect scrollRect;

    private Dictionary<int, string> encryptionIntToDisplayName = new()
    {
        {1, "light"},
        {2, "medium"},
        {3, "heavy"},
    };

    private Dictionary<int, string> authorityIntToDisplayName = new()
    {
        {0, "user"},
        {1, "admin"},
        {2, "root"},
        {3, "kernel"},
    };

    public void Print(string output)
    {
        outputText.text += output + "\n";
    }

}
