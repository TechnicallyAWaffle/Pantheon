using UnityEngine;


//README
//Daemons operate mostly off Update(), so their logic is gated by if they're enabled or disabled in the inspector
//You can use DaemonManager's SetDaemonActiveState() to change their active state once they've been revealed, while 
//revealing them is managed by this script's RevealDaemon. Daemons can never be unrevealed.

public class DaemonBase : MonoBehaviour
{
    public bool isSuspended;
    public string daemonName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void RevealDaemon()
    {
        this.enabled = true;

        //Plus other logic
    }
}
