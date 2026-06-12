using UnityEngine;

public class StormershellScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // stormershell logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}