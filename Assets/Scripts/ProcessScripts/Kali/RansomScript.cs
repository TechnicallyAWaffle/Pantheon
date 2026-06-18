using UnityEngine;

public class RansomScript : ProcessBase
{
    [SerializeField] private int memoryRelinquished = 5;

    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        if (owner == referenceManager.player)                         //TODO: Blow this shitass implementation up and replace it with something half decent 
            referenceManager.opponent.RelinquishServerMemory(memoryRelinquished);
        else
            referenceManager.player.RelinquishServerMemory(memoryRelinquished);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}