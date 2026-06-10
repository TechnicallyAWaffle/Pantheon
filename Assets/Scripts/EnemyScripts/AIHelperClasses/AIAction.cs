using UnityEngine;

public class AIAction
{
    public AIActionType type;
    public SOProcessData process;
    public RunningProcess target;
    public ProcessQueue queue;
    public string[] arguments;

    public static AIAction RunProcess(SOProcessData p, ProcessQueue q, string[] args = null)
        => new AIAction { type = AIActionType.RunProcess, process = p, queue = q, arguments = args };

    public static AIAction KillProcess(RunningProcess target)
        => new AIAction { type = AIActionType.KillProcess, target = target };

    public static AIAction Wait()
        => new AIAction { type = AIActionType.Wait };

    public enum AIActionType { RunProcess, KillProcess, SuspendProcess, Wait }
}
