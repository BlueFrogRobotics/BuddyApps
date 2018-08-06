using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.TakePose
{
    public class Hashtag : AStateMachineBehaviour
    {
        private bool mHasDisplayNotif;
        private bool mReadyToExit;

        public override void Start()
        {

        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.HAPPY);
            Buddy.Vocal.SayKey("sharetwitter");

            mHasDisplayNotif = false;
            mReadyToExit = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mHasDisplayNotif) {
                StartCoroutine(DisplayNotif());
                mHasDisplayNotif = true;
            }

            if (!Buddy.Vocal.IsSpeaking && mReadyToExit)
                Trigger("Exit");
        }

        private IEnumerator DisplayNotif()
        {
            Buddy.GUI.Notifier.Display<SimpleNotification>().With("#SocialRobot @adoptbuddy",
                Buddy.Resources.Get<Sprite>("Ico_Twitter"), 4F);

            yield return new WaitForSeconds(5F);

            mReadyToExit = true;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            
        }
    }
}