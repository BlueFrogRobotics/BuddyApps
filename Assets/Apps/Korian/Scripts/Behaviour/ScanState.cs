using UnityEngine;
using BlueQuark;

namespace BuddyApp.Korian
{
    public sealed class ScanState : AStateMachineBehaviour
    {
        private float mTimeHumanDetected;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimeHumanDetected = Time.time;

            Buddy.Behaviour.SetMood(Mood.THINKING);

            Buddy.Actuators.Head.Yes.SetPosition(2);

            // Creation & Settings of parameters that will be used in detection
            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect,
                new HumanDetectorParameter
                {
                    SensorMode = SensorMode.VISION,
                }
            );

            Buddy.Actuators.Wheels.SetVelocities(0F, 45F);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            // Reset of the mood in case of false positive. (No human during 500ms)
            if (Time.time - mTimeHumanDetected > 0.500 && Buddy.Behaviour.Mood != Mood.THINKING && !Buddy.Behaviour.IsBusy)
                Buddy.Behaviour.SetMood(Mood.THINKING, false);
        }

        /*
        *   On a human detection this function is called.
        */
        private bool OnHumanDetect(HumanEntity[] iHumans)
        {
            Buddy.Behaviour.SetMood(Mood.HAPPY);
            mTimeHumanDetected = Time.time;
            double lCentered = iHumans[0].Center.x / Buddy.Sensors.RGBCamera.Width;
            if (lCentered < 0.65 && lCentered > 0.35)
            {
                Buddy.Actuators.Wheels.Stop();

                Trigger("GOTOUSER");
                return false;

                // otherwise, try to put the human in the center
            }
            else if (lCentered < 0.65)
                Buddy.Actuators.Wheels.SetVelocities(0F, 45F);
            else
                Buddy.Actuators.Wheels.SetVelocities(0F, -45F);

            return true;
        }
    }
}
