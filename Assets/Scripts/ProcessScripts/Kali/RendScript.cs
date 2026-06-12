using UnityEngine;

public class RendScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        ITargetable target = GameManager.FindRunningDaemonOrProcess(arguments[0]);
        EncryptionManager.AddEncryption(target, -1);

        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}