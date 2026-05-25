using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ProcessManager : MonoBehaviour
{ 

    public class ProcessQueue : MonoBehaviour
    {
        public List<RunningProcess> serverQueue = new();
        public List<RunningProcess> localQueue = new();

        void Update()
        {
            TickQueue(serverQueue);
            TickQueue(localQueue);
        }

        void TickQueue(List<RunningProcess> queue)
        {
            for (int i = queue.Count - 1; i >= 0; i--)
            {
                queue[i].timeRemaining -= Time.deltaTime;
                if (queue[i].timeRemaining <= 0)
                {
                    //Execute process here, defer to commandindex for their logic. lookup by string maybe idk?
                    queue.RemoveAt(i);
                }
            }
        }
    }

    //args[0] is the process name
    //args[1..] are arguments
    public void TryRunProcess(string[] args, Entity entity)
    {
        //SOProcessData data = args[0]
    }

    public bool CheckProcessOwnership(RunningProcess process, Entity compareTo)
    {
        return compareTo == process.owner;
    }


}
