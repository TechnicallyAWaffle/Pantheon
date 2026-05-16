using UnityEngine;

[CreateAssetMenu(fileName = "Program", menuName = "Scriptable Objects/Program")]
public class SOProgram : ScriptableObject
{
    public string commandName;
    public string description;
    public float baseExecutionTime;
}
