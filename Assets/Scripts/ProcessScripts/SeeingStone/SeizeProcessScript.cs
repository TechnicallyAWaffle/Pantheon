using UnityEngine;

public class SeizeProcessScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        owner.RequestServerCompute(1);
        base.Execute(owner, arguments);
    }
}
