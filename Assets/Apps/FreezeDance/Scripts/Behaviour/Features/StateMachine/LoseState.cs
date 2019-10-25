using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.FreezeDance
{
    public class LoseState : AStateMachineBehaviour
    {

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Vocal.SayKey("lost");
            Buddy.Behaviour.SetMood(Mood.SAD);
            //Toaster.Display<DefeatToast>().With(Buddy.Resources.GetString("lost"));
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