using UnityEngine;

public class StormershellScript : ProcessBase
{
    [SerializeField] float suspensionDuration = 5;
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        if (target.Encryption <= 0)
            GameManager.KillProcessOrDaemon(target);
        else
        {
            float endTime = Time.time + suspensionDuration;
            EncryptionManager.AddEncryption(target, -1);
            referenceManager.suspensionManager.Suspend(target, () => Time.time >= endTime, runtimeProcessData);
        }

    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}