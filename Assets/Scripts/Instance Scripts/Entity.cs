using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int authority;           // 0 (kernel) to 3
    public long reservedLocalMemory;
    public double reservedLocalCompute;
    public long reservedServerMemory;
    public double reservedServerCompute;
    public float temperature;
    
    //public string activeEnterprise;
    //public List<Daemon> daemons;

}