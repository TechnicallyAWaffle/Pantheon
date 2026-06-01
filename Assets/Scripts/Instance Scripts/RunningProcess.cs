using System;

public class RunningProcess
{
    public SOProcessData data;
    public Entity owner;
    public float timeRemaining;
    public float baseTime;          // needed to recalculate on compute changes
    public EncryptionLevel encryption;
    public bool isServerSide;
    public int processID;
    public string[] arguments;
    public QueueManager.ProcessQueue queue;

    //Runtime
    public bool isSuspended;
    public Func<bool> suspensionLiftCondition;
    public int memoryUsed;
}