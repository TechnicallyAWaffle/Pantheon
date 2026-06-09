using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    //Refs
    private ReferenceManager referenceManager;
    private QueueManager queueManager;
    private ProcessQueue serverProcessQueue;
    [HideInInspector] public ProcessQueue localProcessQueue;

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        queueManager = referenceManager.queueManager;
        serverProcessQueue = referenceManager.serverProcessQueue;
    }

    private void Awake()
    {
        localProcessQueue = GetComponent<ProcessQueue>();
    }


    // Ok irl 0 is kernel but for the sake of not losing my mind we're doing 0 - 3 with 3 being the highest authority
    // This is so I don't confuse the shit out of myself trying to compare this to encryption levels
    public int authority;
    
    public int reservedServerMemory;
    public int reservedServerCompute;
    public int busyLocalMemory = 0; //These two values are for memory that is currently being used by a process
    public int busyServerMemory = 0;

    public int temperature;
    public List<RunningProcess> ownedProcesses;
    public List<DaemonBase> daemons;

    public void ModifyBusyMemory(ProcessQueue queue, int incomingValue)
    {
        Debug.Log("Modifying Busy Memory: " + incomingValue);
        if (queue == serverProcessQueue)
        {
            busyServerMemory += incomingValue;
            Debug.Log("New Server Value: " + busyServerMemory);
        }
        else
        {
            busyLocalMemory += incomingValue;
            Debug.Log("New Local Value: " + busyLocalMemory);
        }
    }

    public void RequestServerMemory(int amountRequested)
    {
        int openMemory = serverProcessQueue.openMemory;
        if (amountRequested > openMemory)
        {
            reservedServerMemory += openMemory;
            serverProcessQueue.openMemory = 0;
            GlobalEventBus.RequestMemory(this, openMemory);
        }
        else
        {
            openMemory -= amountRequested;
            reservedServerMemory += amountRequested;
            GlobalEventBus.RequestMemory(this, amountRequested);
        }
    }

    public void RequestServerCompute(int amountRequested)
    {
        int openCompute = serverProcessQueue.openCompute;
        if (amountRequested > openCompute)
        {
            reservedServerCompute += openCompute;
            serverProcessQueue.openCompute = 0;
        }
        else
        {
            openCompute -= amountRequested;
            reservedServerCompute += amountRequested;
        }
        GlobalEventBus.RequestCompute(this, amountRequested);
    }


    /*
    public void ModifyMemory(ProcessQueue queue, int incomingValue)
    {
        if (queue == serverProcessQueue)
        {
            reservedServerMemory += incomingValue;
            busyLocalMemory -= incomingValue;
        }
        else
        { 
            busyLocalMemory -= incomingValue;
            reservedLocalMemory += incomingValue;
        }
        
    }

    public void ModifyCompute(ProcessQueue queue, int incomingValue)
    {
        if (queue == serverProcessQueue)
            reservedServerCompute += incomingValue;
        else
            reservedLocalCompute += incomingValue;
    }
    */

}