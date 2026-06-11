using UnityEngine;

public class AuthShellProcessScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        DaemonBase lowestEncryptionDaemon = null;
        DaemonBase lastDaemon = owner.daemons[0];
        foreach (DaemonBase daemon in owner.daemons)
        { 
            if(daemon.Encryption < lastDaemon.Encryption)
                lowestEncryptionDaemon = daemon;
        }
        EncryptionManager.AddEncryption(lowestEncryptionDaemon, 1);

        base.Execute(owner, arguments);
    }
}
