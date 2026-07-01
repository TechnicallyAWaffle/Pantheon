using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting;
using UnityEditor;

public class AIContext
{
    // own state
    public Entity self;
    public float localMemoryAvailable;
    public float localCompute;
    public int ownAuthority;
    public RunningProcess[] ownedRunningProcesses;
    public DaemonBase[] activeDaemons;

    // opponent state
    public float playerServerMemoryReserved;
    public float playerServerCompute;
    public int playerAuthority;
    public RunningProcess[] runningPlayerProcesses;
    public DaemonBase[] activePlayerDaemons;

    // server state
    public float serverMemoryReserved;
    public float serverCompute;
    public List<SuspensionManager.Suspension> activeSuspensions;

    // derived helpers
    public bool HasGreaterAuthority => playerAuthority > ownAuthority;
    public bool PlayerHasInstaWinProcess;
    public int PlayerTotalProcessThreat => PlayerTotalProcessThreatCalculator();

    public List<RunningProcess> OwnedSuspendedProcesses => FindOwnedSuspendedProcesses();

    public float serverControlRatio => (serverMemoryReserved + serverCompute) 
        / (playerServerMemoryReserved + playerServerCompute);

    private List<RunningProcess> FindOwnedSuspendedProcesses()
    {
        List<RunningProcess> temp = new();
        foreach (SuspensionManager.Suspension suspension in SuspensionManager.ActiveSuspensions)
        {
            if (suspension.processSuspended.owner == self)
            {
                temp.Add(suspension.processSuspended);
            }
        }
        return temp;
    }

    
    private int PlayerTotalProcessThreatCalculator()
    {
        int threatLevelSum = 0;
        foreach (RunningProcess process in runningPlayerProcesses)
        {
            threatLevelSum += process.data.threatLevel;
            if (process.data.threatLevel == 10)
                PlayerHasInstaWinProcess = true;
        }
        return threatLevelSum;
    }

}