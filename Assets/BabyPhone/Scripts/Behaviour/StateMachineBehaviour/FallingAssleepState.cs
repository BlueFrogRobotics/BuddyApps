using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class FallingAssleepState : AStateMachineBehaviour
    {
        private BabyPhoneData mBabyPhoneData;

        private GameObject mFallingAssleep;
        private GameObject mWindoAppOverWhite;

        private Animator mFallingAssleepAnimator;

        public override void Init()
        {
            mBabyPhoneData = BabyPhoneData.Instance;

            mWindoAppOverWhite = GetGameObject(3);
            mFallingAssleep = GetGameObject(7);
            mFallingAssleepAnimator = mFallingAssleep.GetComponent<Animator>();

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindoAppOverWhite.SetActive(true);
            mFallingAssleep.SetActive(true);           
            mFallingAssleepAnimator.SetTrigger("Open_WFallingAssleep");
            
            iAnimator.SetBool("DoPlayLullaby", true);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mFallingAssleep.SetActive(false);
            mFallingAssleepAnimator.SetTrigger("Close_WFallingAssleep");
            mWindoAppOverWhite.SetActive(false);
            iAnimator.SetBool("DoPlayLullaby", false);

            iAnimator.SetInteger("ForwardState", 3);

            //envoyer l'email d'information 
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }
    }
}
