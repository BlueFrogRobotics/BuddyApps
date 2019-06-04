using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PTwoDetect : AStateMachineBehaviour
    {
        private bool mDetectEnabled;
        private bool mHumanDetectEnabled;
        private float mAngleSequence = 0F;
        private bool mFirstStep;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mFirstStep = false;
            HeadPositionDetect();
            mDetectEnabled = false;
            mHumanDetectEnabled = false;
            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect,
            new HumanDetectorParameter { SensorMode = SensorMode.VISION });
            
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mAngleSequence < 360F)
                return;
            else
            {
                Trigger("");
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private void HeadPositionDetect()
        {
            if (Buddy.Actuators.Head.No.Angle != 0F)
                Buddy.Actuators.Head.No.ResetPosition();
            if(Buddy.Actuators.Head.Yes.Angle != 5F)
                Buddy.Actuators.Head.Yes.SetPosition(5F, 45F);
        }

        private void StartDetect()
        {
            if (mDetectEnabled)
                return;

        


        }

        private bool OnHumanDetect(HumanEntity[] iHumanEntity)
        {
            if(!mFirstStep)
            {
                mFirstStep = true;
                StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                {
                    Buddy.Actuators.Head.No.SetPosition(-100F, 45F);
                }));
            }

            double lMaxArea = iHumanEntity.Max(h => h.BoundingBox.area());
            HumanEntity lBiggestHuman = iHumanEntity.First(h => h.BoundingBox.area() == lMaxArea);

            OutOfBoxUtils.DebugColor("Human detected", "blue");
            return true;
        }

    }

}
