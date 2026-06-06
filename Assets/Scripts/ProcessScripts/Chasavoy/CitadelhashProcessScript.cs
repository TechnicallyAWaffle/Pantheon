using System;
using System.Collections.Generic;
using UnityEngine;

public class CitadelhashProcessScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        base.Execute(owner, arguments);

        try
        {
            string arg0 = arguments[0];

            // check if arg0 is a PID
            if (int.TryParse(arg0, out _))
            {
                if (referenceManager.queueManager.AllRunningProcessesByID.TryGetValue(arg0, out var targetProcess))
                {
                    EncryptionManager.AddEncryption(targetProcess, 3);
                }
                else
                {
                    throw new KeyNotFoundException($"Citadelhash could not find process {arg0}. It may have been killed or completed.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
