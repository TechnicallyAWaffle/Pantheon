using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{

    //Refs
    public ReferenceManager referenceManager;
    QueueManager queueManager;

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
        queueManager = referenceManager.queueManager;
    }

    // Ok irl 0 is kernel but for the sake of not losing my mind we're doing 0 - 3 with 3 being the highest authority
    // This is so I don't confuse the shit out of myself trying to compare this to encryption levels
    public int authority;           
    public int reservedLocalMemory;
    public int reservedLocalCompute;
    public int reservedServerMemory;
    public int reservedServerCompute;
    public int temperature;
    public List<RunningProcess> ownedProcesses;

    
    //public string activeEnterprise;
    //public List<Daemon> daemons;

}