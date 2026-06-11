using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public static class GameManager
{
    //This tracks every single running process instance by their processID
    public static Dictionary<string, RunningProcess> AllRunningProcessesByID = new();

    //Tracks every single daemon that can be targeted 
    public static Dictionary<string, DaemonBase> AllActiveDaemons = new();

    public static ITargetable FindRunningDaemonOrProcess(string id) //name in daemon's case
    {
        RunningProcess process = AllRunningProcessesByID[id];
        if (process)
            return process;
        else
        { 
            DaemonBase daemon = AllActiveDaemons[id];
            if (daemon)
                return AllActiveDaemons[id];
            else return null;
        }
    }

    public static void KillProcessOrDaemon(ITargetable target)
    {
        if (target is RunningProcess process)
            process.queue.processesToRemove.Add(process);
        else if (target is DaemonBase daemon)
        {
            if (daemon.isRevealed)
                DaemonManager.KillDaemon(daemon);
            else
                //print error
                return;
        }
    }

    public static void SuspendProcessOrDaemon(ITargetable target, Func<bool> condition, RunningProcess suspender)
    {
        if (target is DaemonBase daemon && !daemon.isRevealed)
        { 
            //print error
        }
        ReferenceManager.Instance.suspensionManager.Suspend(target, condition, suspender);
    }

    public static string GenerateRandomID()
    {
        int id = UnityEngine.Random.Range(100, 9000);
        return $"{id:D4}";
    }


}
