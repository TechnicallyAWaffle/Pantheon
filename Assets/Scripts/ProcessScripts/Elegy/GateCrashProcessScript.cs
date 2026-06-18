using UnityEngine;

public class GateCrashProcessScript : ProcessBase
{
    protected override void ExecuteAction(Entity owner, string[] arguments)
    {
        EncryptionManager.AddEncryption(target, -3);
    }
}