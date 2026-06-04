using Unity.VisualScripting;
using UnityEngine;

public class DaemonManager : MonoBehaviour
{
    //Daemons are gameobject prefabs that we physically attach to the player and opponent
    
    public void AddDaemon(GameObject daemon, Entity owner)
    { 
        daemon.transform.parent = owner.transform;
        owner.daemons.Add(daemon.GetComponent<DaemonBase>());
    }

    public void KillDaemon(GameObject daemon, Entity owner)
    {
        owner.daemons.Remove(daemon.GetComponent<DaemonBase>());
        GameObject.Destroy(daemon.gameObject);
    }


    public void SetDaemonsActiveState(Entity owner, bool isActive)
    {
        foreach (DaemonBase daemon in owner.daemons)
        { 
            daemon.enabled = isActive;
        }
    }
}
