using UnityEngine;

public class MeltdownScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // meltdown logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}