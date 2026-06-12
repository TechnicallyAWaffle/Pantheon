using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Properties;
using UnityEngine.Rendering.Universal;

public class ProcessQueue : MonoBehaviour
{
    //Refs
    //Future implementation for setting unique server compute/memory levels, though honestly this can probably just be hardcoded lmao
    //SOServerData serverData;
    public Entity owner; //Leave this blank for server
    [HideInInspector] public string queueName;
    private ReferenceManager referenceManager;
    private ProcessManager processManager;
    private TerminalUIManager terminalUIManager;
    private Entity player;
    private Entity opponent;
    

    //Tuning
    [Header("Tuning vars")]
    public int startingMemory;
    public int startingCompute;

    [Header("Runtime vars, DO NOT MODIFY")]
    //Runtime
    [CreateProperty]
    public int OpenMemory => _openMemory;
    public int _openMemory; //memory up for grabs by anyone
    [CreateProperty]
    public int OpenCompute => _openCompute;
    public int _openCompute; //memory up for grabs by anyone

    public List<RunningProcess> queue = new();
    public List<RunningProcess> processesToRemove = new();

    [SerializeField] private float timeBetweenServerResets = 30;
    [SerializeField] private int resourcesWiped;
    private float currentTime;

    private void Awake()
    {
        _openMemory = startingMemory;
        _openCompute = startingCompute;
    }

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        processManager = referenceManager.processManager;
        terminalUIManager = referenceManager.terminalUIManager;
        player = referenceManager.player;
        opponent = referenceManager.opponent;

        GlobalEventBus.OnMemoryRequested += UpdateOpenMemory;
        GlobalEventBus.OnComputeRequested += UpdateOpenCompute;

        currentTime = timeBetweenServerResets;
        if (owner)
            queueName = owner.name + "LOCAL QUEUE";
        else
            queueName = "SERVER";
    }

    void Update()
    {
        TickQueue(queue);
        if(!owner)
            ResetServer();
    }

    private void ResetServer()
    {
        if (currentTime <= 0)
        {
            currentTime = timeBetweenServerResets;
            player.RelinquishServerMemory(resourcesWiped);
            player.RelinquishServerCompute(resourcesWiped);
            opponent.RelinquishServerMemory(resourcesWiped);
            opponent.RelinquishServerCompute(resourcesWiped);
            Debug.Log(currentTime);
            GlobalEventBus.SchedulerReset();
        }
        else
            currentTime -= Time.deltaTime;
    }


    private void UpdateOpenMemory(Entity entity, int amountGiven)
    {
        _openMemory -= amountGiven;
    }

    private void UpdateOpenCompute(Entity entity, int amountGiven)
    {
        _openMemory -= amountGiven;
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
        {
            WriteDebug("Removing and cleaning up process " + process.data.processName);
            processManager.RemoveAndCleanupProcess(process.processID);
        }
        processesToRemove.Clear();

    }

    private void WriteDebug(string message)
    {
        UnityEngine.Debug.Log("<color=#FF00AC>PROCESS QUEUE(" + queueName + ")" + message);
    }


}