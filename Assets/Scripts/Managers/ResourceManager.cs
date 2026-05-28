using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public long openPlayerLocalMemory;
    public double openPlayerLocalCompute;

    public long openServerMemory;
    public double openServerCompute;

    public long openEnemyLocalMemory;
    public double openEnemyLocalCompute;
    
    //Pass in memory/compute location by reference so we can set it in the same function
    public long RequestMemory(ref long openMemory, long amountRequested, Entity requester)
    {
        GlobalEventBus.RequestMemory(requester, amountRequested);

        if (amountRequested > openMemory)
        {
            openMemory = 0;
            return openMemory;
        }
        else
        {
            openMemory -= amountRequested;
            return amountRequested;
        }
    }

    public double RequestCompute(ref double openCompute, double amountRequested, Entity requester)
    {
        GlobalEventBus.RequestCompute(requester, amountRequested);

        if (amountRequested > openCompute)
        {
            openCompute = 0;
            return openCompute;
        }
        else
        {
            openCompute -= amountRequested;
            return amountRequested;
        }
    }



}
