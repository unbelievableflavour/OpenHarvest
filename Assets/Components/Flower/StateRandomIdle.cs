using UnityEngine;
public class StateRandomIdle : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("Offset", Random.Range(0.0f, 1.0f));
        animator.Play("Idle", -1);
    }
}