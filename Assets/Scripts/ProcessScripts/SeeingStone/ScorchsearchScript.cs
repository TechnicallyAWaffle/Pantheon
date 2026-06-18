using UnityEngine;

public class ScorchsearchScript : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        DaemonBase daemon = (DaemonBase)(target);
        daemon.RevealDaemon();
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}