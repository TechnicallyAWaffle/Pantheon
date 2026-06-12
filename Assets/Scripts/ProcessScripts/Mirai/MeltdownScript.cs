using UnityEngine;

public class MeltdownScript : ProcessBase
{

    [SerializeField] private int suspensionDuration = 10;
    public override void Execute(Entity owner, string[] arguments)
    {
        SuspensionManager suspensionManager = referenceManager.suspensionManager;
        Entity target = referenceManager.GetOtherEntity(owner);

        if (target.daemons.Count == 0)
            return;

        DaemonBase lastDaemon = target.daemons[0];
        DaemonBase highestEncryptionDaemon = lastDaemon;
        foreach (DaemonBase daemon in target.daemons)
        {

            if (daemon.Encryption > lastDaemon.Encryption)
            {
                float endTime = Time.time + suspensionDuration;
                suspensionManager.Suspend(daemon, () => Time.time >= endTime, runtimeProcessData);
                lastDaemon = daemon;
                highestEncryptionDaemon = daemon;
            }
        }

        GameManager.KillProcessOrDaemon(highestEncryptionDaemon);

        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}