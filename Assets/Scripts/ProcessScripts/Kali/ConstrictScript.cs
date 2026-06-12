using System.Collections;
using UnityEngine;

public class ConstrictScript : ProcessBase
{

    [SerializeField] private int delayBetweenSuspensions = 4;
    [SerializeField] private int suspensionDuration = 1;


    public override void Execute(Entity owner, string[] arguments)
    {
        ITargetable target = GameManager.FindRunningDaemonOrProcess(arguments[0]);

        StartCoroutine(PeriodicSuspensionHelper(target));

        base.Execute(owner, arguments);
    }

    private IEnumerator PeriodicSuspensionHelper(ITargetable target)
    {
        if (runtimeProcessData.isSuspended) yield return new WaitUntil(() => !runtimeProcessData.isSuspended);

        float endTime = Time.time + suspensionDuration;
        referenceManager.suspensionManager.Suspend(target, () => Time.time >= endTime, runtimeProcessData);

        yield return new WaitForSeconds(delayBetweenSuspensions);
    }


    public override void OnKilled()
    {
        base.OnKilled();
    }
}