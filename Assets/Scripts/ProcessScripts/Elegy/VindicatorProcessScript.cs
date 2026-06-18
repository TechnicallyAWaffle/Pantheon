using System.Buffers;
using UnityEngine;

public class VindicatorProcessScript : ProcessBase
{

    private void Start()
    {
        foreach (RunningProcess process in runtimeProcessData.owner.ownedProcesses)
        {
            if (process.data.processName == "vindicator" && runtimeProcessData.timeRemaining >= 2)
                runtimeProcessData.timeRemaining -= 2;
        }
    }

    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        if (target.Encryption == 0)
            GameManager.KillProcessOrDaemon(target);
        else
        {
            EncryptionManager.AddEncryption(target, -1);
        }
    }
}
