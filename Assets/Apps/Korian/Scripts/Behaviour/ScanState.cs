using UnityEngine;
using BlueQuark;

namespace BuddyApp.Korian
{
    public sealed class ScanState : AStateMachineBehaviour
    {

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {


            Buddy.Behaviour.SetMood(Mood.THINKING);

            Buddy.Actuators.Head.Yes.SetPosition(2);
            // Creation & Settings of parameters that will be used in detection
            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect,
                new HumanDetectorParameter {
                    SensorMode = SensorMode.VISION,
                    //YOLO = new YOLOParameter {
                    //    UseThermal = false,
                    //    //DetectFallenHuman = true,
                    //    DownSample = 1
                    //}
                }
            );

            Buddy.Actuators.Wheels.SetVelocities(0F, 45F);
        }



        /*
        *   On a human detection this function is called.
        */
        private bool OnHumanDetect(HumanEntity[] iHumans)
        {
            Buddy.Behaviour.SetMood(Mood.HAPPY);

            double lCentered = iHumans[0].Center.x / Buddy.Sensors.RGBCamera.Width;
            if (lCentered < 0.65 && lCentered > 0.35) {
                Buddy.Actuators.Wheels.Stop();

                Trigger("GOTOUSER");
                return false;

                // otherwise, try to put the human in the center
            } else if(lCentered < 0.65)
                Buddy.Actuators.Wheels.SetVelocities(0F, 45F);
            else
                Buddy.Actuators.Wheels.SetVelocities(0F, -45F);

            return true;
        }
    }
}
