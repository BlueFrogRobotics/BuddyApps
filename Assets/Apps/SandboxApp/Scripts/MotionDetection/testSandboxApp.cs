using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.SandboxApp
{
    public class testSandboxApp : AStateMachineBehaviour
    {
        private float mTimer;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer = 0F;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (mTimer < 5F)
                Debug.Log("lol");
            else
            {
                animator.SetFloat("GetTimer", 5f);
                animator.SetTrigger("quit");
            }
                
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("exit");
            
        }

    }

}
