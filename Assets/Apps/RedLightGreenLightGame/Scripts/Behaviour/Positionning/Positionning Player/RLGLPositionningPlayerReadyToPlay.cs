using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLPositionningPlayerReadyToPlay : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(Recoil());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator Recoil()
        {
            yield return SayKeyAndWait("willrecoil");
            Interaction.TextToSpeech.SayKey("smallrules");
            Primitive.Motors.Wheels.MoveToAbsolutePosition(mRLGLBehaviour.StartingOdometry, -250f, 0.1f);
            while ((Primitive.Motors.Wheels.Odometry - mRLGLBehaviour.StartingOdometry).magnitude>1)
            {
                Debug.Log("magnitude: "+(Primitive.Motors.Wheels.Odometry - mRLGLBehaviour.StartingOdometry).magnitude);

                //if(!ObstacleInback())
                //    Primitive.Motors.Wheels.SetWheelsSpeed(-200f);
                //else
                //    Primitive.Motors.Wheels.SetWheelsSpeed(0f);
                yield return null;
            }
            Primitive.Motors.Wheels.Stop();
            Trigger("Sentence");
        }

        private bool ObstacleInback()
        {
            if (Primitive.USSensors.Back.Distance < 1.5f && Primitive.USSensors.Back.Distance != 0)
                return true;

            return false;
        }
    }

}
