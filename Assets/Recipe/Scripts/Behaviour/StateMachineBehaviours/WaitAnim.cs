using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class WaitAnim : AStateMachineBehaviour
    {
        private bool mDone;

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Mood.Set(MoodType.HAPPY);
            Primitive.Speaker.Voice.Play(VoiceSound.FOCUS_2);
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Primitive.Speaker.Voice.Status == SoundChannelStatus.FINISH)
                GetComponent<Animator>().SetTrigger("FinishRecipe");
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
        }
    }
}