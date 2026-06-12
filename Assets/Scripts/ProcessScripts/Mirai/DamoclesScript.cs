using UnityEngine;

public class DamoclesScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // damocles logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}