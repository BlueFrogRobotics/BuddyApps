using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;
using BuddyFeature.Web;
using System;

namespace BuddyApp.BabyPhone
{
    public class SoundDetectedState : AStateMachineBehaviour
    {
        private GameObject mSoundDetect;
        private GameObject mWindoAppOverBlack;

        private const int STATE_TIME = 5;
        private float mTime;

        /// <summary>
        /// This variable defines the action user has selected if baby cries, or an unusual sund is deteced
        /// </summary>
        private int mIfBabyCries;

        public override void Init()
        {
            mWindoAppOverBlack = GetGameObject(2);
            mSoundDetect = GetGameObject(11);
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mSoundDetect.SetActive(true);
            mWindoAppOverBlack.SetActive(true);

            mTime = 0;
            mMood.Set(MoodType.SAD);

            //get the last update of action when baby cries
            mIfBabyCries = (int)BabyPhoneData.Instance.ActionWhenBabyCries;
            UpdateFallingAssleep();
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mSoundDetect.SetActive(false);
            mWindoAppOverBlack.SetActive(false);

            iAnimator.SetInteger("ForwardState", 4);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTime += Time.deltaTime;

            //after changing buddy face to sad, whaite some time and then do to next step
            if (mTime >= STATE_TIME)
            {
                if(mIfBabyCries != 0)
                    iAnimator.SetTrigger("StartFallingAssleep");
                else
                    iAnimator.SetTrigger("StartListening");
            }           
        }

        /// <summary>
        /// Update Baby Phone data for the next action 
        /// </summary>
        private void UpdateFallingAssleep()
        {
            switch (mIfBabyCries)
            {
                case 1:
                    // replay lullaby 
                    BabyPhoneData.Instance.IsVolumeOn = true;
                    BabyPhoneData.Instance.IsAnimationOn = false;
                    break;
                case 2:
                    // replay animation 
                    BabyPhoneData.Instance.IsVolumeOn = false;
                    BabyPhoneData.Instance.IsAnimationOn = true;
                    break;
                case 3:
                    // replya both
                    BabyPhoneData.Instance.IsVolumeOn = true;
                    BabyPhoneData.Instance.IsAnimationOn = true;
                    break;
                default:
                    BabyPhoneData.Instance.IsVolumeOn = false;
                    BabyPhoneData.Instance.IsAnimationOn = false;
                    break;
            }
        }

    }
}
