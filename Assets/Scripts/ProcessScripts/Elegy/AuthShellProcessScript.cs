using UnityEngine;

public class AuthShellProcessScript : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        DaemonBase lastDaemon = owner.daemons[0];
        DaemonBase lowestEncryptionDaemon = lastDaemon;
        foreach (DaemonBase daemon in owner.daemons)
        { 
            if(daemon.Encryption < lastDaemon.Encryption)
                lowestEncryptionDaemon = daemon;
        }
        EncryptionManager.AddEncryption(lowestEncryptionDaemon, 1);
    }
}
