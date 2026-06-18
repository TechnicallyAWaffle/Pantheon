using UnityEngine;

public class ExciseProcessScript : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        int randIndex = Random.Range(0, referenceManager.player.daemons.Count);
        DaemonBase daemonToReveal = referenceManager.player.daemons[randIndex];
        DaemonManager.RevealDaemon(daemonToReveal);
    }
}
