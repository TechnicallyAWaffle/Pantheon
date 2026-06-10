using UnityEngine;

public class KillProcess : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        RunningProcess processToKill = referenceManager.queueManager.AllRunningProcessesByID[arguments[0]];
        if (owner.authority > processToKill.encryption)
            processToKill.queue.processesToRemove.Add(processToKill);
        else
            referenceManager.terminalUIManager.Print("AUTHORIZATION ERROR: Access to process decryption hash denied");
        base.Execute(owner, arguments);
    }
}
