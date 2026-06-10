using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEditor.MPE;
using UnityEditor.Tilemaps;
using UnityEngine;
using static AIAction;

public class Enemy1Script : EnemyBase
{

    protected override AIContext GatherContext()
    {
        return new AIContext
        {
            localMemoryAvailable = self.localProcessQueue._openMemory,
            ownAuthority = self.authority,
            ownedRunningProcesses = self.ownedProcesses.ToArray(),
            playerAuthority = player.authority,
            playerServerMemoryReserved = player.reservedServerMemory,
            playerServerCompute = player.reservedServerCompute,
            activeDaemons = self.daemons.ToArray(),
            activePlayerDaemons = player.daemons.ToArray(),
            runningPlayerProcesses = player.ownedProcesses.ToArray(),
            serverMemoryReserved = self.reservedServerMemory - self.busyServerMemory,
        };
    }

    protected override AIAction Decide(AIContext ctx)
    {
        if (ctx.serverControlRatio < minimumTolerableServerControlRatio)
        {
            //Trump card goes here
            return null;
        }


        if (ctx.PlayerTotalProcessThreat > tolerablePlayerProcessThreatSum)
        {
            return DecideDefensiveAction(ctx);
        }

        if (ctx.serverControlRatio >= 1)
            return DecideOffensiveAction(ctx);
        else if (ctx.serverControlRatio <= 1)
            return DecideResourceAction(ctx);

        if (ctx.PlayerHasInstaWinProcess)
            return null;

        return DecideDefaultAction(ctx);
    }

    AIAction DecideDefensiveAction(AIContext ctx)
    {

        RunningProcess highestThreatProcess = FindHighestThreatProcess(ctx.runningPlayerProcesses);

        if (CheckAuthorityAgainstEncryption(highestThreatProcess))
            return AIAction.KillProcess(highestThreatProcess);

        foreach (SOProcessData process in defensiveProcesses)
        {
            ProcessQueue queueToRun = TryLocalOrServerRun(process.memoryUsage);
            if (queueToRun)
            {
                if (process.arguments.Length == 0)
                    return AIAction.RunProcess(SOProcessDataToArgsArray(process, string.Empty), queueToRun);
                else
                    return AIAction.RunProcess(SOProcessDataToArgsArray(process, highestThreatProcess.processID), queueToRun);
            }
        }

        return DecideOffensiveAction(ctx);
    }

    AIAction DecideOffensiveAction(AIContext ctx)
    {



        return null;
    }

    AIAction DecideResourceAction(AIContext ctx)
    {
        return null;
    }

    AIAction DecideDefaultAction(AIContext ctx) => AIAction.Wait();

    protected override void Execute(AIAction action, AIContext ctx)
    {
        switch (action.type)
        {
            case AIActionType.RunProcess:
                referenceManager.processManager.TryRunProcess(action.arguments, self, action.queue, action.isServer);
                break;

            case AIActionType.KillProcess:
                string[] args = { "kill", action.target.processID };
                referenceManager.processManager.TryRunProcess(args, self, action.queue, action.isServer);
                break;
            case AIActionType.Wait:
                break;
        }
    }


}
