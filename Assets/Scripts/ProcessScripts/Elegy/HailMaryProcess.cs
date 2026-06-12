using NUnit.Framework;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class HailMaryProcess : ProcessBase
{
    [SerializeField] int memoryAndComputeGranted = 50;

    public override void Execute(Entity owner, string[] arguments)
    {

        if (referenceManager.player.daemons.Count > 0)
        {
            int index = Random.Range(0, referenceManager.player.daemons.Count);
            DaemonBase daemonToKill = referenceManager.player.daemons[index];
            GameManager.KillProcessOrDaemon(daemonToKill);
        }
        owner.localProcessQueue._openMemory += memoryAndComputeGranted;
        owner.localProcessQueue._openCompute += memoryAndComputeGranted;

        base.Execute(owner, arguments);
    }

}
