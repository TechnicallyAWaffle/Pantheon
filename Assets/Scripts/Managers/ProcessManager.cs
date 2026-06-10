using System.Collections;
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
    DaemonManager daemonManager;

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        terminalUIManager = referenceManager.terminalUIManager;
        queueManager = referenceManager.queueManager;
        daemonManager = referenceManager.daemonManager;
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
        if (processData == null)
        {
            terminalUIManager.Print("Process " + args[0] + " not recognized");
            return;
        }

        int memoryUsage = processData.memoryUsage;
        if (isServer)
        {
            if ((owner.reservedServerMemory - owner.busyServerMemory) < memoryUsage)
            {
                terminalUIManager.Print("Insufficient memory");
                return;
            }
            canRun = true;
            //Modify busy memory
            owner.ModifyBusyMemory(processQueue, memoryUsage);
            GlobalEventBus.MemoryChanged(owner, memoryUsage, owner.GetComponent<ProcessQueue>());
            Debug.Log("Attempting to run Process " + processData.processName + " on server queue");
        }
        else
        {
            if ((owner.localProcessQueue._openMemory - owner.busyLocalMemory) < memoryUsage)
            {
                terminalUIManager.Print("Insufficient memory");
                return;
            }
            canRun = true;
            //Modfiy busy memory
            owner.ModifyBusyMemory(processQueue, memoryUsage);
            GlobalEventBus.MemoryChanged(owner, processData.memoryUsage, owner.GetComponent<ProcessQueue>());
            Debug.Log("Attempting to run Process " + processData.processName + " on local queue");
        }

        //If can't run then just return
        if (!canRun) return;

        //Compile list of all arguments and flags
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

        //Validate arguments
        if (!ValidateFlags(argsList.ToArray(), processData))
        {
            string errorMessageArguments = string.Empty;
            foreach (SOProcessData.ArgumentType argument in processData.arguments)
            {
                errorMessageArguments += "<" + argument.ToString() + "> ";
            }
            terminalUIManager.Print("Invalid arguments detected. Process " + processData.processName
                + " expects arguments " + errorMessageArguments);
            return;
        }

        //Flag parsing is reach goal. In future I'll probably send this to a FlagManager once the RunningProcess has been created 
        //since that's the object that actually has data that gets changed during runtime
        //ParseFlags(flagsList.ToArray());
        Debug.Log("Adding process " + processData.processName + " to " + processQueue.name + " with owner " + owner.name);
        queueManager.AddProcess(processData, processQueue, owner, argsList.ToArray());

    }

    private bool ValidateFlags(string[] args, SOProcessData processData)
    {
        int argIndex = 0;
        foreach (SOProcessData.ArgumentType argument in processData.arguments)
        {
            bool isValidProcessID = false;
            bool isValidDaemon = false;
            if (args.Length > 0)
            {
                isValidProcessID = queueManager.AllRunningProcessesByID.TryGetValue(args[argIndex], out RunningProcess process);
                isValidDaemon = daemonManager.AllRevealedDaemons.TryGetValue(args[argIndex], out DaemonBase daemon);
            }

            switch (argument)
            {
                case SOProcessData.ArgumentType.NONE:
                    if (args.Length == 0)
                        return true;
                    break;
                case SOProcessData.ArgumentType.PROCESSID:
                    if (isValidProcessID)
                        return true;
                    break;
                case SOProcessData.ArgumentType.DAEMON:
                    if (isValidDaemon)
                        return true;
                    break;
                case SOProcessData.ArgumentType.PROCESSORDAEMON:
                    if (isValidProcessID || isValidDaemon)
                        return true;
                    break;
            }
            argIndex++;
        }
        return false;
    }

    public void RemoveAndCleanupProcess(string processID)
    {
        Debug.Log("Removing process with ID" + processID);
        RunningProcess process = queueManager.AllRunningProcessesByID[processID];
        process.script.OnKilled();
        process.queue.queue.Remove(process);
        process.owner.ownedProcesses.Remove(process);
        queueManager.AllRunningProcessesByID.Remove(processID);
        GameObject.Destroy(process);
    }


    public bool CheckProcessOwnership(RunningProcess process, Entity compareTo)
    {
        return compareTo == process.owner;
    }


}
