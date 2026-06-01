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

    private string[] ParseArguments(string[] commandArgs, string[] processArgs)
    {
        //See if any of the arguments match the possible arguments from the process's scriptableobject
        return commandArgs.Intersect(processArgs).ToArray();
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
            string[] processArguments = ParseArguments(args, processData.arguments);
            //ParseFlags(args.Except(processData.arguments).ToArray(), processData);

            queueManager.AddProcess(processData, processQueue, entity, processArguments);
        }
    }

    /*
     * //FLAG IMPLEMENTATION IS REACH GOAL
            //Anything that's NOT a matching argument of the process might be a flag, so send it over to ParseFlags()
    private void ParseFlags(string[] flags, SOProcessData processData)
    {
        foreach (string flagName in flags)
        {
            Debug.Log("Parsing flag: " + flagName);
            GetFlagByName(flagName).flagScript.ApplyFlag(processData);
        }
    }
    */

    public void TryRunProcessServer(string[] args, Entity entity, QueueManager.ProcessQueue processQueue)
    {
        SOProcessData processData = GetProcessByName(args[0]);
        Debug.Log("Attempting to run Process " + processData.processName + " on server queue");
        
        //Check memory to see if we can run the process
        if (entity.reservedServerMemory < processData.memoryUsage)
        {
            terminalUIManager.Print("Insufficient memory");
            return;
        }
        else
        {
            string[] processArguments = ParseArguments(args, processData.arguments);

            //ParseFlags(args.Except(processData.arguments).ToArray(), processData);

            queueManager.AddProcess(processData, processQueue, entity, processArguments);
        }
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

        //We can add additional functionality here (vfx, sfx, etc.)
    }

    public bool CheckProcessOwnership(RunningProcess process, Entity compareTo)
    {
        return compareTo == process.owner;
    }


}
