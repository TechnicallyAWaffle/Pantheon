using System.Collections.Generic;
using UnityEngine;

public class ProcessManager : MonoBehaviour
{

    [SerializeField] public SOProcessData[] processDatabase;
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
        //Load all processes
        processDatabase = Resources.LoadAll<SOProcessData>("ScriptableObjects/Processes");

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



    public void TryRunProcess(string[] args, Entity owner, ProcessQueue processQueue, bool isServer)
    {
        bool isBaseProcess = false;
        //Argument at index 0 is the processname
        SOProcessData processData = GetProcessByName(args[0]);

        if (processData.processName == "kill" || processData.processName == "suspend")            //TODO: SHITASS IMPLEMENTATION
            isBaseProcess = true;

        if (processData == null)
        {
            WriteDebug("Process " + args[0] + " not found");
            terminalUIManager.Print("Process " + args[0] + " not recognized");
            return;
        }

        int memoryUsage = processData.memoryUsage;
        if (isServer)
        {
            //Check for sufficient memory
            if ((owner.availableServerMemory.Value) < memoryUsage)
            {
                if(isBaseProcess)
                    terminalUIManager.Print("Insufficient memory. Base processes must be run on the server.");
                else
                    terminalUIManager.Print("Insufficient server memory.");
                return;
            }
            WriteDebug("Attempting to run Process " + processData.processName + " on server queue");
        }
        else
        {
            if ((owner.availableLocalMemory.Value) < memoryUsage)
            {
                terminalUIManager.Print("Insufficient local memory");
                return;
            }
            WriteDebug("Attempting to run Process " + processData.processName + " on local queue");
        }

        //If can't run then just return obselete because of early returns
        //if (!canRun) return;

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
        queueManager.AddProcess(processData, processQueue, owner, argsList.ToArray());

        if(owner)
            WriteDebug("Adding process " + processData.processName + " to " + processQueue.name + " LOCAL QUEUE, with owner " + owner.name);
        else
            WriteDebug("Adding process " + processData.processName + " to " + processQueue.name + ", with owner " + owner.name);

    }

    private bool ValidateFlags(string[] args, SOProcessData processData)
    {
        WriteDebug("Validating arguments [" + string.Join(", ", args) + "] against [" + string.Join(", ", processData.arguments) + "]");

        //If no args then it's truebert
        if (args.Length == 0 && processData.arguments.Length == 0)
            return true;
        if (args.Length != processData.arguments.Length)
        {
            WriteDebug("Process " + processData.processName + " has mismatched argument array lengths!");
            return false;
        }

        int argIndex = 0;
        foreach (SOProcessData.ArgumentType argument in processData.arguments)
        {
            bool isValidProcessID = false;
            bool isValidDaemon = false;

            isValidProcessID = GameManager.AllRunningProcessesByID.TryGetValue(args[argIndex], out RunningProcess process);
            isValidDaemon = GameManager.AllActiveDaemons.TryGetValue(args[argIndex], out DaemonBase daemon);

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
            if(argIndex < args.Length - 1)
                argIndex++;
        }
        return false;
    }

    public void RemoveAndCleanupProcess(string processID)
    {
        WriteDebug("Removing process with ID" + processID);
        RunningProcess process = GameManager.AllRunningProcessesByID[processID];
        process.script.OnKilled();
        process.queue.queue.Remove(process);
        process.owner.ownedProcesses.Remove(process);
        GameManager.AllRunningProcessesByID.Remove(processID);
        GameObject.Destroy(process.gameObject);
    }


    public bool CheckProcessOwnership(RunningProcess process, Entity compareTo)
    {
        return compareTo == process.owner;
    }


    private void WriteDebug(string message)
    {
        UnityEngine.Debug.Log("<color=yellow>PROCESS MANAGER: " + message);
    }

}
