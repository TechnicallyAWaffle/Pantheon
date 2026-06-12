using UnityEngine;

public class IncServerComputeDaemon : DaemonBase
{
    [SerializeField] float timeBetweenActivations = 12f;
    private float currentTime;

    protected override void Update()
    {
        base.Update();
        if (currentTime <= 0)
        {
            if (isSuspended) return;
            OnTrigger();
            currentTime = timeBetweenActivations;
            owner.RequestServerCompute(1);
        }
        else
            currentTime -= Time.deltaTime;
    }
}
