using UnityEngine;

public class SeizeProcessScript : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        owner.RequestServerCompute(1);
    }
}
