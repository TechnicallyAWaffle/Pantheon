using UnityEngine;

public class LocalMemoryDaemon : DaemonBase
{
    [SerializeField] int memoryGained = 5;

    private void Start()
    {
        owner.localProcessQueue._openMemory += 5;
    }

    public override void OnSuspension()
    {
        base.OnSuspension();
        owner.localProcessQueue._openMemory -= memoryGained;
    }

    public override void OnSuspensionLifted()
    {
        base.OnSuspensionLifted();
        owner.localProcessQueue._openMemory += memoryGained;
    }

    public override void OnKilled()
    {
        owner.localProcessQueue._openMemory -= memoryGained;
        base.OnKilled();
    }


}
