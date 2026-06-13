using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;


//README
//You can use DaemonManager's SetDaemonActiveState() to change their active state once they've been revealed, while 
//revealing them is managed by this script's RevealDaemon. Daemons can never be unrevealed.

public class DaemonBase : MonoBehaviour, ITargetable
{
    //Refs
    public string daemonName;
    private SpriteRenderer daemonSprite;
    public Entity owner;

    public bool isSuspended;
    public bool isRevealed = false;
    // integer 1-3

    public string Identifier { get { return daemonName;} set { } }

    public int Encryption { get; set; }

    public Entity Owner { get { return owner; } set { } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        daemonSprite = GetComponent<SpriteRenderer>();
        if (transform.root.TryGetComponent<Entity>(out Entity entityOwner))
        {
            owner = entityOwner;
        }
        else
            Debug.LogError("Daemon could not find root object with Entity component!");
        if (owner == ReferenceManager.Instance.player)
            daemonSprite.enabled = true;

        //Add to GameManager tracker
        GameManager.AllActiveDaemons.Add(daemonName, this);

        //Add to owner's Daemon List
        owner.daemons.Add(this);
;
    }

    protected virtual void Update()
    {
        if (isSuspended) return;
    }


    public void RevealDaemon()
    {
        WriteDebug("Revealing daemon: " + daemonName);
        daemonSprite.enabled = true;
        isRevealed = true;
        //Plus other logic
    }

    public virtual void OnSuspension()
    {

    }

    protected virtual void OnTrigger()
    {
        if(owner == ReferenceManager.Instance.opponent)
            ReferenceManager.Instance.terminalUIManager.Print("[SYSALERT] External daemon " + daemonName + " executed on scheduler stack successfully");
    }


    public virtual void OnSuspensionLifted()
    {

    }

    public virtual void OnKilled()
    { 
    
    }

    private void WriteDebug(string message)
    {
        UnityEngine.Debug.Log("<color=white>DAEMON: " + message);
    }


}
