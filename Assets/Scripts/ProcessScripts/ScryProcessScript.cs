using System.ComponentModel.Design;
using UnityEngine;

public class ScryProcessScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        ResourceManager resourceManager = owner.referenceManager.resourceManager;
        owner.referenceManager.resourceManager.RequestMemory(ref resourceManager.openServerMemory, 1, owner);
    }
}
