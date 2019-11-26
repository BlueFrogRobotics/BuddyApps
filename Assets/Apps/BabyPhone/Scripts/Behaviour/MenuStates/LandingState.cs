using BlueQuark;

using UnityEngine;


namespace BuddyApp.BabyPhone
{
    /// <summary>
    /// State that init that activate the detections chosen by the user and pass to the next mode state
    /// </summary>
    public sealed class LandingState : AStateMachineBehaviour
    {

        public override void Start()
        {
            Buddy.Vocal.EnableTrigger = true;
            Buddy.GUI.Header.DisplayParametersButton(false);
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (BabyPhoneData.Instance.FirstRun)
            {
                BabyPhoneData.Instance.FirstRun = false;
                Trigger("Parameter");
            }
            else
                Trigger("HeadPosition");
        } 

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }


    }
}