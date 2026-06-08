using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class QueueManager : MonoBehaviour
{
    [HideInInspector] public ProcessQueue playerLocalQueue;
    [HideInInspector] public ProcessQueue serverQueue;
    [HideInInspector] public ProcessQueue opponentLocalQueue;

    //Refs
    private ReferenceManager referenceManager;
    private TerminalUIManager terminalUIManager;

    //Runtime
    //This tracks every single running process instance by their processID
    public Dictionary<string, RunningProcess> AllRunningProcessesByID = new();

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        terminalUIManager = referenceManager.terminalUIManager;
        serverQueue = referenceManager.serverProcessQueue;
        playerLocalQueue = referenceManager.player.GetComponent<ProcessQueue>();
        opponentLocalQueue = referenceManager.opponent.GetComponent<ProcessQueue>();

        GlobalEventBus.OnComputeChanged += RecalculateProcessExecutionTimes;
        //GlobalEventBus.OnMemoryChanged += CheckAndRemoveProcessesInQueue;
    }

    public void AddProcess(SOProcessData process, ProcessQueue queueObject, Entity owner, string[] processArguments)
    {
        //Instantiates a new process and parents it to a universal running process parent
        GameObject processObject = Instantiate(process.processObject, queueObject.transform);
        RunningProcess runningProcessInstance = processObject.GetComponent<RunningProcess>();

        //Add the process and its ID to the global process lookup dictionary
        runningProcessInstance.processID = GenerateRandomProcessID();
        AllRunningProcessesByID.Add(runningProcessInstance.processID, runningProcessInstance);
        Debug.Log("Adding process with ID " + runningProcessInstance.processID);

        //Moves all the data from the scriptableobject to the new live RunningProcess instance
        runningProcessInstance.data = process;
        runningProcessInstance.owner = owner;
        runningProcessInstance.timeRemaining = process.baseExecutionTime;
        runningProcessInstance.baseTime = process.baseExecutionTime;
        runningProcessInstance.encryption = process.encryption;
        runningProcessInstance.arguments = processArguments;
        runningProcessInstance.queue = queueObject;
        runningProcessInstance.script = processObject.GetComponent<ProcessBase>();
        runningProcessInstance.script.runtimeProcessData = runningProcessInstance;

        //Fire global events. If the entity is the player, fire the associated event (this is for AI behaviour)
        GlobalEventBus.ProcessQueued(runningProcessInstance, owner);
        if (owner == referenceManager.player)
            GlobalEventBus.PlayerQueuedProcess(runningProcessInstance);

        //Add the RunningProcess instance to the queue and to the owner's list of running processes
        queueObject.queue.Add(runningProcessInstance);
        owner.ownedProcesses.Add(runningProcessInstance);

        //Print terminal output statment
        string location;
        if (queueObject == referenceManager.serverProcessQueue)
            location = "SERVER CONNECTION";
        else
            location = "LOCAL CLIENT";
        terminalUIManager.Print($"Process '{runningProcessInstance.data.processName}'_" 
            + runningProcessInstance.processID + " added to "
            + location + " scheduler queue");
    }

    private string GenerateRandomProcessID()
    {
        int id = Random.Range(100, 9000);
        return $"{id:D4}";
    }

    public void RemoveAndCleanupProcess(string processID)
    {
        Debug.Log("Removing process with ID" + processID);
        RunningProcess process = AllRunningProcessesByID[processID];
        process.queue.queue.Remove(process);
        process.owner.ownedProcesses.Remove(process);
        AllRunningProcessesByID.Remove(processID);
    }


    //Event Hooks

    public void CheckAndRemoveProcessesInQueue(Entity owner, int memoryChanged, ProcessQueue processQueue)
    {
        float totalMemoryUsed = 0;
        foreach (RunningProcess process in processQueue.queue)
        {
            totalMemoryUsed += process.memoryUsed;
        }
        //Need to know what memory to compare here 

    }

    public void RecalculateProcessExecutionTimes(Entity entity, int newCompute)
    {
        //Take first element of each queue and search for all RunningProcesses with the matching owner instance and do some math mojo im too lazy to figure out rn
    }

}
