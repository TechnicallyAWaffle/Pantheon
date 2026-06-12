using UnityEditor.U2D.Tooling.Analyzer;
using UnityEngine;

public class ProcessBase : MonoBehaviour
{
    protected ReferenceManager referenceManager;
    public RunningProcess runtimeProcessData;
    
    public bool isExecuted = false;

    private void Awake()
    {
        referenceManager = ReferenceManager.Instance;
    }
    //This is literally just so that we can find Processes somehow without doing exhaustive searches. If we need to add anything else in here feel free -meowlvin
    //Arguments is left blank if the process has no additional arguments. The unique logic of the execute function can just simply not care that they're there
    public virtual void Execute(Entity owner, string[] arguments)
    {
        isExecuted = true;
        if (runtimeProcessData.data.removedWhenExecuted)
            runtimeProcessData.queue.processesToRemove.Add(runtimeProcessData);
    }

    public virtual void OnKilled()
    {
        Debug.Log("Releasing " + runtimeProcessData.memoryUsed + " busy memory");
        runtimeProcessData.owner.RemoveAvailableMemoryMod(runtimeProcessData, runtimeProcessData.queue);
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
