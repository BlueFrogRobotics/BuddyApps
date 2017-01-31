using UnityEngine;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.Recipe
{
    public class WaitAnim : AStateMachineBehaviour
    {
        private bool mDone;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMood.Set(MoodType.HAPPY);
            BYOS.Instance.Speaker.Voice.Play(VoiceSound.FOCUS_2);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mSpeaker.Voice.Status == SoundChannelStatus.FINISH)
                GetComponent<Animator>().SetTrigger("DisplayIngredient");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetGameObject(1).SetActive(false);
            GetGameObject(2).SetActive(true);
        }
    }
}