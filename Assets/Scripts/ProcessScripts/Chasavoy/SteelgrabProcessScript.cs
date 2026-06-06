using System;
using System.Collections.Generic;
using UnityEngine;

public class SteelgrabProcessScript : ProcessBase
{
    RunningProcess targetProcess = null;
    SuspensionManager suspensionManager;

    protected override void Start()
    {
        base.Start();   
        suspensionManager = FindAnyObjectByType<SuspensionManager>();
    }

    public override void Execute(Entity owner, string[] arguments)
    {
        base.Execute(owner, arguments);

        try
        {
            string arg0 = arguments[0];

            // check if arg0 is a valid PID
            if (int.TryParse(arg0, out _))
            {
                if (referenceManager.queueManager.AllRunningProcessesByID.TryGetValue(arg0, out targetProcess))
                {
                    suspensionManager.Suspend(targetProcess, () => false, owner);
                }
                else
                {
                    throw new KeyNotFoundException($"Steelgrab could not find process {arg0}. It may have been killed or completed.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public override void Update()
    {
        base.Update();
        if (!isExecuted) return;
    }
}
