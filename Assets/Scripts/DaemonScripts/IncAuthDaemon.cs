using UnityEngine;

public class IncAuthDaemon : DaemonBase
{
    [SerializeField] float timeBetweenActivations = 45f;
    private float currentTime;

    protected override void Update()
    {
        base.Update();
        if (currentTime <= 0)
        {
            if (!OnTrigger()) return;
            currentTime = timeBetweenActivations;
            if(owner.authority < 4)
                owner.authority++;
        }
        else
            currentTime -= Time.deltaTime;
    }
}
