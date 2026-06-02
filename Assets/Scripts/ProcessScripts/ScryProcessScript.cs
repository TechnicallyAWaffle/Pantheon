using System.ComponentModel.Design;
using UnityEngine;

public class ScryProcessScript : ProcessBase
{
    public override void Execute(Entity owner, string[] arguments)
    {
        owner.RequestMemory(1);
    }
}
