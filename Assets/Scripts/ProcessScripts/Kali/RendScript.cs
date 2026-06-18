using UnityEngine;

public class RendScript : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        EncryptionManager.AddEncryption(target, -1);
    }

    public override void OnKilled()
    {
        base.OnKilled();
    }
}