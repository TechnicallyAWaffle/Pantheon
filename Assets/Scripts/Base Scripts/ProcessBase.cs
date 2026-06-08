using UnityEditor.U2D.Tooling.Analyzer;
using UnityEngine;

public class ProcessBase : MonoBehaviour
{
    protected ReferenceManager referenceManager;
    public RunningProcess runtimeProcessData;
    
    public bool isExecuted = false;

    private void Awake()
    {
        runtimeProcessData = GetComponent<RunningProcess>();
    }

    protected virtual void Start()
    {
        referenceManager = ReferenceManager.Instance;
        Debug.Log(runtimeProcessData);
    }
    //This is literally just so that we can find Processes somehow without doing exhaustive searches. If we need to add anything else in here feel free -meowlvin
    //Arguments is left blank if the process has no additional arguments. The unique logic of the execute function can just simply not care that they're there
    public virtual void Execute(Entity owner, string[] arguments)
    {
        Debug.Log("After execute: " + runtimeProcessData);
        Debug.Log("After execute: " + runtimeProcessData.data);
        //Some processes have persistent effects after they're executed, some are just one-time effects
        //This flag becomes TRUE once the process completes in the queue. Persistent effects only happen after this flag is TRUE
        Debug.Log("meow2");
        isExecuted = true;
        referenceManager.processManager.KillProcess(runtimeProcessData);
        //if (runtimeProcessData.data.removedWhenExecuted)
          //  referenceManager.processManager.KillProcess(runtimeProcessData);
    }

    public virtual void OnKilled()
    {
        runtimeProcessData.owner.ModifyBusyMemory(runtimeProcessData.queue, -runtimeProcessData.memoryUsed);
    }

    public virtual void OnSuspension()
    {
        
    }

    public virtual void OnSuspensionLifted()
    {
        
    }

    public virtual void Update()
    {
        if (!isExecuted || runtimeProcessData.isSuspended) return;
    }
}
