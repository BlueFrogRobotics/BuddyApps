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

        public override void Init()
        {
            mHeadAdjustmentObject = GetGameObject(6);
            mWindowAppOverWhite = GetGameObject(3);

            mHeadAdjustmentAnimator = mHeadAdjustmentObject.GetComponent<Animator>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRGBCam.Open();
            //HeadAdjustmentObject.SetActive(true);
            mWindowAppOverWhite.SetActive(true);
            mHeadAdjustmentAnimator.SetTrigger("Open_WHeadController");       
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRGBCam.Close();
           // HeadAdjustmentObject.SetActive(false);
            mWindowAppOverWhite.SetActive(false);           
            mHeadAdjustmentAnimator.SetTrigger("Close_WHeadController");
            iAnimator.SetBool("DoStartCount", true);
            iAnimator.SetInteger("ForwardState", 2);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {        
        }
    }
}
