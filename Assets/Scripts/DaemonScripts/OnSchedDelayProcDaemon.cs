using UnityEngine;

public class OnSchedDelayProcDaemon : DaemonBase
{
    private void Start()
    {
        GlobalEventBus.OnSchedulerReset += SchedulerReset;
    }

    private void SchedulerReset()
    {
        if (isSuspended) return;
        OnTrigger();
        ProcessQueue serverQueue = ReferenceManager.Instance.serverProcessQueue;
        Entity player = ReferenceManager.Instance.player;
        foreach (RunningProcess process in serverQueue.queue)
        {
            if (process.owner == player)
                process.timeRemaining += 2;
        }
    }
}
