using UnityEngine;

public class AIAction
{
    public AIActionType type;
    public SOProcessData process;
    public RunningProcess target;
    public ProcessQueue queue;
    public string[] arguments;
    public bool isServer;

    public static AIAction RunProcess(string[] args, ProcessQueue q)
        => new AIAction { type = AIActionType.RunProcess, arguments = args, queue = q};

    public static AIAction KillProcess(RunningProcess target)
        => new AIAction { type = AIActionType.KillProcess, target = target };

    public static AIAction Wait()
        => new AIAction { type = AIActionType.Wait };

    public enum AIActionType { RunProcess, KillProcess, SuspendProcess, Wait }
}
