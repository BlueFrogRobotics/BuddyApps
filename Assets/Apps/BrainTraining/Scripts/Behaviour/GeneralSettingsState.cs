using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BrainTraining
{

    public class GeneralSettingsState : AStateMachineBehaviour
    {
        private int mNbCategories;
        private int mNbQuestions;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("generalsettings"));
            mNbCategories = BrainTrainingData.Instance.NbCategories;
            mNbQuestions = BrainTrainingData.Instance.NbQuestions;

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                TText text = iBuilder.CreateWidget<TText>();
                text.SetLabel("Nombre de catégories");
                TTextField nbCategoriesTextField = iBuilder.CreateWidget<TTextField>();
                nbCategoriesTextField.SetText(mNbCategories.ToString());
                nbCategoriesTextField.OnChangeValue.Add(UpdateNbCategories);

                text = iBuilder.CreateWidget<TText>();
                text.SetLabel("Nombre de questions");
                TTextField nbQuestionsTextField = iBuilder.CreateWidget<TTextField>();
                nbQuestionsTextField.SetText(mNbQuestions.ToString());
                nbQuestionsTextField.OnChangeValue.Add(UpdateNbQuestions);

            }, () => {
                Next();
            }, "Cancel",
             () => {
                 BrainTrainingData.Instance.NbCategories = mNbCategories;
                 BrainTrainingData.Instance.NbQuestions = mNbQuestions;
                 Next();
             }, "OK");
        }

        private void UpdateNbCategories(string txt)
        {
            int.TryParse(txt, out mNbCategories);
        }

        private void UpdateNbQuestions(string txt)
        {
            int.TryParse(txt, out mNbQuestions);
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
