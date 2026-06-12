using UnityEngine;

public class LogicbombScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        DaemonBase daemon = (DaemonBase)GameManager.FindRunningDaemonOrProcess(arguments[0]);
        EncryptionManager.AddEncryption(daemon, -1);
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}