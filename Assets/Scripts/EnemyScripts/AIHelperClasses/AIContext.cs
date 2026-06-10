using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting;
using UnityEditor;

public class AIContext
{
    // own state
    public float localMemoryAvailable;
    public float localCompute;
    public int ownAuthority;
    public List<RunningProcess> ownedRunningProcesses;
    public List<DaemonBase> activeDaemons;

    // opponent state
    public float playerServerMemoryReserved;
    public float playerServerCompute;
    public int playerAuthority;
    public List<RunningProcess> runningPlayerProcesses;
    public List<DaemonBase> activePlayerDaemons;

    // server state
    public float serverMemoryReserved;
    public float serverCompute;

    // derived helpers
    public bool HasGreaterAuthority => playerAuthority > ownAuthority;
    public bool PlayerHasInstaWinProcess;
    public int PlayerTotalProcessThreat => PlayerTotalProcessThreatCalculator();

    public float serverControlRatio => (serverMemoryReserved + serverCompute) 
        / (playerServerMemoryReserved + playerServerCompute);  

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