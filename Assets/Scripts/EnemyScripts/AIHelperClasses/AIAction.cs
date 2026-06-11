using UnityEngine;

public class AIAction
{
    public AIActionType type;
    public SOProcessData process;
    public ITargetable target;
    public ProcessQueue queue;
    public string[] arguments;
    public bool isServer;

    public static AIAction RunProcess(string[] args, ProcessQueue q)
        => new AIAction { type = AIActionType.RunProcess, arguments = args, queue = q};

    public static AIAction KillProcess(RunningProcess process)
        => new AIAction { type = AIActionType.KillProcess, target = process };

    public static AIAction KillDaemon(DaemonBase daemon)
        => new AIAction { type = AIActionType.KillDaemon, target = daemon };

    public static AIAction Wait()
        => new AIAction { type = AIActionType.Wait };

    public enum AIActionType { RunProcess, KillProcess, SuspendProcess, KillDaemon, Wait }
}
