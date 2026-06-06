using UnityEditor.MPE;
using UnityEngine;

public class Enemy1Script : EnemyBase
{

    protected override void DefaultProcessStateMachine()
    {
        base.DefaultProcessStateMachine();
    }

    protected override void EvaluateBestOption(RunningProcess process)
    {
        if (!CheckComputeThreshold())
        {

        }
        if (!CheckMemoryThreshold())
        { 
            
        }
    }

}
