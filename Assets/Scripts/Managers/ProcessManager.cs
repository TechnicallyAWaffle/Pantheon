using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ProcessManager : MonoBehaviour
{ 

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
