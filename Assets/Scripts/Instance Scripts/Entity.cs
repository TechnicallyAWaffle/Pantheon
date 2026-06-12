using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    // Ok irl 0 is kernel but for the sake of not losing my mind we're doing 0 - 3 with 3 being the highest authority
    // This is so I don't confuse the shit out of myself trying to compare this to encryption levels
    public int authority;
    public int reservedServerMemory = 0;
    public int reservedServerCompute = 0;
    public ModVar availableLocalMemory;
    public ModVar availableServerMemory;

    public int temperature;
    public List<RunningProcess> ownedProcesses;
    public List<DaemonBase> daemons;

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

        availableLocalMemory = new ModVar(localProcessQueue._openMemory);
        availableServerMemory = new ModVar(reservedServerMemory);
    }

    private void Awake()
    {
        localProcessQueue = GetComponent<ProcessQueue>();
    }

    public void ModifyAvailableMemory(ITargetable source, ProcessQueue queue, int incomingValue)
    {
        WriteDebug("Adding modifier for busy memory of " + gameObject.name + " with value: " + incomingValue);
        if (queue == serverProcessQueue)
        {
            availableServerMemory.CreateAddMod(source, incomingValue);
            //WriteDebug("New Server Value: " + busyServerMemory);
        }
        else
        {
            availableLocalMemory.CreateAddMod(source, incomingValue);
            //WriteDebug("New Local Value: " + busyLocalMemory);
        }
    }

    public void RemoveAvailableMemoryMod(ITargetable source, ProcessQueue queue)
    {
        if (queue == serverProcessQueue)
        {
            availableServerMemory.RemoveAddMod(source);
        }
        else
        {
            availableLocalMemory.RemoveAddMod(source);
        }
    }


    public void RequestServerMemory(int incomingValue)
    {
        int openMemory = serverProcessQueue._openMemory;
        int actual = Mathf.Min(incomingValue, openMemory);
        openMemory -= actual;
        availableServerMemory.BaseValue += actual;
    }

    public void RelinquishServerMemory(int incomingValue)
    {
        int openMemory = serverProcessQueue._openMemory;
        int actual = Mathf.Min(incomingValue, reservedServerMemory);
        openMemory += actual;
        availableServerMemory.BaseValue -= actual;
    }


    public void RequestServerCompute(int amountRequested)
    {
        int openCompute = serverProcessQueue._openCompute;
        if (amountRequested > openCompute)
        {
            reservedServerCompute += openCompute;
            serverProcessQueue._openCompute = 0;
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

    private void WriteDebug(string message)
    {
        UnityEngine.Debug.Log("<color=orange>ENTITY: " + message);
    }

}