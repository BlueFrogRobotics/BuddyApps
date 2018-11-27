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
        
        private readonly float F_MAX_TIME_LISTENING = 10.0f;

        private float mFTimeListening;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On State Enter...");

            mFTimeListening = F_MAX_TIME_LISTENING;

            // Stop current vocal process
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.GUI.Screen.OnTouch.Add(OnStopListening);

            // Start new listening process
            Buddy.Vocal.OnEndListening.Add(OnEndListening);
            Buddy.Vocal.SayKeyAndListen(STR_ASK_FOR_VALIDATION);

            // Display graphical interface
            InitializeDialoger();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Buddy.Vocal.IsListening)
            {
                mFTimeListening -= Time.deltaTime;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");

            Buddy.Vocal.OnEndListening.Clear();
            Buddy.GUI.Screen.OnTouch.Remove(OnStopListening);
            Buddy.GUI.Dialoger.Hide();
        }

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
            OnStopListening(null);
            Buddy.Vocal.StopAndClear();

            // Delete photo (slide + disk)
            if (!PhotoManager.GetInstance().DeleteCurrentPhoto())
            {
                // Error could not erase image
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Failed to delete current image.");
                Trigger("TRIGGER_REMOVE_CANCEL");
                return;
            }
            
            Buddy.Vocal.SayKey(STR_CONFIRM_DELETION);
            Trigger("TRIGGER_REMOVE_CONFIRM");
        }

        private void RemoveCanceled()
        {
            OnStopListening(null);
            Buddy.Vocal.StopAndClear();
            Trigger("TRIGGER_REMOVE_CANCEL");
        }

        private void OnEndListening(SpeechInput iSpeechInput)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "RULE : " + iSpeechInput.Rule);
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "UTTERANCE : " + iSpeechInput.Utterance);
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "CONFIDENCE : " + iSpeechInput.Confidence);

            if (iSpeechInput.IsInterrupted || -1 == iSpeechInput.Confidence) // Error during recognition or forced StopListening
            {
                return;
            }

            if (Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_NO))
            {
                RemoveCanceled();
                return;
            }

            if (Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_YES))
            {
                RemoveConfirmed();
                return;
            }

            if (0.0F <= mFTimeListening)
            {
                Buddy.Vocal.Listen();
            }
        }

        private void OnStopListening(Touch[] iTouch)
        {
            mFTimeListening = -1.0F;
        }
    }
}
