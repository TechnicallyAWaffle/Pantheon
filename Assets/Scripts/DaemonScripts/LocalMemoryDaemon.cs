using UnityEngine;

public class LocalMemoryDaemon : DaemonBase
{
    [SerializeField] int memoryGained = 5;

    private void Start()
    {
        owner.ChangeOpenLocalMemory(memoryGained);
    }

    public override void OnSuspension()
    {
        base.OnSuspension();
        owner.ChangeOpenLocalMemory(-memoryGained);
    }

    public override void OnSuspensionLifted()
    {
        base.OnSuspensionLifted();
        owner.ChangeOpenLocalMemory(memoryGained);
    }

    public override void OnKilled()
    {
        owner.ChangeOpenLocalMemory(-memoryGained);
        base.OnKilled();
    }


}
