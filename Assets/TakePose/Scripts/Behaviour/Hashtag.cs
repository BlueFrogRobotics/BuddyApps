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

        public override void Start()
        {

        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mMood.Set(MoodType.HAPPY);
            mTTS.SayKey("sharetwitter");

            mNotifier.Display<SimpleNot>().With("#SocialRobot @adoptbuddy", BYOS.Instance.ResourceManager.GetSprite("Ico_Twitter"), Color.blue);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mTTS.HasFinishedTalking)
                Trigger("Exit");
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mMood.Set(MoodType.NEUTRAL);
        }
    }
}