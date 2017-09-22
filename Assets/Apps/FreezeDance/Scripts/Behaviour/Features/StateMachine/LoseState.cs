using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class LoseState : AStateMachineBehaviour
    {

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.TextToSpeech.SayKey("lost");
            Interaction.Mood.Set(MoodType.SAD);
            Toaster.Display<DefeatToast>().With(Dictionary.GetString("lost"));
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