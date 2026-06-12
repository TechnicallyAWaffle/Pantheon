using UnityEngine;

public class DamoclesScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        owner.authority = 3;
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}