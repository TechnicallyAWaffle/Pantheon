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
        return entity.authority >= process.encryption;
    }
        
    /// <summary>
    /// Returns true when the entity's authority is greater than the daemon's encryption. False otherwise.
    /// </summary>
    public static bool CheckAuthority(Entity entity, DaemonBase daemon)
    {
        return entity.authority >= daemon.encryption;
    }

    /// <summary>
    /// Adds amount to encryption and clamps it between 1 and 3.
    /// </summary>
    /// <param name="process"></param>
    /// <param name="amount"></param>
    public static void AddEncryption(RunningProcess process, int amount)
    {
        process.encryption = Mathf.Clamp(process.encryption + amount, 1, int.MaxValue);
    }

    /// <summary>
    /// Adds amount to encryption and clamps it between 1 and 3.
    /// </summary>
    /// <param name="process"></param>
    /// <param name="amount"></param>
    public static void AddEncryption(DaemonBase daemon, int amount)
    {
        daemon.encryption = Mathf.Clamp(daemon.encryption + amount, 1, int.MaxValue);
    }

}
