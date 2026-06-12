using UnityEngine;

public class LogicbombScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // logicbomb logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}