using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int openPlayerLocalMemory;
    public int openPlayerLocalCompute;

    public int openServerMemory;
    public int openServerCompute;

    public int openEnemyLocalMemory;
    public int openEnemyLocalCompute;
    
    //Pass in memory/compute location by reference so we can set it in the same function
    //All this script does is validate if you are allowed to extract X amount of memory/compute from Y place and give it to you
    public long RequestMemory(ref int openMemory, int amountRequested, Entity requester)
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

    public double RequestCompute(ref int openCompute, int amountRequested, Entity requester)
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
