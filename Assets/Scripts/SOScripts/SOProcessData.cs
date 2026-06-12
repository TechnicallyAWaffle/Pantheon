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
    public Enterprise enterprise;
    public GameObject processObject;
    public bool removedWhenExecuted;
    public int threatLevel; //1 - 10 for the enemy AI to evaluate if it's gonna fuck it up or not
    public ArgumentType[] arguments;


    public enum Enterprise
    { 
        SEEINGSTONE,
        KALI,
        CHASAVOY,
        MIRAI,
        MINDCLOUD
    }

    public enum ArgumentType 
    {
        NONE,
        PROCESSID,
        DAEMON,
        PROCESSORDAEMON,
    };

}