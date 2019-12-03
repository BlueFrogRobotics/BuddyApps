using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.MemoryGame
{
	public class ParametersState : AStateMachineBehaviour
	{
        bool mMoveHead;
        bool mMoveBody;
        int mDiffultyLevel;

		public override void Start()
		{
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            mMoveHead = MemoryGameData.Instance.MoveHead;
            mMoveBody = MemoryGameData.Instance.MoveBody;
            mDiffultyLevel = MemoryGameData.Instance.Difficulty + 1;

            DisplayParameters();
        }


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}


        private void DisplayParameters()
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("settingslabel"));

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                TText lText = iBuilder.CreateWidget<TText>();
                lText.SetLabel("    " + Buddy.Resources.GetString("difficultylabel"));
                lText.SetCenteredLabel(false);

                TRate lRate = iBuilder.CreateWidget<TRate>();
                // Workaround, waiting for TRate fix
                lRate.RateValue = mDiffultyLevel > 2 ? mDiffultyLevel : mDiffultyLevel - 1;
                lRate.OnRate.Add((rate) => {
                    mDiffultyLevel = rate;
                });

                TToggle lToggleAdd = iBuilder.CreateWidget<TToggle>();
                lToggleAdd.SetLabel(Buddy.Resources.GetString("movehead"));
                lToggleAdd.ToggleValue = mMoveHead;
                lToggleAdd.OnToggle.Add((isOn) => { mMoveHead = isOn; });

                TToggle lToggleSub = iBuilder.CreateWidget<TToggle>();
                lToggleSub.SetLabel(Buddy.Resources.GetString("movebody"));
                lToggleSub.ToggleValue = mMoveBody;
                lToggleSub.OnToggle.Add((isOn) => { mMoveBody = isOn; });

            }, () =>
            {
                NextStep();
            }, "Cancel",
            () =>
            {
                SaveParameters();
                NextStep();
            }, "OK");
        }

        private void NextStep()
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Header.DisplayParametersButton(true);
            Buddy.GUI.Toaster.Hide();
            Trigger("StartGame");
        }

        private void SaveParameters()
        {
            MemoryGameData.Instance.MoveBody = mMoveBody;
            MemoryGameData.Instance.MoveHead = mMoveHead;
            MemoryGameData.Instance.Difficulty = mDiffultyLevel - 1;
        }
    }
}
