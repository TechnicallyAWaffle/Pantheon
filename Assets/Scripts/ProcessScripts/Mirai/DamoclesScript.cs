using UnityEngine;

public class DamoclesScript : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        owner.authority = 3;
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}