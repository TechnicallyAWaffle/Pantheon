using UnityEngine;

public class IncServerMemoryDaemon : DaemonBase
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
            owner.RequestServerMemory(1);
        }
        else
            currentTime -= Time.deltaTime;
    }
}
