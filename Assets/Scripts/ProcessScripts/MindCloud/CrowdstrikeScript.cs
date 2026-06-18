using UnityEngine;

public class CrowdstrikeScript : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        // crowdstrike logic here
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}