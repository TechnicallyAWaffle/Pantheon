using UnityEngine;

public class PsychostormScript : ProcessBase
{
    [SerializeField] private int suspensionDuration = 5;

    public override void Execute(Entity owner, string[] arguments)
    {
        float endTime = Time.time + suspensionDuration;
        DaemonBase daemon = (DaemonBase)GameManager.FindRunningDaemonOrProcess(arguments[0]);
        referenceManager.suspensionManager.Suspend(daemon, () => Time.time >= endTime, runtimeProcessData);
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}