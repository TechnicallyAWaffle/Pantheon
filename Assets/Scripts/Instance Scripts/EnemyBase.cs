using System.Buffers;
using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public Entity entityScript;
    
    //Runtime Vars
    public int aggression;

    protected virtual IEnumerator BehaviourLoop()
    {
        while (true)
        { 
            
        }
    }

    protected virtual void EvaluateBestOption()
    {



    }

    protected virtual void DefaultProcessStateMachine()
    { 
        
    }


}
