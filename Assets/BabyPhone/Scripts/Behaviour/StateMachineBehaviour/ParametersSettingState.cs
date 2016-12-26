using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class ParametersSettingState : AStateMachineBehaviour
    {
        private BabyPhoneData mBabyPhoneData;

        private GameObject mParameters;
        private GameObject mWindoAppOverWithe;
        private GameObject mBlackground;
        private Animator mBackgroundBlackAnimator;
        private Animator mParametersAnimator;

        public override void Init()
        {
            mBabyPhoneData = BabyPhoneData.Instance;

            mParameters = GetGameObject(5);
            mWindoAppOverWithe = GetGameObject(3);
            mBlackground = GetGameObject(1);
            mBackgroundBlackAnimator = mBlackground.GetComponent<Animator>();
            mParametersAnimator = mParameters.GetComponent<Animator>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mParameters.SetActive(true);
            mWindoAppOverWithe.SetActive(true);
            //mBlackground.SetActive(true);
            mBackgroundBlackAnimator.SetTrigger("Open_BG");
            mParametersAnimator.SetTrigger("Open_WParameters");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mParameters.SetActive(false);
            mWindoAppOverWithe.SetActive(false);
            //mBlackground.SetActive(false);
            mBackgroundBlackAnimator.SetTrigger("Close_BG");
            mParametersAnimator.SetTrigger("Close_WParameters");
            iAnimator.SetInteger("ForwardState", 1);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
             
        }

    }
}
