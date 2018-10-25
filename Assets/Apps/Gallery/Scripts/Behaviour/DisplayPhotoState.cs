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

        private readonly string STR_REMOVE_SPRITE = "os_buddy_img";
        private readonly string STR_SHARE_SPRITE = "os_icon_share";
        
        private bool mIsFooterSet = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // New listening events
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnEndListening);

            // Check if footer is correctly displayed
            UpdateFooter();
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
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        public void OnEndListening (SpeechInput iSpeechInput)
        {
            ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.OUT_OF_BOUND, "RULE : " + iSpeechInput.Rule);

            if (string.IsNullOrEmpty(iSpeechInput.Rule))
            {
                return;
            }
            
            if (string.Equals(iSpeechInput.Rule, STR_QUIT_COMMAND))
            {
                QuitApp();
                return;
            }
            
            if (string.Equals(iSpeechInput.Rule, STR_DELETE_COMMAND))
            {
                Trigger("TRIGGER_REMOVE_REQUEST");
                return;
            }

            if (string.Equals(iSpeechInput.Rule, STR_NEXT_COMMAND)
                && PhotoManager.GetInstance().GetSlideSet().GoNext())
            {
                Trigger("TRIGGER_CHANGE_PHOTO");
                return;
            }

            if (string.Equals(iSpeechInput.Rule, STR_PREVIOUS_COMMAND)
                && PhotoManager.GetInstance().GetSlideSet().GoPrevious())
            {
                Trigger("TRIGGER_CHANGE_PHOTO");
                return;
            }

            if (string.Equals(iSpeechInput.Rule, STR_SHARE_COMMAND))
            {
                Trigger("TRIGGER_SHARE_PHOTO");
                return;
            }
        }
        
        private void UpdateFooter()
        {
            if (0 == PhotoManager.GetInstance().GetCount())
            {
                Buddy.GUI.Footer.Hide();
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
            //
            /// Create remove button
            FButton lDeleteButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lDeleteButton.SetIcon(Buddy.Resources.Get<Sprite>(STR_REMOVE_SPRITE));
            lDeleteButton.SetBackgroundColor(Color.red);
            lDeleteButton.OnClick.Add(() => {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.OUT_OF_BOUND, "DELETE 2"); Trigger("TRIGGER_REMOVE_REQUEST"); });

            //
            /// Create share button
            FButton lShareButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lShareButton.SetIcon(Buddy.Resources.Get<Sprite>(STR_SHARE_SPRITE));
            lShareButton.SetBackgroundColor(Color.cyan);
            lShareButton.OnClick.Add(() => { Trigger("TRIGGER_SHARE_PHOTO"); });
        }
    }
}
