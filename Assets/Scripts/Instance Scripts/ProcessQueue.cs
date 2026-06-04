using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ProcessQueue : MonoBehaviour
{

    //Refs
    //Future implementation for setting unique server compute/memory levels, though honestly this can probably just be hardcoded lmao
    //SOServerData serverData; 
    [HideInInspector] public Entity owner; //Leave this blank for server
    private ReferenceManager referenceManager;
    private QueueManager queueManager;
    private TerminalUIManager terminalUIManager;

    //Tuning
    [Header("Tuning vars")]
    public int startingMemory;
    public int startingCompute;

    [Header("Runtime vars, DO NOT MODIFY")]
    //Runtime
    public int openMemory; //memory up for grabs by anyone
    public int openCompute; //memory up for grabs by anyone

    public List<RunningProcess> queue = new();

    private void Awake()
    {
        openMemory = startingMemory;
        openCompute = startingCompute;
    }

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        queueManager = referenceManager.queueManager;
        terminalUIManager = referenceManager.terminalUIManager;
        StartCoroutine(TickQueue());
    }

    void Update()
    {
        //TickQueue(queue);
    }

    private IEnumerator TickQueue()
    {
        List<RunningProcess> processesToRemove = new();
        while (true)
        {   
            foreach (RunningProcess process in queue)
            {
                //Skip over counting down time if process is suspended
                if (!process.isSuspended)
                {
                    Debug.Log("Ticking process: " + process.data.processName);
                    process.timeRemaining -= 1;
                }

                if (process.timeRemaining <= 0 && !process.executed)
                {
                    //Set executed flag to true
                    process.executed = true;

                    //Execute the process
                    process.data.processObject.TryGetComponent<ProcessBase>(out ProcessBase processScript);
                    processScript.Execute(process.owner, process.arguments);
                    GlobalEventBus.ProcessCompleted(process);

                    //Print Process execution to terminal
                    terminalUIManager.Print("Executing " +
                        process.data.processName + "_" + process.processID);

                    //Add process to a list for later removal to avoid modification during iteration
                    if(process.data.removedWhenExecuted)
                        processesToRemove.Add(process);
                }
            }

            //Now remove the process by ID and then clear the list
            foreach (RunningProcess process in processesToRemove)
                queueManager.RemoveAndCleanupProcess(process.processID);
            processesToRemove.Clear();

            //Honesly this might cause us issues later but I'd be down to just stick this in update again if ticking
            //once per second doesn't work out 
            yield return new WaitForSeconds(1f);   
        }
    }


    void TickQueue(List<RunningProcess> queue)
    {
        List<RunningProcess> processesToRemove = new();
        foreach (RunningProcess process in queue)
        {
            //Skip over counting down time if process is suspended
            if (!process.isSuspended)
            {
                Debug.Log("Ticking process: " + process.data.processName);
                process.timeRemaining -= Time.deltaTime;
            }

            if (process.timeRemaining <= 0)
            {
                //Execute the process
                process.data.processObject.TryGetComponent<ProcessBase>(out ProcessBase processScript);
                processScript.Execute(process.owner, process.arguments);
                GlobalEventBus.ProcessCompleted(process);

                //Print Process execution to terminal
                terminalUIManager.Print("Executing " + 
                    process.data.processName + "_" + process.processID);

                //Remove processes from both the queue and the owner's list of owned processes
                //if(process.data.)
                processesToRemove.Add(process);
            }
        }

    }

}