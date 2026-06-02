using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    //Refs
    public ReferenceManager referenceManager;
    QueueManager queueManager;
    [SerializeField] ProcessQueue serverProcessQueue;
    [SerializeField] ProcessQueue localProcessQueue;

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        queueManager = referenceManager.queueManager;

        localProcessQueue = GetComponent<ProcessQueue>();
        //localProcessQueue.RequestMemory(localProcessQueue.startingMemory, this);

        resources.Add(serverProcessQueue, reservedLocalMemory);
    }

    // Ok irl 0 is kernel but for the sake of not losing my mind we're doing 0 - 3 with 3 being the highest authority
    // This is so I don't confuse the shit out of myself trying to compare this to encryption levels
    public int authority;
    
    public int reservedLocalMemory;
    public int reservedLocalCompute;
    public int reservedServerMemory;
    public int reservedServerCompute;

    public int busyLocalMemory = 0; //These two values are for memory that is currently being used by a process
    public int busySeverMemory = 0;

    public int temperature;
    public List<RunningProcess> ownedProcesses;
    public List<DaemonBase> daemons;

    public Dictionary<ProcessQueue, int> resources = new();

    public void ModifyBusyMemory(ProcessQueue queue, int incomingValue)
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

    public void RequestMemory(int amountRequested)
    {
        GlobalEventBus.RequestMemory(this, amountRequested);
        int openMemory = serverProcessQueue.openMemory;
        if (amountRequested > openMemory)
        {
            reservedServerMemory += openMemory;
            serverProcessQueue.openMemory = 0;
        }
        else
        {
            openMemory -= amountRequested;
            reservedServerMemory += amountRequested;
        }
    }

    public void RequestCompute(int amountRequested)
    {
        GlobalEventBus.RequestCompute(this, amountRequested);
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