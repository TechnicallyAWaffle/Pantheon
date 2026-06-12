using UnityEngine;

public class RansomScript : ProcessBase
{
    [SerializeField] private int memoryRelinquished = 5;

    public override void Execute(Entity owner, string[] arguments)
    {
        if (owner == referenceManager.player)                         //TODO: Blow this shitass implementation up and replace it with something half decent 
            referenceManager.opponent.RelinquishServerMemory(5);
        else
            referenceManager.player.RelinquishServerMemory(5);

        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}