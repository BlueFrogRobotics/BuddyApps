using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.TestCoroutineDisplay
{
    public class testdelay : AStateMachineBehaviour
    {
        private float timer;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer = 0F;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer += Time.deltaTime;
            Debug.Log("Timer : " + timer + " is busy ? : " + Buddy.GUI.Toaster.IsBusy);
            if (timer > 6F)
                Trigger("lol");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

    }
}

