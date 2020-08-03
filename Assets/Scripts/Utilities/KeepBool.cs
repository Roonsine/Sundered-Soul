using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepBool : StateMachineBehaviour
{
    public string boolName;
    public bool status;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, status);   
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, !status);
    }
}
