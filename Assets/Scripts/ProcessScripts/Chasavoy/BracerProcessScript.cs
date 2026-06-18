using System;
using System.Collections.Generic;
using UnityEngine;

public class BracerProcessScript : ProcessBase
{
    private readonly int processEncryptionAmount = 2; 
    private readonly int daemonEncryptionAmount = 1;

    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        if (target is RunningProcess)
        {
            EncryptionManager.AddEncryption(target, processEncryptionAmount);
        }
        if (target is DaemonBase)
        {
            EncryptionManager.AddEncryption(target, daemonEncryptionAmount);
        }
    }
}
