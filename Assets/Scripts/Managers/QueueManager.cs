using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
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

    private void Update()
    {
    }

    public void AddProcess(SOProcessData process, ProcessQueue queueObject, Entity owner, string[] processArguments)
    {
        //Instantiates a new process and parents it to a universal running process parent
        GameObject processObject = Instantiate(process.processObject, queueObject.transform);
        RunningProcess runningProcessInstance = processObject.GetComponent<RunningProcess>();

        //Add the process and its ID to the global process lookup dictionary
        runningProcessInstance.processID = GameManager.GenerateRandomID();
        GameManager.AllRunningProcessesByID.Add(runningProcessInstance.processID, runningProcessInstance);
        WriteDebug("Adding process with ID " + runningProcessInstance.processID);

        //Moves all the data from the scriptableobject to the new live RunningProcess instance
        runningProcessInstance.memoryUsed = process.memoryUsage;
        runningProcessInstance.data = process;
        runningProcessInstance.owner = owner;
        runningProcessInstance.Encryption = process.encryption;
        runningProcessInstance.arguments = processArguments;
        runningProcessInstance.queue = queueObject;
        runningProcessInstance.script = processObject.GetComponent<ProcessBase>();
        runningProcessInstance.script.runtimeProcessData = runningProcessInstance;

        //Modify time remaining based on compute                 //SHITASS IMPLEMENTATION
        if (queueObject == serverQueue)
            runningProcessInstance.baseTime = CalculateExecutionTimeBasedOnCompute(process.baseExecutionTime, owner.reservedServerCompute);
        else
            runningProcessInstance.baseTime = CalculateExecutionTimeBasedOnCompute(process.baseExecutionTime, owner.localProcessQueue._openCompute);
        runningProcessInstance.timeRemaining = runningProcessInstance.baseTime;
        WriteDebug("Process " + runningProcessInstance.data.processName + 
            " execution time lowered to " + runningProcessInstance.timeRemaining + " from " + process.baseExecutionTime);


        //Modify available memory
        owner.ModifyAvailableMemory(runningProcessInstance, queueObject, -runningProcessInstance.memoryUsed);

        //Fire global events. If the entity is the player, fire the associated event (this is for AI behaviour
        GlobalEventBus.ProcessQueued(runningProcessInstance, owner);
        if (owner == referenceManager.player)
            GlobalEventBus.PlayerQueuedProcess(runningProcessInstance);
        if (runningProcessInstance.data.processName == "kill")
        {
            GlobalEventBus.BaseKillProcessRan(runningProcessInstance);
        }

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

    private float CalculateExecutionTimeBasedOnCompute(float executionTime, float compute)
    {
        float finalValue = executionTime * (100 / (100 + compute));
        Debug.Log("adflmaldkfmaldfasdfdsf" + finalValue);
        return finalValue;
    }

    public void RecalculateProcessExecutionTimes(Entity entity, int newCompute)
    {
        //Take first element of each queue and search for all RunningProcesses with the matching owner instance and do some math mojo im too lazy to figure out rn
    }

    private void WriteDebug(string message)
    {
        UnityEngine.Debug.Log("<color=green>QUEUE MANAGER: " + message);
    }

}
