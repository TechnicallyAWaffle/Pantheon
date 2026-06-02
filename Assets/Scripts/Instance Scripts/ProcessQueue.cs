using System;
using System.Collections.Generic;
using UnityEngine;

public class ProcessQueue : MonoBehaviour
{
    
    //Refs
    //Future implementation for setting unique server compute/memory levels, though honestly this can probably just be hardcoded lmao
    //SOServerData serverData; 
    public Entity owner; //Leave this blank for server

    //Tuning
    public int startingMemory;
    public int startingCompute;

    //Runtime
    public int openMemory; //memory up for grabs by anyone
    public int openCompute; //memory up for grabs by anyone

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

    }

}