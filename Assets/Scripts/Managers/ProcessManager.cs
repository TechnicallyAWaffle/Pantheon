using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using static QueueManager;

public class ProcessManager : MonoBehaviour
{

    [SerializeField] private SOProcessData[] processDatabase;
    private Dictionary<string, SOProcessData> processesByName;

    [SerializeField] private SOFlagData[] flagDatabase;
    private Dictionary<string, SOFlagData> flagsByName;

    ReferenceManager referenceManager;
    TerminalUIManager terminalUIManager;
    QueueManager queueManager;

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        terminalUIManager = referenceManager.terminalUIManager;
        queueManager = referenceManager.queueManager;
    }


    private void Awake()
    {
        //Set up string -> Process/Flag ScriptableObject data dictionary for lookup via GetProcessByName and GetFlagByName
        processesByName = new Dictionary<string, SOProcessData>();
        flagsByName = new Dictionary<string, SOFlagData>();

        foreach (var process in processDatabase)
        {
            if (!processesByName.TryAdd(process.processName.ToLower(), process))
                Debug.LogWarning($"Duplicate process name: '{process.processName}'");
        }

        foreach (var flag in flagDatabase)
        {
            if (!flagsByName.TryAdd(flag.flagName.ToLower(), flag))
                Debug.LogWarning($"Duplicate flag name: '{flag.flagName}'");
        }
    }

    public SOProcessData GetProcessByName(string processName)
    {
        processesByName.TryGetValue(processName, out var processData);
        return processData;
    }

    public SOFlagData GetFlagByName(string flagName)
    {
        flagsByName.TryGetValue(flagName, out var flagData);
        return flagData;
    }

    public void TryRunProcess(string[] args, Entity entity, QueueManager.ProcessQueue processQueue)
    {
        SOProcessData processData = GetProcessByName(args[0]);
        Debug.Log("Attempting to run Process " + processData.processName + " on local queue");
        if (entity.reservedLocalMemory < processData.memoryUsage)
        {
            terminalUIManager.Print("Insufficient memory");
            return;
        }
        else
        {
            //This just compares the command input to both the process's unique arguments and the universal flags you can apply to them
            string[] processArguments = args.Intersect(processData.arguments).ToArray();
            ParseFlags(args.Except(processData.arguments).ToArray(), processData);

            queueManager.AddProcess(processData, processQueue, entity, processArguments);
        }
    }

    private void ParseFlags(string[] flags, SOProcessData processData)
    {
        foreach (string flagName in flags)
        {
            Debug.Log("Parsing flag: " + flagName);
            GetFlagByName(flagName).flagScript.ApplyFlag(processData);
        }
    }

    public void TryRunProcessServer(string[] args, Entity entity, QueueManager.ProcessQueue processQueue)
    {
        SOProcessData processData = GetProcessByName(args[0]);
        Debug.Log("Attempting to run Process " + processData.processName + " on server queue");
        if (entity.reservedServerMemory < processData.memoryUsage)
        {
            terminalUIManager.Print("Insufficient memory");
            return;
        }
        else
        {
            string[] processArguments = args.Intersect(processData.arguments).ToArray();
            ParseFlags(args.Except(processData.arguments).ToArray(), processData);
            queueManager.AddProcess(processData, processQueue, entity, processArguments);
        }
    }

    public void KillProcess(RunningProcess process)
    {
        //kill me
        process.queue.queue.Remove(process);

        //Will be updated with other things later 
    }

    public bool CheckProcessOwnership(RunningProcess process, Entity compareTo)
    {
        return compareTo == process.owner;
    }


}
