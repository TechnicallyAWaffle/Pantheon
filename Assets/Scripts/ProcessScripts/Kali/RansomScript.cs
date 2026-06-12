using UnityEngine;

public class RansomScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // ransom logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}