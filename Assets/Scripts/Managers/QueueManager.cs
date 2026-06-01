using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [HideInInspector] public ProcessQueue playerLocalQueue;
    [HideInInspector] public ProcessQueue serverQueue;
    [HideInInspector] public ProcessQueue enemyLocalQueue;

    private void Start()
    {
        playerLocalQueue = new ProcessQueue();
        serverQueue = new ProcessQueue();
        enemyLocalQueue = new ProcessQueue();

        GlobalEventBus.OnComputeChanged += RecalculateProcessExecutionTimes;
    }

    public void AddProcess(SOProcessData process, ProcessQueue queue, Entity owner, string[] processArguments)
    {
        RunningProcess runningProcessInstance = new RunningProcess();
        runningProcessInstance.data = process;
        runningProcessInstance.owner = owner;
        runningProcessInstance.timeRemaining = process.baseExecutionTime;
        runningProcessInstance.baseTime = process.baseExecutionTime;
        runningProcessInstance.encryption = process.encryption;
        runningProcessInstance.arguments = processArguments;
        runningProcessInstance.queue = queue;
        GlobalEventBus.ProcessQueued(runningProcessInstance);

        owner.ownedProcesses.Add(runningProcessInstance);

    }

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
                if(!process.isSuspended)
                    process.timeRemaining -= Time.deltaTime;

                if (process.timeRemaining <= 0)
                {
                    process.data.processScript.Execute(process.arguments);
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


    //Event Hooks
    public void RecalculateProcessExecutionTimes(Entity entity, int newCompute)
    {
        //Take first element of each queue and search for all RunningProcesses with the matching owner instance and do some math mojo im too lazy to figure out rn
    }

}
