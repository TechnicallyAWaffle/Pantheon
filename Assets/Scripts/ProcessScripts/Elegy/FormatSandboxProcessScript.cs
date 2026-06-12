using System.Collections;
using UnityEngine;

public class FormatSandboxProcessScript : ProcessBase
{
    [SerializeField] float suspensionDuration = 15;
    public override void Execute(Entity owner, string[] arguments)
    {
        float endTime = Time.time + suspensionDuration;
        ITargetable target = GameManager.FindRunningDaemonOrProcess(arguments[0]);
        referenceManager.suspensionManager.Suspend(target, () => Time.time >= endTime, runtimeProcessData);

        base.Execute(owner, arguments);
    }
}
