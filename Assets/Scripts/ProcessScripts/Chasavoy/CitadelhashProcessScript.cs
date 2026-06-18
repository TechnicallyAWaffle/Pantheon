using System;
using System.Collections.Generic;
using UnityEngine;

public class CitadelhashProcessScript : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        try
        {
            ITargetable target = GameManager.FindRunningDaemonOrProcess(arguments[0]);
            string arg0 = arguments[0];
            if(target != null)
                EncryptionManager.AddEncryption(target, 3);
            else
                throw new KeyNotFoundException($"Citadelhash could not find process {arg0}. It may have been killed or completed.");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
