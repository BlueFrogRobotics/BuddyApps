using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BrainTraining
{

    public class SettingsState : AStateMachineBehaviour
    {
        private BrainTrainingBehaviour mBrainTrainingBehaviour;

        public override void Start()
        {
            mBrainTrainingBehaviour = GetComponent<BrainTrainingBehaviour>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("settings"));
            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                TVerticalListBox lBox = iBuilder.CreateBox();

                lBox.SetLabel("Catégories images");
                lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_pics", Context.OS));
                lBox.OnClick.Add(() => 
                {
                    SetInteger("CategoryType", (int)QuizzType.IMAGE);
                    Trigger("CategorySettings");
                });
                lBox = iBuilder.CreateBox();
                lBox.SetLabel("Catégories questions");
                lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_help", Context.OS));
                lBox.OnClick.Add(() =>
                {
                    SetInteger("CategoryType", (int)QuizzType.QUESTION);
                    Trigger("CategorySettings");
                });
                lBox = iBuilder.CreateBox();
                lBox.SetLabel("Catégories musiques");
                lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_music", Context.OS));
                lBox.OnClick.Add(() =>
                {
                    SetInteger("CategoryType", (int)QuizzType.MUSIC);
                    Trigger("CategorySettings");
                });
                lBox = iBuilder.CreateBox();
                lBox.SetLabel("Nombre de questions");
                lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_cogs", Context.OS));
                lBox.OnClick.Add(() => { Trigger("GeneralSettings"); });
            });

            FButton validateButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            validateButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));
            validateButton.SetBackgroundColor(Utils.BUDDY_COLOR);
            validateButton.SetIconColor(Color.white);

            validateButton.OnClick.Add(() => 
            {
                mBrainTrainingBehaviour.ResetQuestionList();
                Trigger("Start");
            });
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
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
