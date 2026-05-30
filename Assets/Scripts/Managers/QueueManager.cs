using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public ProcessQueue playerLocalQueue;
    public ProcessQueue serverQueue;
    public ProcessQueue enemyLocalQueue;

    private void Start()
    {
        playerLocalQueue = new ProcessQueue();
        serverQueue = new ProcessQueue();
        enemyLocalQueue = new ProcessQueue();

        GlobalEventBus.OnComputeChanged += RecalculateProcessExecutionTimes;
    }

    public void AddProcess(SOProcessData process, ProcessQueue queue, Entity owner)
    {
        RunningProcess runningProcessInstance = new RunningProcess();
        runningProcessInstance.data = process;
        runningProcessInstance.owner = owner;
        runningProcessInstance.timeRemaining = process.baseExecutionTime;
        runningProcessInstance.baseTime = process.baseExecutionTime;
        runningProcessInstance.encryption = process.encryption;
        GlobalEventBus.ProcessQueued(runningProcessInstance);
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
            RunningProcess firstProcessInLine = queue[0];
            firstProcessInLine.timeRemaining -= Time.deltaTime;
            if (firstProcessInLine.timeRemaining <= 0)
            {
                firstProcessInLine.data.processScript.Execute();
                GlobalEventBus.ProcessCompleted(firstProcessInLine);
                queue.RemoveAt(0);
            }
        }
    }

    //Event Hooks
    public void RecalculateProcessExecutionTimes(Entity entity, double newCompute)
    {
        //Take first element of each queue and search for all RunningProcesses with the matching owner instance and do some math mojo im too lazy to figure out rn
    }

}
