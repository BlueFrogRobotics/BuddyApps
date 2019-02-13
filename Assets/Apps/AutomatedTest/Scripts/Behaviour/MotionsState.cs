using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public class MotionsState : AStateMachineBehaviour
    {
        private TToggle mToggleRotate;
        private TToggle mToggleRotateYes;
        private TToggle mToggleRotateNo;
        private TToggle mToggleMoveForward;
        private TToggle mToggleMoveBackward;
        private TToggle mToggleObstacleStop;
        private TToggle mToggleObstacleAvoidance;

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                Buddy.Vocal.Say("salut mon ami");
                // A boolean toggle
                mToggleRotate = iBuilder.CreateWidget<TToggle>();
                mToggleRotateYes = iBuilder.CreateWidget<TToggle>();
                mToggleRotateNo = iBuilder.CreateWidget<TToggle>();
                mToggleMoveForward = iBuilder.CreateWidget<TToggle>();
                mToggleMoveBackward = iBuilder.CreateWidget<TToggle>();
                mToggleObstacleStop = iBuilder.CreateWidget<TToggle>();
                mToggleObstacleAvoidance = iBuilder.CreateWidget<TToggle>();

                mToggleRotate.OnToggle.Add(OnToggleRotate);

                mToggleRotate.SetLabel("Rotate");
                mToggleRotateYes.SetLabel("Rotate yes");
                mToggleRotateNo.SetLabel("Rotate no");
                mToggleMoveForward.SetLabel("Move forward");
                mToggleMoveBackward.SetLabel("Move backward");
                mToggleObstacleStop.SetLabel("Obstacle stop");
                mToggleObstacleAvoidance.SetLabel("Obstacle avoidance");
            },
            
            () => {
                Debug.Log("Click cancel");
                Buddy.GUI.Toaster.Hide();
                Trigger("MenuTrigger");
            },
            "Cancel",
            
            () => {
                Debug.Log("Click next");
                Buddy.GUI.Toaster.Hide();
                Trigger("RunTrigger");
            },
            "Next"
            );
        }

        //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{ 

        //}

        //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{

        //}

        private void OnToggleRotate(bool iValue)
        {
            if (iValue)
                AutomatedTestData.Instance.TestOptions.Add("Rotate");
        }

        private void OnToggleRotateYes(bool iValue)
        {
            if (iValue)
                AutomatedTestData.Instance.TestOptions.Add("RotateYes");
        }

        private void OnToggleRotateNo(bool iValue)
        {
            if (iValue)
                AutomatedTestData.Instance.TestOptions.Add("RotateNo");
        }

        private void OnToggleMoveForward(bool iValue)
        {
            if (iValue)
                AutomatedTestData.Instance.TestOptions.Add("MoveForward");
        }

        private void OnToggleMoveBackward(bool iValue)
        {
            if (iValue)
                AutomatedTestData.Instance.TestOptions.Add("MoveBackward");
        }

        private void OnToggleObstacleStop(bool iValue)
        {
            if (iValue)
                AutomatedTestData.Instance.TestOptions.Add("Obstacle Stop");
        }

        private void OnToggleObstacleAvoidance(bool iValue)
        {
            if (iValue)
                AutomatedTestData.Instance.TestOptions.Add("Obstacle Avoidance");
        }

    }
}
