using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public class CameraState :  AStateMachineBehaviour
    {

        private TToggle mToggleMotionDetect;
        private TToggle mToggleFaceDetect;
        private TToggle mToggleShowSkeleton;
        private TToggle mToggleHumanDetect;
        private TToggle mToggleTakePhoto;

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                Buddy.Vocal.Say("salut mon ami");
                // A boolean toggle
                mToggleMotionDetect = iBuilder.CreateWidget<TToggle>();
                mToggleFaceDetect = iBuilder.CreateWidget<TToggle>();
                mToggleShowSkeleton = iBuilder.CreateWidget<TToggle>();
                mToggleHumanDetect = iBuilder.CreateWidget<TToggle>();
                mToggleTakePhoto = iBuilder.CreateWidget<TToggle>();
                mToggleMotionDetect.SetLabel("Motion Detect");
                mToggleFaceDetect.SetLabel("Face Detect");
                mToggleShowSkeleton.SetLabel("Show Skeleton");
                mToggleHumanDetect.SetLabel("Human Detect");
                mToggleTakePhoto.SetLabel("Take Photo");
            },

            // Cancel button callback
            () => { Debug.Log("Click cancel"); Buddy.GUI.Toaster.Hide(); Trigger("MenuTrigger"); },
            // And label
            "Cancel",

             // next button callback
             () => {
                 Debug.Log("Click next");

                 Buddy.GUI.Toaster.Hide();
             },
            // And label
            "Next"
             );
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}

