using System.Buffers;
using UnityEngine;

public class MemoryAdderDaemon : DaemonBase
{
    [SerializeField] float timeBetweenActivations = 12;
    private float currentTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = timeBetweenActivations;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (currentTime <= 0)
        {
            if (!OnTrigger()) return;
            currentTime = timeBetweenActivations;
            owner.RequestServerMemory(1);
        }
        else
            currentTime -= Time.deltaTime;
    }
}
