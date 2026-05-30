using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static QueueManager;

public class ProcessManager : MonoBehaviour
{

    [SerializeField] private SOProcessData[] processDatabase;
    private Dictionary<string, SOProcessData> processesByName;

    ReferenceManager referenceManager;
    TerminalUIManager terminalUIManager;
    QueueManager queueManager;

    private void Awake()
    {
        processesByName = new Dictionary<string, SOProcessData>();

        foreach (var process in processDatabase)
        {
            if (!processesByName.TryAdd(process.processName.ToLower(), process))
                Debug.LogWarning($"Duplicate process name: '{process.processName}'");
        }
    }

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        terminalUIManager = referenceManager.terminalUIManager;
        queueManager = referenceManager.queueManager;
    }

    public SOProcessData GetProcessByName(string processName)
    {
        processesByName.TryGetValue(processName, out var processData);
        return processData;
    }

    //args[0] is the process name
    //args[1..] are arguments
    public void TryRunProcess(string[] args, Entity entity, QueueManager.ProcessQueue processQueue)
    {
        SOProcessData processData = GetProcessByName(args[0]);
        if (entity.reservedLocalMemory < processData.memoryUsage)
        {
            terminalUIManager.Print("Insufficient memory");
            return;
        }
        else
        {
            queueManager.AddProcess(processData, processQueue, entity);
        }
    }

    public void TryRunProcessServer(string[] args, Entity entity, QueueManager.ProcessQueue processQueue)
    {
        SOProcessData processData = GetProcessByName(args[0]);
        if (entity.reservedServerMemory < processData.memoryUsage)
        {
            terminalUIManager.Print("Insufficient memory");
            return;
        }
        else
        {
            queueManager.AddProcess(processData, processQueue, entity);
        }
    }

    public bool CheckProcessOwnership(RunningProcess process, Entity compareTo)
    {
        return compareTo == process.owner;
    }


}
