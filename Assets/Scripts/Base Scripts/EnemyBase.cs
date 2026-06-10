using NUnit.Framework;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public abstract class EnemyBase : MonoBehaviour
{
    protected Entity self;
    protected Entity player;
    protected ReferenceManager referenceManager;

    public string enemyName;
    [Tooltip("The limit of the sum of player process threat levels until something drastic must be done")]
    [SerializeField] protected int tolerablePlayerProcessThreatSum = 15;
    [Tooltip("The minimum threshold of the ratio of server control (0 - 2) until something must be done")]
    [SerializeField] protected float minimumTolerableServerControlRatio = 0.2f;
    [SerializeField] protected float tickRate = 1f;
    private float tickTimer;

    [SerializeField] protected SOProcessData[] offensiveProcesses;
    [SerializeField] protected SOProcessData[] defensiveProcesses;
    [SerializeField] protected SOProcessData[] resourceProcesses;

    protected abstract AIContext GatherContext();
    protected abstract AIAction Decide(AIContext ctx);
    protected abstract void Execute(AIAction action, AIContext ctx);

    protected virtual void Start()
    {
        referenceManager = ReferenceManager.Instance;
        self = referenceManager.opponent;
        player = referenceManager.player;
    }

    protected virtual void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickRate)
        {
            tickTimer = 0f;
            Tick();
        }
    }

    protected virtual void Tick()
    {
        AIContext ctx = GatherContext();
        AIAction action = Decide(ctx);
        Execute(action, ctx);
    }

    //Helpers
    protected bool CheckAuthorityAgainstEncryption(RunningProcess process)
    {
        return self.authority > process.encryption;
    }

    protected ProcessQueue TryLocalOrServerRun(float memoryUsage)
    {
        if (memoryUsage > self.localProcessQueue.openMemory)
            return self.localProcessQueue;
        if (memoryUsage > self.reservedServerMemory)
            return null;
        else
            return referenceManager.serverProcessQueue;
    }

    protected RunningProcess FindHighestThreatProcess(RunningProcess[] processes)
    {
        RunningProcess highestThreatProcess = null;
        RunningProcess lastProcess = processes[0];
        foreach (RunningProcess process in processes)
        {
            if (process.data.threatLevel > lastProcess.data.threatLevel)
                highestThreatProcess = process;
        }
        return highestThreatProcess;
    }


    protected string[] SOProcessDataToArgsArray(SOProcessData processData, string processOrDaemonID)
    {
        return new string[] { processData.processName, processOrDaemonID };
    }
}