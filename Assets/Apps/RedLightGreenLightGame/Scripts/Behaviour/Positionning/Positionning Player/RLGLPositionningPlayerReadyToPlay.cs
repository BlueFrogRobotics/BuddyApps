using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLPositionningPlayerReadyToPlay : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mSentenceDone;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
            mSentenceDone = false;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(Recoil());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(mSentenceDone)
                mRLGLBehaviour.TimerMove -= Time.deltaTime;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(Buddy.MoodType.NEUTRAL);
        }

        private IEnumerator Recoil()
        {
            yield return SayKeyAndWait("willrecoil");
            Interaction.TextToSpeech.SayKey("smallrules");
            mSentenceDone = true;
            Primitive.Motors.YesHinge.SetPosition(45.0F, 150.0F);
            while (mRLGLBehaviour.TimerMove > 0F)
            {
                if (!ObstacleInback())
                {
                    Primitive.Motors.Wheels.SetWheelsSpeed(-200F);
                }
                else
                {
                    Primitive.Motors.Wheels.SetWheelsSpeed(0F);
                }
                yield return null;
            }
            //Primitive.Motors.Wheels.SetWheelsSpeed(-200F);
            Primitive.Motors.Wheels.Stop();
            mRLGLBehaviour.TimerMove = 0F;
            Trigger("Sentence");

        }

        private bool ObstacleInback()
        {
            if (Primitive.USSensors.Back.Distance < 0.5F && Primitive.USSensors.Back.Distance != 0)
            {
                mSentenceDone = false;
                return true;
            }

            mSentenceDone = true;
            return false;
        }
    }

}
