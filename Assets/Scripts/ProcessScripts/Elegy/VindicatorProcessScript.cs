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

    public override void Execute(Entity owner, string[] arguments)
    {
        ITargetable target = GameManager.FindRunningDaemonOrProcess(arguments[0]);
        if (target.Encryption == 0)
            GameManager.KillProcessOrDaemon(target);
        else
        {
            EncryptionManager.AddEncryption(target, -1);
        }
        base.Execute(owner, arguments);
    }
}
