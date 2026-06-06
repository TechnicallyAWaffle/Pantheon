using System;
using System.Collections.Generic;
using UnityEngine;

public class BracerProcessScript : ProcessBase
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
                    RaiseProcessEncryption(targetProcess);
                }
                else
                {
                    throw new KeyNotFoundException($"Bracer could not find process {arg0}. It may have been killed or completed.");
                }
            }
            else
            {
                if (referenceManager.daemonManager.AllRevealedDaemons.TryGetValue(arg0, out var targetDaemon))
                {
                    RaiseDaemonEncryption(targetDaemon);
                }
                else
                {
                    throw new KeyNotFoundException($"Bracer could not find daemon {arg0}. It may have been killed or completed.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// Raises a process's encryption by 2. 
    /// </summary>
    /// <param name="process"></param>
    public void RaiseProcessEncryption(RunningProcess process)
    {
        EncryptionManager.AddEncryption(process, 2);
        process.encryption = Math.Clamp(process.encryption + 2, 1, 3);
    }

    /// <summary>
    /// Raises a daemon's encryption by 1. 
    /// </summary>
    /// <param name="process"></param>
    public void RaiseDaemonEncryption(DaemonBase daemon)
    {
        EncryptionManager.AddEncryption(daemon, 1);
    }
}
