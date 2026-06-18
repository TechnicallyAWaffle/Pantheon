using System.ComponentModel.Design;
using UnityEngine;

public class ScryProcessScript : ProcessBase
{

    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        owner.RequestServerMemory(1);
    }
}
