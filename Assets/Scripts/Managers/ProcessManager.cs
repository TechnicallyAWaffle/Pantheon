using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using static QueueManager;
using static UnityEngine.Rendering.GPUSort;

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

    private string[] ReturnArguments(string[] commandArgs, string[] processArguments)
    {
        //My math might be wrong on this but essentially this tries to tell how many arguments there are and then returns the elements
        //in that range
        return commandArgs[1..(processArguments.Length + 1)];
    }

    public void TryRunProcess(string[] args, Entity owner, ProcessQueue processQueue, bool isServer)
    {
        bool canRun = false;
        //Argument at index 0 is the processname
        SOProcessData processData = GetProcessByName(args[0]);
        if (isServer)
        {
            if (owner.reservedServerMemory < processData.memoryUsage)
            {
                terminalUIManager.Print("Insufficient memory");
                return;
            }
            canRun = true;
            //Take memory
            owner.reservedServerMemory -= processData.memoryUsage;
            GlobalEventBus.MemoryChanged(owner, processData.memoryUsage, owner.GetComponent<ProcessQueue>());
            Debug.Log("Attempting to run Process " + processData.processName + " on server queue");
        }
        else
        {
            if (owner.localProcessQueue.openMemory < processData.memoryUsage)
            {
                terminalUIManager.Print("Insufficient memory");
                return;
            }
            canRun = true;
            //Take memory
            owner.localProcessQueue.openMemory -= processData.memoryUsage;
            GlobalEventBus.MemoryChanged(owner, processData.memoryUsage, owner.GetComponent<ProcessQueue>());
            Debug.Log("Attempting to run Process " + processData.processName + " on local queue");
        }
        
        //If can't run then just return
        if (!canRun) return;

        List<string> argsList = new();
        List<string> flagsList = new();
        //Exclude first element since that's just the process name
        foreach (string argumentOrFlag in args[1..])
        {
            if (argumentOrFlag[0] != '-')
                argsList.Add(argumentOrFlag);
            else
                flagsList.Add(argumentOrFlag);
        }

        //Flag parsing is reach goal. In future I'll probably send this to a FlagManager once the RunningProcess has been created 
        //since that's the object that actually has data that gets changed during runtime
        //ParseFlags(flagsList.ToArray());
        queueManager.AddProcess(processData, processQueue, owner, argsList.ToArray());
    }

    
    public bool CheckAuthority(Entity entity, RunningProcess process)
    {
        return entity.authority >= process.encryption;
    }

    public void KillProcess(RunningProcess process)
    {
        //maybe i shouldn't name both of these properties queue lmfao
        //kill me
        process.queue.queue.Remove(process);
        GameObject.Destroy(process);

        //We can add additional functionality here (vfx, sfx, etc.)
    }

    public bool CheckProcessOwnership(RunningProcess process, Entity compareTo)
    {
        return compareTo == process.owner;
    }


}
