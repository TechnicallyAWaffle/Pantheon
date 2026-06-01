using UnityEditor.Build;
using UnityEngine;

[CreateAssetMenu(fileName = "SOProcessData", menuName = "Scriptable Objects/SOProcessData")]
// The process/command definition
public class SOProcessData : ScriptableObject
{
    public string processName;
    public string description;
    public float baseExecutionTime;
    public long memoryUsage;        
    public double computeUsage;     
    public EncryptionLevel encryption;
    public IProcess processScript;
    public string[] arguments;
}

public enum EncryptionLevel { None, Light, Moderate, Heavy }