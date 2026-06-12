using UnityEngine;

public class WormsearchScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // wormsearch logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}