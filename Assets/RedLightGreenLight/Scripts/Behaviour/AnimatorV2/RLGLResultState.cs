using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLight
{
    public class RLGLResultState : AStateMachineBehaviour
    {
        private float mTimer;
        private bool mIsMovementDone;
        private bool mIsSentenceDone;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsSentenceDone = false;
            mIsMovementDone = false;
            mTimer = 0.0f;
            Primitive.Motors.Wheels.TurnAngle(-180.0F, 250.0F, 0.5F);
            Primitive.Motors.YesHinge.SetPosition(0.0F, 150.0F);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if ((Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || 
                (Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS &&
                mTimer < 2.0F)) && !mIsMovementDone)
            {
                mIsMovementDone = true;
                Interaction.Mood.Set(MoodType.HAPPY);
            }

            if (Interaction.TextToSpeech.HasFinishedTalking && mTimer < 6.0f && !mIsSentenceDone && mIsMovementDone)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("resultState1"));
                //mTTS.Say("Good job you won, you have been too fast for me!");
                mIsSentenceDone = true;
            }
            if (Interaction.TextToSpeech.HasFinishedTalking && mIsSentenceDone)
                Interaction.Mood.Set(MoodType.NEUTRAL);
            if (mTimer > 6.0f)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("resultState2"));
                //do you want to replay?
                BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetRandomString("resultState2"),
                    () => Trigger("Replay"),
                    () => NoReplay()
                );
                animator.SetBool("IsReplayTrue", true);
            }

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private void NoReplay()
        {

        }

    }
}

