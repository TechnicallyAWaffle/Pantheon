using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//README
//Anything that interacts with visual elements should delegate their logic here to avoid clutter


public class TerminalUIManager : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private TextMeshProUGUI playerReservedServerMemory;
    [SerializeField] private TextMeshProUGUI playerReservedServerCompute;
    [SerializeField] private TextMeshProUGUI opponentReservedServerMemory;
    [SerializeField] private TextMeshProUGUI opponentReservedServerCompute;
    [SerializeField] private TextMeshProUGUI playerLocalMemory;
    [SerializeField] private TextMeshProUGUI playerLocalCompute;
    [SerializeField] private TextMeshProUGUI opponentLocalMemory;
    [SerializeField] private TextMeshProUGUI opponentLocalCompute;
    //[SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
        //GlobalEventBus.OnMemoryChanged += UpdateMemoryUI;
        outputText.text = string.Empty;
    }

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
        string currentText = outputText.text;
        outputText.text = output + "\n" + currentText;
    }

    private void UpdateMemoryUI()
    { 
        
    }

    public void UpdateServerMemoryAndComputeDistribution()
    { 
        //playerReservedServerMemory.text = 
    }


}
