using UnityEngine;

public class SuspendProcess : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {



        RunningProcess processToSuspend = referenceManager.queueManager.AllRunningProcessesByID[arguments[0]];

        if (owner.authority > processToSuspend.encryption)
            referenceManager.suspensionManager.Suspend(processToSuspend, () => false, runtimeProcessData);
        else
            referenceManager.terminalUIManager.Print("AUTHORIZATION ERROR: Access to process decryption hash denied");
        base.Execute(owner, arguments);
    }
}
