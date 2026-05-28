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
            for (int i = queue.Count - 1; i >= 0; i--)
            {
                queue[i].timeRemaining -= Time.deltaTime;
                if (queue[i].timeRemaining <= 0)
                {
                    GlobalEventBus.ProcessCompleted(queue[i]);

                    //Execute process here, defer to commandindex for their logic. lookup by string maybe idk?
                    queue.RemoveAt(i);
                }
            }
        }
    }

}
