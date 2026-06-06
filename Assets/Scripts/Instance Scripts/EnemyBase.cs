using System.Buffers;
using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public Entity entityScript;
    
    //Refs
    ReferenceManager referenceManager;
    Entity player;

    //Tuning Vars
    [SerializeField] private int minimumTolerableMemory; //The enemy will always try to keep their values over these
    [SerializeField] private int minimumTolerableCompute;


    //Runtime Vars
    [SerializeField] private float aggression; //0 - 1 inclusive float
    [SerializeField] private SOProcessData nextBestAction;
    
    private void Start()
    {
        entityScript = GetComponent<Entity>();
        referenceManager = ReferenceManager.Instance;
        player = referenceManager.player;

        GlobalEventBus.OnPlayerQueuedAProcess += EvaluateBestOption;
    }


    protected virtual IEnumerator BehaviourLoop()
    {
        //
        while (true)
        { 
            
        }
    }

    protected virtual void EvaluateBestOption(RunningProcess process)
    {

    }

    protected virtual void DefaultProcessStateMachine()
    { 
        
    }

    protected bool CheckMemoryThreshold()
    { 
        return entityScript.reservedServerMemory > minimumTolerableMemory;
    }

    protected bool CheckComputeThreshold()
    {
        return entityScript.reservedServerCompute > minimumTolerableCompute;
    }



}
