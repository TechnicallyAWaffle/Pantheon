using System;
using UnityEngine;

public class RunningProcess : MonoBehaviour
{
    public SOProcessData data;
    public Entity owner;
    public string[] arguments;
    public ProcessQueue queue;
    public int memoryUsed;

    //runtime
    public float timeRemaining;
    public float baseTime;          // needed to recalculate on compute changes
    public int encryption;
    public string processID;
    public bool isSuspended;
    public Func<bool> suspensionLiftCondition;
    public bool executed = false;
}