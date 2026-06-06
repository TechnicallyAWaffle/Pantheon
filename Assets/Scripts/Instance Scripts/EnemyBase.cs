using System.Buffers;
using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public Entity entityScript;
    
    //Runtime Vars
    public float aggression; //0 - 1 inclusive function 

    private void Start()
    {
        entityScript = GetComponent<Entity>();
    }


    protected virtual IEnumerator BehaviourLoop()
    {
        //
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
