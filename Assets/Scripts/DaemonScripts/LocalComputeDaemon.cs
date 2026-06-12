using UnityEngine;

public class LocalComputeDaemon : DaemonBase
{
    [SerializeField] int computeGained = 5;

    private void Start()
    {
        owner.localProcessQueue._openMemory += 5;
    }

    public override void OnSuspension()
    {
        base.OnSuspension();
        owner.localProcessQueue._openCompute -= computeGained;
    }

    public override void OnSuspensionLifted()
    {
        base.OnSuspensionLifted();
        owner.localProcessQueue._openCompute += computeGained;
    }

    public override void OnKilled()
    {
        owner.localProcessQueue._openCompute -= computeGained;
        base.OnKilled();
    }
}
