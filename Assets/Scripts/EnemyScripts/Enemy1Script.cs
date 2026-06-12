using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEditor.Tilemaps;
using UnityEngine;
using static AIAction;

/*
HAIL MARY
If I'm cooked, run hailmary

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
    [SerializeField] private DaemonBase hailMaryDaemon;

    [Header("Processes")]
    [SerializeField] SOProcessData vindicatorProcess;
    [SerializeField] SOProcessData hailmaryProcess;
    [SerializeField] SOProcessData gatecrashProcess;
    [SerializeField] SOProcessData formatsandboxProcess;
    [SerializeField] SOProcessData exciseProcess;
    [SerializeField] SOProcessData authshellProcess;
    [SerializeField] SOProcessData acquisitionProcess;

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
            serverMemoryReserved = self.reservedServerMemory,
        };
    }
    
    protected override AIAction Decide(AIContext ctx)
    {
        //Decide if I just want to sit around and twiddle my thumbs
        float decideIfIWantToDoNothing = UnityEngine.Random.Range(0, 10);
        if (decideIfIWantToDoNothing > aggression)
        {
            reasonForWait = "I am a peace loving citizen";
            return AIAction.Wait();
        }
        

        AIAction finalDecision = null;

        if (ctx.serverControlRatio < minimumTolerableServerControlRatio && self.daemons.Count < 3 && !usedHailMary)
        {
            if (!hailMaryDaemon.isSuspended)
            {
                usedHailMary = true;
                return TryRunEnemyProcess(hailmaryProcess, null);
            }
        }

        if (ctx.PlayerTotalProcessThreat > tolerablePlayerProcessThreatSum)
        {
            finalDecision = DecideDefensiveAction(ctx);
        }

        if (ctx.serverControlRatio >= 1)
            finalDecision = DecideAttackAction(ctx);
        else if (ctx.serverControlRatio <= 1)
            finalDecision = DecideResourceAction(ctx);

        finalDecision ??= DecideDefaultAction(ctx); //bro intellisense you gotta stop recommending me syntax from the moon wtf is this :wiltedrose: :wiltedrose:

        return finalDecision;
    }

    AIAction DecideDefensiveAction(AIContext ctx)
    {
        //Checks to see if it can kill any process
        //Also pre-warms encryption >= 3 check for later use
        RunningProcess highEncryptionProcess = null;
        foreach (RunningProcess process in ctx.runningPlayerProcesses)
        {
            if (CheckAuthorityAgainstEncryption(process))
                return AIAction.KillProcess(process);
            if(process.Encryption >= 3 && highEncryptionProcess == null)
                highEncryptionProcess = process;
        }

        //Checks for the highest priority process and sees if it can suspend it with sandbox
        ProcessQueue queueToRun = TryLocalOrServerRun(formatsandboxProcess.memoryUsage);
        if (queueToRun)
            return AIAction.RunProcess(SOProcessDataToArgsArray(formatsandboxProcess), queueToRun);

        //Checks for a process with >3 encryption that it can gatecrash to lower its encryption
        queueToRun = TryLocalOrServerRun(gatecrashProcess.memoryUsage);
        if (queueToRun && highEncryptionProcess)
            return AIAction.RunProcess(SOProcessDataToArgsArray(gatecrashProcess, highEncryptionProcess.processID), queueToRun);

        //No good option found: Move on 
        return null;
    }

    AIAction DecideAttackAction(AIContext ctx)
    {
        /*
        
        
        Checks if it can excise anything 
         */

        //Checks if it can kill any Daemon
        foreach (DaemonBase daemon in ctx.activePlayerDaemons)
        {
            if (CheckAuthorityAgainstEncryption(daemon))
                return AIAction.KillDaemon(daemon);
        }


        //Coinflip for excise vs vindicator process run. Add weight in favor of vindicator if existing vindicator processes are running
        float weightedCoin = 0.5f;
        foreach (RunningProcess process in ctx.ownedRunningProcesses)
        {
            if (process.data.processName == "vindicator")
                weightedCoin += 0.1f;
        }

        
        if (UnityEngine.Random.Range(0f, 1f) > weightedCoin)
        { 
            DaemonBase daemon = ctx.activePlayerDaemons[UnityEngine.Random.Range(0, ctx.activePlayerDaemons.Count())];
            AIAction exciseAction = TryRunEnemyProcess(exciseProcess, daemon);
            if (exciseAction != null)
            {
                reasonForWait = "No memory to run vindicator";
                return exciseAction;
            }
        }

        //If excise didn't run, run vindicator
        if (ctx.runningPlayerProcesses.Count() == 0)
            return null;
        RunningProcess lastProcess = ctx.runningPlayerProcesses[0];
        RunningProcess lowestEncryptionProcess = lastProcess;
        foreach (RunningProcess process in ctx.runningPlayerProcesses)
        {
            if (process.Encryption < lastProcess.Encryption)
            {
                lastProcess = process;
                lowestEncryptionProcess = process;
            }   
        }

        AIAction vindicatorAction = TryRunEnemyProcess(vindicatorProcess, lowestEncryptionProcess);
        if (vindicatorAction != null)
        {
            reasonForWait = "No memory to run vindicator";
            return vindicatorAction;
        }
        return null;
    }

    AIAction DecideResourceAction(AIContext ctx)
    {
        //Run acquisition if possible
        AIAction acquisitionAction = TryRunEnemyProcess(acquisitionProcess, null);
        if (acquisitionAction != null)
        {
            reasonForWait = "No memory to run acquisition";
            return acquisitionAction;
        }

        return null;
    }

    AIAction DecideDefaultAction(AIContext ctx)
    {

        //Run authshell to raise daemon encryption (finding the right target is handled by the process itself)
        AIAction authShellAction = TryRunEnemyProcess(authshellProcess, null);
        if (authShellAction != null)
            return authShellAction;

        //If you really can't do anything, wait
        return AIAction.Wait();
    }

    protected override void Execute(AIAction action, AIContext ctx)
    {
        WriteDebug("Executing action type " + action.type);
        switch (action.type)
        {
            case AIActionType.RunProcess:
                referenceManager.processManager.TryRunProcess(action.arguments, self, action.queue, action.isServer);
                break;

            case AIActionType.KillProcess:
                string[] pArgs = { "kill", (action.target as RunningProcess).processID }; //TODO: Later make this less of a hardcoded call to the string and actually store base functions
                referenceManager.processManager.TryRunProcess(pArgs, self, action.queue, action.isServer);
                break;
            case AIActionType.KillDaemon:
                string[] dArgs = { "kill", (action.target as DaemonBase).daemonName};  //TODO: Later make this less of a hardcoded call to the string and actually store base functions
                referenceManager.processManager.TryRunProcess(dArgs, self, action.queue, action.isServer);
                break;
            case AIActionType.Wait:
                WriteDebug("Reason For Wait: " + reasonForWait);
                break;
        }
    }

}

