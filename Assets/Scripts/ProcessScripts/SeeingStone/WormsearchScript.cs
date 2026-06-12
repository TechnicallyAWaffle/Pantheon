using UnityEngine;

public class WormsearchScript : ProcessBase
{
    [SerializeField] private int maxMod = 8;
    public override void Execute(Entity owner, string[] arguments)
    {
        int randomExecutionTimeMod = Random.Range(0, maxMod);

        base.Execute(owner, arguments);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}