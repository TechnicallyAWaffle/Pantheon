using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEditor.MPE;
using UnityEditor.Tilemaps;
using UnityEngine;
using static AIAction;

/*
 * 
RESOURCE ACTIONS
runs acquisition

DEFENSIVE ACTIONS

Check highest encryption process and see if you can run gatecrash


OFFENSIVE ACTIONS
Check if you can kill any process with vindicate
Excise if you can't 


Luxury Actions
Runs authshell on the lowest encryption daemon it has if one of them is at or lower than 1
runs formatsandbox 
 
 
*/

public class Enemy1Script : EnemyBase
{

    private bool usedHailMary = false;


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
        AIAction finalDecision = null;

        if (ctx.serverControlRatio < minimumTolerableServerControlRatio && self.daemons.Count < 3 && !usedHailMary)
        {
            //Check if hail mary exists
            usedHailMary = true;
            //hail mary 
            return null;
        }


        if (ctx.PlayerTotalProcessThreat > tolerablePlayerProcessThreatSum)
        {
            finalDecision = DecideDefensiveAction(ctx);
        }

        if (ctx.serverControlRatio >= 1)
            finalDecision = DecideOffensiveAction(ctx);
        else if (ctx.serverControlRatio <= 1)
            finalDecision = DecideResourceAction(ctx);

        if (ctx.PlayerHasInstaWinProcess)
            return null;

        finalDecision = DecideDefaultAction(ctx);
        return finalDecision;
    }

    AIAction DecideDefensiveAction(AIContext ctx)
    {
        /*
        Checks to see if it can kill any process
        Checks for the highest priority process and sees if it can suspend it with sandbox
        Checks for a process with >3 encryption that it can gatecrash (lower encryption)
         */


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
        return null;
    }

    AIAction DecideOffensiveAction(AIContext ctx)
    {
        /*
        Checks if it can kill any Daemon
        Checks if it can vindicate anything, 50% chance to skip, lowered by existing vindicate programs
        Checks if it can excise anything 
         */


        return null;
    }

    AIAction DecideResourceAction(AIContext ctx)
    {
        /*
        Run acquisition
         */
        return null;
    }

    AIAction DecideDefaultAction(AIContext ctx)
    {
        /*
        Run authshell to raise daemon encryption
        Do nothing
        */


        return null;
    }

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
