using System;
using System.Linq;
using UnityEditor.MPE;
using UnityEditor.Tilemaps;
using UnityEngine;
using static AIAction;

public class Enemy1Script : EnemyBase
{
    private void Awake()
    {
        enemyName = "ELEGY";
    }

    protected override AIContext GatherContext()
    {
        return new AIContext
        {
            localMemoryAvailable = self.localProcessQueue._openMemory,
            ownAuthority = self.authority,
            ownedRunningProcesses = self.ownedProcesses,
            playerAuthority = player.authority,
            playerServerMemoryReserved = player.reservedServerMemory,
            playerServerCompute = player.reservedServerCompute,
            activeDaemons = self.daemons,
            activePlayerDaemons = player.daemons,
            runningPlayerProcesses = player.ownedProcesses,
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
                //referenceManager.processManager.TryRunProcess();
                break;

            case AIActionType.KillProcess:
                //referenceManager.processManager(action.target); TODO: Add default kill process
                break;

            case AIActionType.Wait:
                break;
        }
    }


}
