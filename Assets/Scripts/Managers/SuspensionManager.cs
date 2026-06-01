using System;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionManager : MonoBehaviour
{
    //When a process is suspended, a Suspension class instance is created that stores the process its suspending and its lift condition
    //Then it's added to a list of suspended proccesses and ticked every update to check if its lift condition has become true
    private class Suspension
    {
        public RunningProcess processSuspended;
        public Func<bool> liftCondition;        // returns true when suspension should lift

        public Suspension(RunningProcess process, Func<bool> condition, Entity suspender)
        {
            this.processSuspended = process;
            this.liftCondition = condition;
        }
    }

    private List<Suspension> activeSuspensions = new();

    // To suspend a process:
    //Call Suspend(targetProcess, () => liftCondition, source) make sure that the liftCondition is readable as a boolean
    public void Suspend(RunningProcess process, Func<bool> condition, Entity suspender)
    {
        process.isSuspended = true;
        activeSuspensions.Add(new Suspension(process, condition, suspender));
        GlobalEventBus.ProcessSuspended(process, suspender);
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
        suspension.processSuspended.isSuspended = false;
        
        //If we need something to listen to when a process is resumed we can add a condition for that in the global event bus
        //GlobalEventBus.ProcessResumed(suspension.suspended);
    }

}