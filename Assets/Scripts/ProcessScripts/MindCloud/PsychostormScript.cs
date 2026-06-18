using UnityEngine;

public class PsychostormScript : ProcessBase
{
    [SerializeField] private int suspensionDuration = 5;

    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        float endTime = Time.time + suspensionDuration;
        DaemonBase daemon = (DaemonBase)(target);
        referenceManager.suspensionManager.Suspend(daemon, () => Time.time >= endTime, runtimeProcessData);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}