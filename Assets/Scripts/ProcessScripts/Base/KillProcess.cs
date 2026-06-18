using UnityEngine;

public class KillProcess : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        if (owner.authority > target.Encryption || owner == target.Owner)
            GameManager.KillProcessOrDaemon(target);
        else
            referenceManager.terminalUIManager.Print("AUTHORIZATION ERROR: Access to process decryption hash denied");
    }
}
