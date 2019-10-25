using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.FreezeDance
{
    public class WinState : AStateMachineBehaviour
    {

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Vocal.SayKey("won");
            Buddy.Behaviour.SetMood(Mood.HAPPY);
            //Toaster.Display<VictoryToast>().With(Buddy.Resources.GetString("won"));
            StartCoroutine(Restart());
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        private IEnumerator Restart()
        {
            yield return new WaitForSeconds(5.0f);
            Trigger("Restart");
            yield return null;
        }
    }
}