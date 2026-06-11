using UnityEngine;

public class KillProcess : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        ITargetable targetToKill = GameManager.FindRunningDaemonOrProcess(arguments[0]);
        if (owner.authority > targetToKill.Encryption)
            GameManager.KillProcessOrDaemon(targetToKill);
        else
            referenceManager.terminalUIManager.Print("AUTHORIZATION ERROR: Access to process decryption hash denied");
        base.Execute(owner, arguments);
    }
}
