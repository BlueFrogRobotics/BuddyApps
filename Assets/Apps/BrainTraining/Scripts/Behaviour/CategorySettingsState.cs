using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BrainTraining
{

    public class CategorySettingsState : AStateMachineBehaviour
    {
        private BrainTrainingBehaviour mBrainTrainingBehaviour;

        public override void Start()
        {
            mBrainTrainingBehaviour = GetComponent<BrainTrainingBehaviour>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            QuizzType type = (QuizzType)GetInteger("CategoryType");
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString(type.ToString().ToLower()));
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                for (int i = 0; i < mBrainTrainingBehaviour.Categories.Categories.Count; i++)
                {                    
                    CategoryData category = mBrainTrainingBehaviour.Categories.Categories[i];
                    if (category.Type == type)
                    {
                        TToggle toggle = iBuilder.CreateWidget<TToggle>();
                        toggle.SetLabel(category.Category);
                        toggle.ToggleValue = category.IsActive;
                        toggle.OnToggle.Add((param) => { category.IsActive = param; });
                    }
                }
            }, () => {
                Next();
            }, "Cancel",
             () => {
                 mBrainTrainingBehaviour.SaveCategories();
                 Next();
             }, "OK");
        }

        private void Next()
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Trigger("Back");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

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
