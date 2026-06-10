using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

//README
//Anything that interacts with visual elements should delegate their logic here to avoid clutter


public class TerminalUIManager : MonoBehaviour
{

    //Refs
    ReferenceManager referenceManager;

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
        referenceManager = ReferenceManager.Instance;

        GlobalEventBus.OnMemoryRequested += UpdateMemoryUI;
        GlobalEventBus.OnComputeRequested += UpdateComputeUI;
        GlobalEventBus.OnMemoryChanged += UpdateMemoryUI;
        GlobalEventBus.OnComputeChanged += UpdateComputeUI;

        outputText.text = string.Empty;
        UpdateMemoryUI(referenceManager.player);
        UpdateComputeUI(referenceManager.opponent);
        UpdateMemoryUI(referenceManager.opponent);
        UpdateComputeUI(referenceManager.opponent);
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
        string currentText = outputText.text;
        outputText.text = output + "\n" + currentText;
    }

    private void UpdateMemoryUI(Entity entity, int amount)
    {
        playerLocalMemory.text = entity.localProcessQueue._openMemory.ToString(); //TODO: Update all these to also show busy memory
        playerReservedServerMemory.text = entity.reservedServerMemory.ToString();
    }

    private void UpdateMemoryUI(Entity entity)
    {
        playerLocalMemory.text = (entity.localProcessQueue._openMemory - entity.busyLocalMemory).ToString();
        playerReservedServerMemory.text = entity.reservedServerMemory.ToString();
    }

    private void UpdateMemoryUI(RunningProcess process, Entity entity)
    {
        playerLocalMemory.text = (entity.localProcessQueue._openMemory - entity.busyLocalMemory).ToString();
        playerReservedServerMemory.text = entity.reservedServerMemory.ToString();
    }

    private void UpdateComputeUI(Entity entity, int amount)
    {
        playerLocalCompute.text = entity.localProcessQueue._openCompute.ToString();
        playerReservedServerCompute.text = entity.reservedServerCompute.ToString();
    }

    private void UpdateComputeUI(Entity entity)
    {
        playerLocalCompute.text = entity.localProcessQueue._openCompute.ToString();
        playerReservedServerCompute.text = entity.reservedServerCompute.ToString();
    }

    public void UpdateServerMemoryAndComputeDistribution()
    {
        //playerReservedServerMemory.text = 
    }


}
