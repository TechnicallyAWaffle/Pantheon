using UnityEngine;

public class ConstrictScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // constrict logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}