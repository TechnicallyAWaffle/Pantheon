using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    //Refs
    ReferenceManager referenceManager;
    QueueManager queueManager;

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        queueManager = referenceManager.queueManager;
    }


    public int authority;           // 0 (kernel) to 3
    public int reservedLocalMemory;
    public int reservedLocalCompute;
    public int reservedServerMemory;
    public int reservedServerCompute;
    public int temperature;
    public List<RunningProcess> ownedProcesses;

    
    //public string activeEnterprise;
    //public List<Daemon> daemons;

}