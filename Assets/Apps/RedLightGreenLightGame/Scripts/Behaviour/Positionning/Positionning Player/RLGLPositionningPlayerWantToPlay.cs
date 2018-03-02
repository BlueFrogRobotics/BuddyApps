using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLPositionningPlayerWantToPlay : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            GetGameObject(1).GetComponent<Animator>().SetTrigger("close");
            Interaction.Mood.Set(Buddy.MoodType.HAPPY);
            Primitive.Motors.Wheels.Stop();
            mRLGLBehaviour.TargetClicked = false;
            Vector2 lDist = Primitive.Motors.Wheels.Odometry - mRLGLBehaviour.StartingOdometry;
            Debug.Log("dist: " + lDist);
            Debug.Log("odom start: " + Primitive.Motors.Wheels.Odometry.ToString() + " end odom: " + mRLGLBehaviour.StartingOdometry.ToString());
            if (lDist.magnitude < 1.5F)
                Trigger("RecoilQuestion");
            else
                Trigger("ReadyToPlay");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //GetGameObject(1).SetActive(false);
        }
    }

}
