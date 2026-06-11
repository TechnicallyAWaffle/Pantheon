using UnityEngine;

public class ExciseProcessScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        int randIndex = Random.Range(0, referenceManager.player.daemons.Count);
        DaemonBase daemonToReveal = referenceManager.player.daemons[randIndex];
        DaemonManager.RevealDaemon(daemonToReveal);
        base.Execute(owner, arguments);
    }
}
