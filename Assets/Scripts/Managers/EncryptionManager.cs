using System;
using System.Diagnostics;
using UnityEngine;

public static class EncryptionManager
{
    /// <summary>
    /// Returns true when the entity's authority is greater than the process's encryption. False otherwise.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="process"></param>
    /// <returns></returns>
    public static bool CheckAuthority(Entity entity, RunningProcess process)
    {
        return entity.authority >= process.Encryption;
    }
        
    /// <summary>
    /// Returns true when the entity's authority is greater than the daemon's encryption. False otherwise.
    /// </summary>
    public static bool CheckAuthority(Entity entity, DaemonBase daemon)
    {
        return entity.authority >= daemon.Encryption;
    }

    /// <summary>
    /// Adds amount to encryption and clamps it between 1 and 3.
    /// </summary>
    /// <param name="process"></param>
    /// <param name="amount"></param>
    public static void AddEncryption(ITargetable target, int amount)
    {
        target.Encryption = Mathf.Clamp(target.Encryption + amount, 1, int.MaxValue);
    }

}
