using UnityEngine;

public class SuspendProcess : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        ITargetable targetToSuspend = GameManager.AllRunningProcessesByID[arguments[0]];

        if (owner.authority > targetToSuspend.Encryption)
            GameManager.SuspendProcessOrDaemon(targetToSuspend, () => false, runtimeProcessData);
        else
            referenceManager.terminalUIManager.Print("AUTHORIZATION ERROR: Access to process decryption hash denied");
        base.Execute(owner, arguments);
    }
}
