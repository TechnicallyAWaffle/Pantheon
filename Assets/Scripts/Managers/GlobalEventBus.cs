using System;
using UnityEngine;
using UnityEngine.Playables;

public static class GlobalEventBus
{

    // Process events
    public static event Action<RunningProcess> OnProcessQueued;
    public static event Action<RunningProcess> OnProcessCompleted;
    public static event Action<RunningProcess, Entity> OnProcessKilled;
    public static event Action<RunningProcess, Entity> OnProcessSuspended;

    // Resource events
    public static event Action<Entity, long> OnMemoryChanged;    
    public static event Action<Entity, double> OnComputeChanged;
    public static event Action<Entity, int> OnAuthorityChanged; 

    // Queue events
    public static event Action<RunningProcess> OnServerProcessQueued;
    public static event Action<RunningProcess> OnLocalProcessQueued;
    public static event Action<int> OnServerQueueCountChanged; 

    // Daemon events
    public static event Action<Entity> OnDaemonRevealed;

    // requests — anyone can fire these
    public static event Action<Entity, long> OnMemoryRequested;
    public static event Action<Entity, double> OnComputeRequested;

    // results — manager fires these after processing
    public static event Action<Entity, long> OnMemoryAllocated;
    public static event Action<Entity, long> OnMemoryDenied;

    

    // Invokers
    public static void ProcessQueued(RunningProcess p) => OnProcessQueued?.Invoke(p);
    public static void ProcessCompleted(RunningProcess p) => OnProcessCompleted?.Invoke(p);
    public static void ProcessKilled(RunningProcess p, Entity killer)
                                                                    => OnProcessKilled?.Invoke(p, killer);
    public static void AuthorityChanged(Entity p, int newVal) => OnAuthorityChanged?.Invoke(p, newVal);
    public static void ServerQueueCountChanged(int count) => OnServerQueueCountChanged?.Invoke(count);

    public static void RequestMemory(Entity player, long amount)
        => OnMemoryRequested?.Invoke(player, amount);
    public static void RequestCompute(Entity player, double amount)
        => OnComputeRequested?.Invoke(player, amount);
}