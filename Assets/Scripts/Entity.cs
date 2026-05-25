using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int authority;           // 0 (kernel) to 3
    public long reservedMemory;
    public double reservedCompute;
    public float temperature;
    public string activeEnterprise;
    //public List<Daemon> daemons;
}