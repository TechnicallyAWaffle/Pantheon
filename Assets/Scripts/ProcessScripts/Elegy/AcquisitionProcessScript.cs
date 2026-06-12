using UnityEngine;

public class AcquisitionProcessScript : ProcessBase
{
    [SerializeField] int resourcesGranted = 5;
    public override void Execute(Entity owner, string[] arguments)
    {
        if (owner.reservedServerCompute < owner.reservedServerMemory)
            owner.RequestServerCompute(resourcesGranted);
        else
            owner.RequestServerMemory(resourcesGranted);

            base.Execute(owner, arguments);
    }
}
