using UnityEngine;

public class AcquisitionProcessScript : ProcessBase
{
    [SerializeField] int resourcesGranted = 5;
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        if (owner.reservedServerCompute < owner.reservedServerMemory)
            owner.RequestServerCompute(resourcesGranted);
        else
            owner.RequestServerMemory(resourcesGranted);
    }
}
