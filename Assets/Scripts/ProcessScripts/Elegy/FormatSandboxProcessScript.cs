using System.Collections;
using UnityEngine;

public class FormatSandboxProcessScript : ProcessBase
{
    [SerializeField] float suspensionDuration = 15;
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        float endTime = Time.time + suspensionDuration;
        referenceManager.suspensionManager.Suspend(target, () => Time.time >= endTime, runtimeProcessData);
    }
}
