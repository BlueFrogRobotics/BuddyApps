using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class WinState : AStateMachineBehaviour
    {

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.TextToSpeech.SayKey("won");
            Interaction.Mood.Set(MoodType.HAPPY);
            Toaster.Display<VictoryToast>().With(Dictionary.GetString("win"));
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