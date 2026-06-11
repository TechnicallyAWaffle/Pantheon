using UnityEngine;

public class GateCrashProcessScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        ITargetable target = GameManager.FindRunningDaemonOrProcess(arguments[0]);

        EncryptionManager.AddEncryption(target, -3);
        base.Execute(owner, arguments);
    }
}