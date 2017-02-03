using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class HeadAdjustmentState : AStateMachineBehaviour
    {
        private GameObject mHeadAdjustmentObject;
        private GameObject mWindowAppOverWhite;

        private Animator mHeadAdjustmentAnimator;
        private Animator mBackgroundBlackAnimator;

        public override void Init()
        {
            mHeadAdjustmentObject = GetGameObject(6);
            mWindowAppOverWhite = GetGameObject(3);

            mHeadAdjustmentAnimator = mHeadAdjustmentObject.GetComponent<Animator>();
            mBackgroundBlackAnimator = GetGameObject(1).GetComponent<Animator>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRGBCam.Open();
            mHeadAdjustmentObject.SetActive(true);
            mWindowAppOverWhite.SetActive(true);
            mHeadAdjustmentAnimator.SetTrigger("Open_WHeadController");
            mBackgroundBlackAnimator.SetTrigger("Close_BG");
            mMood.Set(MoodType.HAPPY);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRGBCam.Close();
            mHeadAdjustmentObject.SetActive(false);
            mWindowAppOverWhite.SetActive(false);           
            mHeadAdjustmentAnimator.SetTrigger("Close_WHeadController");
            iAnimator.SetBool("DoStartCount", true);
            iAnimator.SetInteger("ForwardState", 2);
            mFace.LookAt(FaceLookAt.CENTER);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {        
        }
    }
}
