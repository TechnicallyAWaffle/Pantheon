using UnityEditor.U2D.Tooling.Analyzer;
using UnityEngine;

public class ProcessBase : MonoBehaviour
{
    ReferenceManager referenceManager;
    RunningProcess runtimeProcessData;
    
    public bool isExecuted = false;

    private void Start()
    {
        referenceManager = ReferenceManager.Instance;
    }
    //This is literally just so that we can find Processes somehow without doing exhaustive searches. If we need to add anything else in here feel free -meowlvin
    //Arguments is left blank if the process has no additional arguments. The unique logic of the execute function can just simply not care that they're there
    public virtual void Execute(Entity owner, string[] arguments)
    {
        //Some processes have persistent effects after they're executed, some are just one-time effects
        //This flag becomes TRUE once the process completes in the queue. Persistent effects only happen after this flag is TRUE
        isExecuted = true;
    }

    public virtual void Update()
    {
        if (!isExecuted || runtimeProcessData.isSuspended) return;
    }
}
