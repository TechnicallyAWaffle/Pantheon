using UnityEngine;

public class PsychostormScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // psychostorm logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}