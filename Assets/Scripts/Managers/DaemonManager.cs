using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DaemonManager : MonoBehaviour
{
    //Daemons are gameobject prefabs that we physically attach to the player and opponent

    //ONLY CALLED DURING RUNTIME 
    public void SetupDaemon(GameObject daemon, Entity owner, Transform parent)
    {
        DaemonBase daemonScript = daemon.GetComponent<DaemonBase>();
        daemon.transform.parent = parent;
        owner.daemons.Add(daemonScript);
        GameManager.AllActiveDaemons.Add(daemonScript.daemonName, daemonScript);
    }

    public static void KillDaemon(DaemonBase daemonScript)
    {
        daemonScript.owner.daemons.Remove(daemonScript);
        GameObject.Destroy(daemonScript);
        GameManager.AllActiveDaemons.Remove(daemonScript.daemonName);
    }

    public static void RevealDaemon(DaemonBase daemonScript)
    {
        daemonScript.RevealDaemon();
    }


    public void SetDaemonsActiveState(Entity owner, bool isActive)
    {
        foreach (DaemonBase daemon in owner.daemons)
        { 
            daemon.enabled = isActive;
        }
    }
}
