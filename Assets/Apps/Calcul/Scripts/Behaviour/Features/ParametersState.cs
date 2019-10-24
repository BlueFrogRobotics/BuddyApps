using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BlueQuark;

namespace BuddyApp.Calcul
{
    public class ParametersState : AStateMachineBehaviour
    {
        private int mCurrentLevel;
        private GameParameters mGameParameters;

        public override void Start()
        {
            mGameParameters = User.Instance.GameParameters;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mCurrentLevel = mGameParameters.Difficulty > 0 ? mGameParameters.Difficulty : 1;
            DisplayParameters();
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
                lRate.RateValue = mCurrentLevel > 2 ? mCurrentLevel : mCurrentLevel - 1;
                lRate.OnRate.Add((rate) => {
                    mCurrentLevel = rate;
                });

                TToggle lToggleAdd = iBuilder.CreateWidget<TToggle>();
                lToggleAdd.SetLabel(Buddy.Resources.GetString("additionlabel"));
                lToggleAdd.ToggleValue = mGameParameters.CheckOperand(Operand.ADD);
                lToggleAdd.OnToggle.Add((isOn) => {mGameParameters.SetOperand(Operand.ADD, isOn);});

                TToggle lToggleSub = iBuilder.CreateWidget<TToggle>();
                lToggleSub.SetLabel(Buddy.Resources.GetString("subtractionlabel"));
                lToggleSub.ToggleValue = mGameParameters.CheckOperand(Operand.SUB);
                lToggleSub.OnToggle.Add((isOn) => { mGameParameters.SetOperand(Operand.SUB, isOn); });

                TToggle lToggleMulti = iBuilder.CreateWidget<TToggle>();
                lToggleMulti.SetLabel(Buddy.Resources.GetString("multiplylabel"));
                lToggleMulti.ToggleValue = mGameParameters.CheckOperand(Operand.MULTI);
                lToggleMulti.OnToggle.Add((isOn) => { mGameParameters.SetOperand(Operand.MULTI, isOn); });

                TToggle lToggleDiv = iBuilder.CreateWidget<TToggle>();
                lToggleDiv.SetLabel(Buddy.Resources.GetString("divisionlabel"));
                lToggleDiv.ToggleValue = mGameParameters.CheckOperand(Operand.DIV);
                lToggleDiv.OnToggle.Add((isOn) => { mGameParameters.SetOperand(Operand.DIV, isOn); });

            }, () =>
            {
                NextStep();
            }, "Cancel",
            () =>
            {
                mGameParameters.Difficulty = mCurrentLevel;
                NextStep();
            }, "OK");
        }

        private void NextStep()
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Trigger("StartGame");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        ////override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        ////
        ////}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}
