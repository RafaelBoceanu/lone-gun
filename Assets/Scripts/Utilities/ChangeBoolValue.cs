using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class ChangeBoolValue : StateMachineBehaviour
    {
        public string boolName;
        public bool value;
        public bool onExitRevers;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(boolName, value);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(boolName, !value);
        }
    }
}