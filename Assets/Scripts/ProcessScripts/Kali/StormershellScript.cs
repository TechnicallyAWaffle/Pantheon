using UnityEngine;

public class StormershellScript : ProcessBase
{
    [SerializeField] float suspensionDuration = 5;
    public override void Execute(Entity owner, string[] arguments)
    {

        ITargetable target = GameManager.FindRunningDaemonOrProcess(arguments[0]);
        if (target.Encryption <= 0)
            GameManager.KillProcessOrDaemon(target);
        else
        {
            float endTime = Time.time + suspensionDuration;
            EncryptionManager.AddEncryption(target, -1);
            referenceManager.suspensionManager.Suspend(target, () => Time.time >= endTime, runtimeProcessData);
        }

        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}