using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SuspensionManager : MonoBehaviour
{
    //When a process is suspended, a Suspension class instance is created that stores the process its suspending and its lift condition
    //Then it's added to a list of suspended proccesses and ticked every update to check if its lift condition has become true
    private class Suspension
    {
        public RunningProcess processSuspended;
        public DaemonBase daemonSuspended;
        public Func<bool> liftCondition;        // returns true when suspension should lift

        public Suspension(RunningProcess process, Func<bool> condition)
        {
            this.processSuspended = process;
            this.liftCondition = condition;
        }

        public Suspension(DaemonBase daemon, Func<bool> condition)
        {
            this.daemonSuspended = daemon;
            this.liftCondition = condition;
        }

    }

    private List<Suspension> activeSuspensions = new();

    // To suspend a process:
    //Call Suspend(targetProcess, () => liftCondition, source) make sure that the liftCondition is readable as a boolean
    public void Suspend(RunningProcess process, Func<bool> condition, Entity suspender)
    {
        process.isSuspended = true;
        activeSuspensions.Add(new Suspension(process, condition));
        GlobalEventBus.ProcessSuspended(process, suspender);
    }

    public void Suspend(DaemonBase daemon, Func<bool> condition, Entity suspender)
    {
        daemon.isSuspended = true;
        activeSuspensions.Add(new Suspension(daemon, condition));
        //GlobalEventBus.ProcessSuspended(process, suspender);
    }


    void Update()
    {
        for (int i = activeSuspensions.Count - 1; i >= 0; i--)
        {
            //Check if lift condition is now true
            if (activeSuspensions[i].liftCondition())
            {
                //Lift the suspension and then remove the Suspension instance from this list
                Lift(activeSuspensions[i]);
                activeSuspensions.RemoveAt(i);
            }
        }
    }

    void Lift(Suspension suspension)
    {
        if (suspension.processSuspended)
            suspension.processSuspended.isSuspended = false;
        else
            suspension.daemonSuspended.isSuspended = false;
        //If we need something to listen to when a process is resumed we can add a condition for that in the global event bus
        //GlobalEventBus.ProcessResumed(suspension.suspended);
    }

}