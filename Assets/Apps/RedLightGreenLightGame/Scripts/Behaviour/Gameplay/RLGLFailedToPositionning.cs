using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLFailedToPositionning : AStateMachineBehaviour
    {
        private bool mIsSentenceDone;
        private TextToSpeech mTTS;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsSentenceDone = false;
            mTTS = Interaction.TextToSpeech;
            Interaction.Mood.Set(MoodType.THINKING);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!mIsSentenceDone)
            {
                mTTS.SayKey("failedtopositionning");
                mIsSentenceDone = true;
            }

            if (mTTS.HasFinishedTalking && mIsSentenceDone)
                animator.SetTrigger("Start");
            
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

    }
}

