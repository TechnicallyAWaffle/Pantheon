using UnityEngine;

public class ScorchsearchScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        DaemonBase daemon = GameManager.AllActiveDaemons[arguments[0]];
        daemon.RevealDaemon();
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}