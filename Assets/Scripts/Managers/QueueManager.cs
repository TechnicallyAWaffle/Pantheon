using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class QueueManager : MonoBehaviour
{
    [HideInInspector] public ProcessQueue playerLocalQueue;
    [HideInInspector] public ProcessQueue serverQueue;
    [HideInInspector] public ProcessQueue opponentLocalQueue;

    [SerializeField] private Transform activeProcessesParent;

    //Refs
    [SerializeField] private ReferenceManager referenceManager;

    public class ProcessQueue : MonoBehaviour
    {
        public List<RunningProcess> queue = new();


        void Update()
        {
            TickQueue(queue);
        }

        void TickQueue(List<RunningProcess> queue)
        {

            foreach (RunningProcess process in queue)
            {
                //Skip over counting down time if process is suspended
                if (!process.isSuspended)
                    process.timeRemaining -= Time.deltaTime;

                if (process.timeRemaining <= 0)
                {
                    //Execute the process
                    process.data.processObject.TryGetComponent<ProcessBase>(out ProcessBase processScript);
                    processScript.Execute(process.owner, process.arguments);
                    GlobalEventBus.ProcessCompleted(process);

                    //Remove processes from both the queue and the owner's list of owned processes
                    process.owner.ownedProcesses.Remove(process);
                    queue.RemoveAt(0);
                }
            }
            /*
            RunningProcess firstProcessInLine = queue[0];
            firstProcessInLine.timeRemaining -= Time.deltaTime;
            if (firstProcessInLine.timeRemaining <= 0)
            {
                firstProcessInLine.data.processScript.Execute();
                GlobalEventBus.ProcessCompleted(firstProcessInLine);
                queue.RemoveAt(0);
            }
            */
        }
    }

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;

        playerLocalQueue = referenceManager.player.AddComponent<ProcessQueue>();
        serverQueue = this.AddComponent<ProcessQueue>();
        opponentLocalQueue = referenceManager.opponent.AddComponent<ProcessQueue>();

        GlobalEventBus.OnComputeChanged += RecalculateProcessExecutionTimes;
        GlobalEventBus.OnMemoryChanged += CheckAndRemoveProcessesInQueue;
    }

    public void AddProcess(SOProcessData process, ProcessQueue queue, Entity owner, string[] processArguments)
    {
        //Instantiates a new process and parents it to a universal running process parent
        GameObject processObject = Instantiate(process.processObject, activeProcessesParent);
        RunningProcess runningProcessInstance = processObject.GetComponent<RunningProcess>();
        
        //Moves all the data from the scriptableobject to the new live RunningProcess instance
        runningProcessInstance.data = process;
        runningProcessInstance.owner = owner;
        runningProcessInstance.timeRemaining = process.baseExecutionTime;
        runningProcessInstance.baseTime = process.baseExecutionTime;
        runningProcessInstance.encryption = process.encryption;
        runningProcessInstance.arguments = processArguments;
        runningProcessInstance.queue = queue;
        GlobalEventBus.ProcessQueued(runningProcessInstance);

        //Add the RunningProcess instance to the queue
        owner.ownedProcesses.Add(runningProcessInstance);

    }


    //Event Hooks

    public void CheckAndRemoveProcessesInQueue(Entity owner, int memoryChanged, QueueManager.ProcessQueue processQueue)
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
