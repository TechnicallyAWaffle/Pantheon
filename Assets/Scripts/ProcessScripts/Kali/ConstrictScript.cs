using System.Collections;
using UnityEngine;

public class ConstrictScript : ProcessBase
{

    [SerializeField] private int delayBetweenSuspensions = 3;
    [SerializeField] private int suspensionDuration = 1;


    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        StartCoroutine(PeriodicSuspensionHelper(target));
    }

    private IEnumerator PeriodicSuspensionHelper(ITargetable target)
    {
        while (true)
        {
            if (runtimeProcessData.isSuspended) yield return new WaitUntil(() => !runtimeProcessData.isSuspended);
            if (target == null)
            {
                StopAllCoroutines();
                GameManager.KillProcessOrDaemon(runtimeProcessData);
            }
            float endTime = Time.time + suspensionDuration;
            referenceManager.suspensionManager.Suspend(target, () => Time.time >= endTime, runtimeProcessData);

            yield return new WaitForSeconds(delayBetweenSuspensions);
        }
    }


    public override void OnKilled()
    {
        base.OnKilled();
    }
}