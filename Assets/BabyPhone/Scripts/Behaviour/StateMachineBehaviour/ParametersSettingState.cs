using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class ParametersSettingState : AStateMachineBehaviour
    {
        private GameObject mParameters;
        private GameObject mWindoAppOverWithe;
        private GameObject mBlackground;
        private Animator mBackgroundBlackAnimator;
        private Animator mParametersAnimator;
        private GameObject mCartoon;
        private Animator mCartoonAnimator;

        public override void Init()
        {
            mParameters = GetGameObject(5);
            mWindoAppOverWithe = GetGameObject(3);
            mBlackground = GetGameObject(1);
            mBackgroundBlackAnimator = mBlackground.GetComponent<Animator>();
            mParametersAnimator = mParameters.GetComponent<Animator>();
            mCartoon = GetGameObject(12);
            mCartoonAnimator = mCartoon.GetComponent<Animator>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mParameters.SetActive(true);
            mWindoAppOverWithe.SetActive(true);
            //mBlackground.SetActive(true);
            mBackgroundBlackAnimator.SetTrigger("Open_BG");
            mParametersAnimator.SetTrigger("Open_WParameters");
            mMood.Set(MoodType.HAPPY);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mParameters.SetActive(false);
            mWindoAppOverWithe.SetActive(false);
            //mBlackground.SetActive(false);
            //if (iAnimator.GetBool("QuitButton"))
            mBackgroundBlackAnimator.SetTrigger("Close_BG");           
            mParametersAnimator.SetTrigger("Close_WParameters");
            iAnimator.SetInteger("ForwardState", 1);

            mSpeaker.Media.Stop();

            mCartoon.SetActive(false);
            mCartoonAnimator.SetBool("IsPlaying", false);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
             
        }

    }
}
