using System;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionManager : MonoBehaviour
{
    //When a process is suspended, a Suspension class instance is created that stores the process its suspending and its lift condition
    //Then it's added to a list of suspended proccesses and ticked every update to check if its lift condition has become true
    public class Suspension
    {
        public RunningProcess suspender;
        public RunningProcess processSuspended;
        public DaemonBase daemonSuspended;
        public Func<bool> liftCondition;        // returns true when suspension should lift

        public Suspension(RunningProcess process, Func<bool> condition, RunningProcess suspender)
        {
            this.suspender = suspender;
            this.processSuspended = process;
            this.liftCondition = condition;
        }

        public Suspension(DaemonBase daemon, Func<bool> condition, RunningProcess suspender)
        {
            this.suspender = suspender;
            this.daemonSuspended = daemon;
            this.liftCondition = condition;
        }

    }

    public static List<Suspension> ActiveSuspensions = new();
    private static List<Suspension> SuspendedSuspensions = new();

    // To suspend a process:
    //Call Suspend(targetProcess, () => liftCondition, source) make sure that the liftCondition is readable as a boolean
    public void Suspend(ITargetable target, Func<bool> condition, RunningProcess suspender)
    {
        if (target is RunningProcess process)
        {
            WriteDebug("Process suspension started: " + suspender.data.processName + " is suspending " + process.data.processName);
            process.isSuspended = true;
            ActiveSuspensions.Add(new Suspension(process, condition, suspender));
            process.GetComponent<ProcessBase>().OnSuspension();
            GlobalEventBus.ProcessSuspended(process, suspender);
        }
        else if (target is DaemonBase daemon)
        {
            WriteDebug("Daemon suspension started: " + suspender.data.processName + " is suspending " + daemon.daemonName);
            daemon.isSuspended = true;
            ActiveSuspensions.Add(new Suspension(daemon, condition, suspender));
            daemon.OnSuspension();
            //GlobalEventBus.ProcessSuspended(, suspender); TODO: Replace with DaemonSuspended global event
        }

    }


    void Update()
    {

        if (ActiveSuspensions.Count == 0) return;

        for (int i = ActiveSuspensions.Count - 1; i >= 0; i--)
        {
            Suspension suspension = ActiveSuspensions[i];
            WriteDebug("Suspender is: " + suspension.suspender);

            //Check if the process/daemon being suspended even still exists.
            //If it doesn't, lift the suspension and kill the suspender
            if (!suspension.processSuspended && !suspension.daemonSuspended)
            {
                GameManager.KillProcessOrDaemon(suspension.suspender);
                Lift(suspension);
                return;
            }

            if (!suspension.suspender || suspension.liftCondition())
            {
                Lift(suspension);
                break;
            }
            //If the process suspending this one is suspended, lift its suspension but store it in a special list
            if (suspension.suspender.isSuspended)
            {
                Lift(suspension);
                SuspendedSuspensions.Add(suspension);
            }
        }

        //For each suspended process that is currently paused due to the suspender being suspended, check if they're
        //still running or suspended. If the process is no longer suspended, re-suspend its target.
        List<Suspension> pausedSuspensions = new();
        foreach (Suspension suspension in SuspendedSuspensions)
        {
            if (!suspension.suspender)
                break;
            if (suspension.suspender.isSuspended)
            {
                pausedSuspensions.Add(suspension);
            }
            else
                Suspend(suspension.processSuspended, suspension.liftCondition, suspension.suspender);
        }
        SuspendedSuspensions = pausedSuspensions;

    }

    void Lift(Suspension suspension)
    {
        ActiveSuspensions.Remove(suspension);
        //Add OnSuspended call to the suspended process here
        if (suspension.processSuspended)
        {
            WriteDebug("Lifting process suspension: " + suspension.processSuspended.data.processName);
            suspension.processSuspended.isSuspended = false;
            suspension.processSuspended.GetComponent<ProcessBase>().OnSuspensionLifted();
        }
        else
        {
            WriteDebug("Lifting daemon suspension: " + suspension.daemonSuspended.daemonName);
            suspension.daemonSuspended.isSuspended = false;
            suspension.daemonSuspended.OnSuspensionLifted();
        }
        //If we need something to listen to when a process is resumed we can add a condition for that in the global event bus
        //GlobalEventBus.ProcessResumed(suspension.suspended);
    }

    protected void WriteDebug(string message)
    {
        UnityEngine.Debug.Log("<color=#D469F5> Suspension Manager: " + message);
    }

}