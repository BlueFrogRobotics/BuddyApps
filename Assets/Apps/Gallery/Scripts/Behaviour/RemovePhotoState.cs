using BlueQuark;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public class RemovePhotoState : AStateMachineBehaviour
    {
        private readonly string STR_ASK_FOR_VALIDATION = "deletevalidation";
        private readonly string STR_CONFIRM_DELETION = "picturedeleted";

        private readonly string STR_CONFIRM = "accept";
        private readonly string STR_CANCEL = "refuse";

        private readonly string STR_YES = "yes";
        private readonly string STR_NO = "no";

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Stop current vocal process
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.Stop();

            // Start new process
            Buddy.Vocal.OnEndListening.Add((iSpeechInput) => OnEndListening(iSpeechInput));
            Buddy.Vocal.SayKey(STR_ASK_FOR_VALIDATION);

            // Display graphical interface
            InitializeDialoger();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // TODO: LISTEN ONLY FOR EDITOR VERSION!!!
            /*
            if (!Buddy.Vocal.IsListening)
            {
                string[] grammars = { "app_grammar", "gallery" };
                Buddy.Vocal.Listen(grammars);
            }
            */
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.GUI.Dialoger.Hide();
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        private void InitializeDialoger()
        {
            Buddy.GUI.Dialoger.Display<ParameterToast>().With(
                (iBuilder) => {
                    iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetRandomString(STR_ASK_FOR_VALIDATION));
                },
                RemoveCanceled, Buddy.Resources.GetString(STR_CANCEL),
                RemoveConfirmed, Buddy.Resources.GetString(STR_CONFIRM)
            );
        }

        private void RemoveConfirmed()
        {
            // Delete photo (slide + disk)
            if (!PhotoManager.GetInstance().DeleteCurrentPhoto()) {
                // Error could not erase image
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Failed to delete current image.");
            }

            Buddy.Vocal.Stop();
            Buddy.Vocal.SayKey(STR_CONFIRM_DELETION);
            Trigger("TRIGGER_REMOVE_CONFIRM");
        }

        private void RemoveCanceled()
        {
            Buddy.Vocal.Stop();
            Trigger("TRIGGER_REMOVE_CANCEL");
        }

        private void OnEndListening(SpeechInput iSpeechInput)
        {
            if (string.IsNullOrEmpty(iSpeechInput.Rule)) {
                return;
            }

            if (string.Equals(iSpeechInput.Rule, STR_NO)) {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Cancel");
                RemoveCanceled();
            } else if (string.Equals(iSpeechInput.Rule, STR_YES)) {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Confirm");
                RemoveConfirmed();
            }
        }
    }
}
