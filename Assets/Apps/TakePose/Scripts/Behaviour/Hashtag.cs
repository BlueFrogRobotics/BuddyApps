using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Buddy;
using Buddy.UI;

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
            Interaction.Mood.Set(MoodType.HAPPY);
            Interaction.TextToSpeech.SayKey("sharetwitter");

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

            if (Interaction.TextToSpeech.HasFinishedTalking && mReadyToExit)
                Trigger("Exit");
        }

        private IEnumerator DisplayNotif()
        {
            Notifier.Display<SimpleNot>(4F).With("#SocialRobot @adoptbuddy", 
                Resources.GetSpriteFromAtlas("Ico_Twitter"), Color.blue);

            yield return new WaitForSeconds(5F);

            mReadyToExit = true;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }
    }
}