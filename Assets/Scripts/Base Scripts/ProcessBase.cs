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
        {
            GameManager.KillProcessOrDaemon(runtimeProcessData);
        }
    }

    public virtual void OnKilled()
    {
        WriteDebug("Releasing " + runtimeProcessData.memoryUsed + " busy memory to " + runtimeProcessData.owner);
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

    private void WriteDebug(string message)
    {
        UnityEngine.Debug.Log("<color=#d5ebc5>Process " + runtimeProcessData.data.processName + ": " + message);
    }

}
