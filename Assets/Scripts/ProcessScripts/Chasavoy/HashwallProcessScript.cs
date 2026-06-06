using System.Collections.Generic;
using UnityEngine;

public class HashwallProcessScript : ProcessBase
{
    Entity _owner;
    List<RunningProcess> affectedProcesses;

    public override void Execute(Entity owner, string[] arguments)
    {
        base.Execute(owner, arguments);
        this._owner = owner;

        affectedProcesses = owner.ownedProcesses;
        AddEncryptionToAll(1);
    }

    public override void OnSuspension()
    {
        base.OnSuspension();

        if (isExecuted)
        {
            AddEncryptionToAll(-1);
        }
    }

    public override void OnSuspensionLifted()
    {
        base.OnSuspensionLifted();

        if (isExecuted)
        {
            AddEncryptionToAll(1);
        }
    }

    public override void OnKilled()
    {
        if (!runtimeProcessData.isSuspended && isExecuted)
        {
            AddEncryptionToAll(-1);
        }
    }

    private void AddEncryptionToAll(int amount)
    {
        foreach (var proc in affectedProcesses)
        {
            EncryptionManager.AddEncryption(proc, amount);
        }
    }
}
