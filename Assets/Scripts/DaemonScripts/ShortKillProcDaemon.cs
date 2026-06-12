using UnityEngine;

public class ShortKillProcDaemon : DaemonBase
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GlobalEventBus.OnBaseKillProcessQueued += ShortenKillProcessExecutionTime;
    }

    private void ShortenKillProcessExecutionTime(RunningProcess killProcess)
    {
        if (isSuspended) return;
        OnTrigger();
        if (killProcess.owner == owner)
            killProcess.timeRemaining = Mathf.Clamp(killProcess.timeRemaining - 5, 0, Mathf.Infinity);
    }
}
