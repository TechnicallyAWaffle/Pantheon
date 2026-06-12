using UnityEngine;

public class CrowdstrikeScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        // crowdstrike logic here
        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}