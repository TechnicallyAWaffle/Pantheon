using UnityEditor.Build;
using UnityEngine;

[CreateAssetMenu(fileName = "SOProcessData", menuName = "Scriptable Objects/SOProcessData")]
// The process/command definition
public class SOProcessData : ScriptableObject
{
    public string processName;
    public string description;
    public float baseExecutionTime;
    public int memoryUsage;        
    public int encryption; // 1 - 3 just like authority
    public GameObject processObject;
    public string[] arguments;
    public bool releasesMemoryOnExecution;
}