using UnityEngine;

public class SuspendProcess : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        if (owner.authority > target.Encryption || owner == target.Owner)
            GameManager.SuspendProcessOrDaemon(target, () => false, runtimeProcessData);
        else
            referenceManager.terminalUIManager.Print("AUTHORIZATION ERROR: Access to process decryption hash denied");
    }
}
