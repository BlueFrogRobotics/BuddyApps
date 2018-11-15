using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public class DisplayPhotoState : AStateMachineBehaviour
    {
        private readonly string STR_QUIT_COMMAND = "quit";
        private readonly string STR_NEXT_COMMAND = "next";
        private readonly string STR_PREVIOUS_COMMAND = "previous";
        private readonly string STR_DELETE_COMMAND = "delete";
        private readonly string STR_SHARE_COMMAND = "share";

        private readonly string STR_REMOVE_SPRITE = "os_icon_trash";
        private readonly string STR_SHARE_SPRITE = "os_icon_share";

        private readonly float F_MAX_TIME_LISTENING = 10.0F;

        [SerializeField]
        private bool mIsFooterSet = false;
        
        [SerializeField]
        private float mTimeListening;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On State Enter...");

            mTimeListening = F_MAX_TIME_LISTENING;

            // New listening events
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnEndListening);
            
            Buddy.Vocal.OnListeningStatus.Add((SpeechInputStatus t) => ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "MY STATUS : " + t.Type));

            // Check if footer is correctly displayed
            UpdateFooter();
            
            if (Buddy.Vocal.IsSpeaking)
            {
                Buddy.Vocal.OnEndSpeaking.Clear();
                Buddy.Vocal.OnEndSpeaking.Add(OnEndSpeaking);
            }
            else
            {
                Buddy.Vocal.Listen();
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Buddy.Vocal.IsListening) {
                mTimeListening -= Time.deltaTime;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");
            Buddy.Vocal.OnEndListening.Clear();
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        public void OnEndSpeaking(SpeechOutput iSpeechOutput)
        {
            if (0.0F <= mTimeListening)
            {
                Buddy.Vocal.Listen();
            }
        }

        public void OnEndListening (SpeechInput iSpeechInput)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "RULE : " + iSpeechInput.Rule);
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "UTTERANCE : " + iSpeechInput.Utterance);
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "CONFIDENCE : " + iSpeechInput.Confidence);

            if (iSpeechInput.Confidence < Buddy.Vocal.DefaultInputParameters.RecognitionThreshold)
            {
                if (0.0F <= mTimeListening)
                {
                    Buddy.Vocal.Listen();
                }
                return;
            }
            
            if (Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_QUIT_COMMAND))
            {
                QuitApp();
                return;
            }
            
            if (0 < PhotoManager.GetInstance().GetCount()
                && Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_DELETE_COMMAND))
            {
                Trigger("TRIGGER_REMOVE_REQUEST");
                return;
            }

            if (0 < PhotoManager.GetInstance().GetCount()
                && Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_NEXT_COMMAND)
                && PhotoManager.GetInstance().GetCurrentIndex() < PhotoManager.GetInstance().GetCount() - 1 // Not last index
                && PhotoManager.GetInstance().GetSlideSet().GoNext())
            {
                Trigger("TRIGGER_CHANGE_PHOTO");
                return;
            }

            if (0 < PhotoManager.GetInstance().GetCount()
                && Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_PREVIOUS_COMMAND)
                && 0 < PhotoManager.GetInstance().GetCurrentIndex() - 1 // Not last index
                && PhotoManager.GetInstance().GetSlideSet().GoPrevious())
            {
                Trigger("TRIGGER_CHANGE_PHOTO");
                return;
            }

            if (0 < PhotoManager.GetInstance().GetCount()
                && Utils.GetRealStartRule(iSpeechInput.Rule).EndsWith(STR_SHARE_COMMAND))
            {
                Trigger("TRIGGER_SHARE_PHOTO");
                return;
            }

            if (0.0F <= mTimeListening) {
                ExtLog.W(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "START LISTENING!!! " + mTimeListening);
                Buddy.Vocal.Listen();
            }
        }
        
        private void UpdateFooter()
        {
            if (0 == PhotoManager.GetInstance().GetCount())
            {
                mIsFooterSet = false;
                return;
            }

            if (!mIsFooterSet && 0 < PhotoManager.GetInstance().GetCount())
            {
                InitializeFooter();
                mIsFooterSet = true;
            }
        }

        private void InitializeFooter()
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.READING, "Changing Footer... ");

            //
            /// Create remove button
            FButton lDeleteButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lDeleteButton.SetIcon(Buddy.Resources.Get<Sprite>(STR_REMOVE_SPRITE));
            lDeleteButton.OnClick.Add(() => { Trigger("TRIGGER_REMOVE_REQUEST"); });

            //
            /// Create share button
            FButton lShareButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lShareButton.SetIcon(Buddy.Resources.Get<Sprite>(STR_SHARE_SPRITE));
            lShareButton.OnClick.Add(() => { Trigger("TRIGGER_SHARE_PHOTO"); });
        }
    }
}
