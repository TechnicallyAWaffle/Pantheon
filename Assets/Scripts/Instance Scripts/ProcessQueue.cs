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
    private ProcessManager processManager;
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
    public List<RunningProcess> processesToRemove = new();

    private void Awake()
    {
        openMemory = startingMemory;
        openCompute = startingCompute;
    }

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        processManager = referenceManager.processManager;
        terminalUIManager = referenceManager.terminalUIManager;

        GlobalEventBus.OnMemoryRequested += UpdateOpenMemory;
        GlobalEventBus.OnComputeRequested += UpdateOpenCompute;
        
        //StartCoroutine(TickQueue());
    }

    void Update()
    {
        TickQueue(queue);
    }

    private void UpdateOpenMemory(Entity entity, int amountGiven)
    {
        openMemory -= amountGiven;
    }

    private void UpdateOpenCompute(Entity entity, int amountGiven)
    {
        openMemory -= amountGiven;
    }


    private IEnumerator TickQueue()
    {
        while (true)
        {   
            

            //Honesly this might cause us issues later but I'd be down to just stick this in update again if ticking
            //once per second doesn't work out 
            yield return new WaitForSeconds(1f);   
        }
    }


    void TickQueue(List<RunningProcess> queue)
    {
        foreach (RunningProcess process in queue)
        {
            //Skip over counting down time if process is suspended
            if (!process.isSuspended)
            {
                //Debug.Log("Ticking process: " + process.data.processName);
                process.timeRemaining -= Time.deltaTime;
            }

            if (process.timeRemaining <= 0 && !process.executed)
            {
                //Set executed flag to true
                process.executed = true;

                //Execute the process
                process.script.Execute(process.owner, process.arguments);
                GlobalEventBus.ProcessCompleted(process);

                //Print Process execution to terminal
                terminalUIManager.Print("Executing " +
                    process.data.processName + "_" + process.processID);
            }
        }

        //Now remove the process by ID and then clear the list
        foreach (RunningProcess process in processesToRemove)
            processManager.RemoveAndCleanupProcess(process.processID);
        processesToRemove.Clear();

    }

}