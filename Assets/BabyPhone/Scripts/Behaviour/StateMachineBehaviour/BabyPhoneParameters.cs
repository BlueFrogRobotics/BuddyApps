using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneParameters : AStateMachineBehaviour
    {
        private BabyPhoneData mBabyPhoneData;
        private GameObject mWindoAppOverWithe;
        private Animator mBackgroundBlackAnimator;
        private Animator mParametersAnimator;

        public override void Init()
        {
            mBabyPhoneData = BabyPhoneData.Instance;

            mWindoAppOverWithe = GetGameObject(12);
            mBackgroundBlackAnimator = GetGameObject(2).GetComponent<Animator>();
            mParametersAnimator = GetGameObject(3).GetComponent<Animator>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindoAppOverWithe.SetActive(true);
            mBackgroundBlackAnimator.SetTrigger("Open_BG");
            mParametersAnimator.SetTrigger("Open_WParameters");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindoAppOverWithe.SetActive(false);
            mBackgroundBlackAnimator.SetTrigger("Close_BG");
            mParametersAnimator.SetTrigger("Close_WParameters");
            iAnimator.SetFloat("ForwardState", 1);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
             
        }

    }
}
