using UnityEngine;

public class RendScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // rend logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}