using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.DedicatedServer;

public class ProcessBase : MonoBehaviour
{
    protected ReferenceManager referenceManager;
    public RunningProcess runtimeProcessData;
    protected ITargetable target;
    
    public bool isExecuted = false;

    private void Awake()
    {
        referenceManager = ReferenceManager.Instance;
    }
    //This is literally just so that we can find Processes somehow without doing exhaustive searches. If we need to add anything else in here feel free -meowlvin
    //Arguments is left blank if the process has no additional arguments. The unique logic of the execute function can just simply not care that they're there
    public void Execute(Entity owner, string[] arguments)
    {
        isExecuted = true;
        if(PreExecute(arguments))
            ExecuteAction(owner, arguments);
        PostExecute();
    }

    protected virtual bool PreExecute(string[] arguments)
    {
        if (arguments.Length == 0)
            return true;
        target = GameManager.FindRunningDaemonOrProcess(arguments[0]);
        if (target == null)
            return false;
        return true;
    }

    protected virtual void PostExecute()
    {
        if (runtimeProcessData.data.removedWhenExecuted)
        {
            GameManager.KillProcessOrDaemon(runtimeProcessData);
        }
    }



    protected virtual void ExecuteAction(Entity owner, string[] arguments)
    { }



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

    //Helpers

    private void WriteDebug(string message)
    {
        UnityEngine.Debug.Log("<color=#42adf5>Process " + runtimeProcessData.data.processName + ": " + message);
    }

}
