using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DaemonManager : MonoBehaviour
{
    //Daemons are gameobject prefabs that we physically attach to the player and opponent

    public Dictionary<string, DaemonBase> AllRevealedDaemons = new();

    public void AddDaemon(GameObject daemon, Entity owner)
    {
        DaemonBase daemonScript = daemon.GetComponent<DaemonBase>();
        daemon.transform.parent = owner.transform;
        owner.daemons.Add(daemonScript);
        AllRevealedDaemons.Add(daemonScript.daemonName, daemonScript);
    }

    public void KillDaemon(GameObject daemon, Entity owner)
    {
        DaemonBase daemonScript = daemon.GetComponent<DaemonBase>();
        owner.daemons.Remove(daemonScript);
        GameObject.Destroy(daemon);
        AllRevealedDaemons.Remove(daemonScript.daemonName);
    }


    public void SetDaemonsActiveState(Entity owner, bool isActive)
    {
        foreach (DaemonBase daemon in owner.daemons)
        { 
            daemon.enabled = isActive;
        }
    }
}
