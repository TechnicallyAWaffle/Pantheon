using UnityEngine;

[CreateAssetMenu(fileName = "Command", menuName = "Scriptable Objects/Command")]
public class SOCommand : ScriptableObject
{
    public string commandName;
    public string description;
}
