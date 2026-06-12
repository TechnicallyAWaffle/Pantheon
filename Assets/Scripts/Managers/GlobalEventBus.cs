using System;
using UnityEngine;
using UnityEngine.Playables;

public static class GlobalEventBus
{
    //README
    //Suscribe whatever you need to the events here with Action += Function in Start()
    //Set when these events actually fire by calling the Invokers down below


    // Process events
    public static event Action<RunningProcess> OnPlayerQueuedAProcess;
    public static event Action<RunningProcess, Entity> OnProcessQueued;
    public static event Action<RunningProcess> OnProcessCompleted;
    public static event Action<RunningProcess, Entity> OnProcessKilled;
    public static event Action<RunningProcess, RunningProcess> OnProcessSuspended;

    // Resource events
    public static event Action<Entity> OnMemoryChanged;    //Todo figure out what events i actually want
    public static event Action<Entity, int> OnComputeChanged;
    public static event Action<Entity, int> OnAuthorityChanged;
    public static event Action OnSchedulerReset;

    // Queue events
    public static event Action<RunningProcess> OnServerProcessQueued;
    public static event Action<RunningProcess> OnLocalProcessQueued;
    public static event Action<int> OnServerQueueCountChanged;
    public static event Action<RunningProcess> OnBaseKillProcessQueued;

    // Daemon events
    public static event Action<Entity> OnDaemonRevealed;

    // requests — anyone can fire these
    public static event Action<Entity, int> OnMemoryRequested;
    public static event Action<Entity, int> OnComputeRequested;

    // results — manager fires these after processing
    public static event Action<Entity, int> OnMemoryAllocated;
    public static event Action<Entity, int> OnMemoryDenied;



    // Invokers

    public static void MemoryChanged(Entity owner, int amount, ProcessQueue location)
        => OnMemoryChanged?.Invoke(owner);
    public static void ProcessQueued(RunningProcess p, Entity entity) => OnProcessQueued?.Invoke(p, entity);

    public static void PlayerQueuedProcess(RunningProcess p) => OnPlayerQueuedAProcess?.Invoke(p);
    public static void ProcessCompleted(RunningProcess p) => OnProcessCompleted?.Invoke(p);

    public static void ProcessSuspended(RunningProcess p, RunningProcess suspender) => OnProcessSuspended?.Invoke(p, suspender);
    public static void ProcessKilled(RunningProcess p, Entity killer)
                                                                    => OnProcessKilled?.Invoke(p, killer);
    public static void AuthorityChanged(Entity p, int newVal) => OnAuthorityChanged?.Invoke(p, newVal);
    public static void ServerQueueCountChanged(int count) => OnServerQueueCountChanged?.Invoke(count);

    public static void SchedulerReset() => OnSchedulerReset?.Invoke();

    public static void BaseKillProcessRan(RunningProcess process) => OnBaseKillProcessQueued?.Invoke(process);

    public static void RequestMemory(Entity player, int amount)
        => OnMemoryRequested?.Invoke(player, amount);
    public static void RequestCompute(Entity player, int amount)
        => OnComputeRequested?.Invoke(player, amount);
}