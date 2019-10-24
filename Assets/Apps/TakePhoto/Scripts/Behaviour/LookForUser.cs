using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.TakePhoto
{
    public sealed class LookForUser : AStateMachineBehaviour
    {
        private float mTimer;


        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0F;

            Buddy.Perception.HumanDetector.OnDetect.Add(HumanDetected);
        }

        private void HumanDetected(HumanEntity[] iHumanEntities)
        {
            if (iHumanEntities.Length > 0) {
                Buddy.Perception.HumanDetector.OnDetect.Remove(HumanDetected);
                mTimer = 0F;

                // Get the biggest area
                double lMaxArea = 0;
                double lMeanX = 0;
                double lMeanY = 0;
                int lNbHumans = 1;

                if (iHumanEntities.Length == 1) {
                    lMeanX += (iHumanEntities[0].Center.x / Buddy.Sensors.RGBCamera.Width);
                    lMeanY += (iHumanEntities[0].Center.y + iHumanEntities[0].BoundingBox.y) / (2 * Buddy.Sensors.RGBCamera.Height);
                } else {

                    lNbHumans = 0;

                    for (int i = 0; i < iHumanEntities.Length; ++i) {
                        if (iHumanEntities[i].BoundingBox.area() > lMaxArea)
                            lMaxArea = iHumanEntities[i].BoundingBox.area();
                    }

                    // Get centered position of closest humans
                    for (int i = 0; i < iHumanEntities.Length; ++i) {
                        if (iHumanEntities[i].BoundingBox.area() * 1.3 > lMaxArea) {
                            lMeanX += (iHumanEntities[i].Center.x / Buddy.Sensors.RGBCamera.Width);
                            lMeanY += (iHumanEntities[i].Center.y + iHumanEntities[i].BoundingBox.y) / (2 * Buddy.Sensors.RGBCamera.Height);
                            lNbHumans++;
                        }
                    }
                }

                lMeanX /= lNbHumans;
                lMeanY /= lNbHumans;


                float lNoAngle = Buddy.Actuators.Head.No.Angle + ((float)lMeanX - 0.5F) * 60F;
                float lYesAngle = Buddy.Actuators.Head.Yes.Angle + ((float)lMeanY - 0.5F) * 49.5F;
                Buddy.Actuators.Head.SetPosition((float)lMeanX, (float)lMeanY);
                Trigger("Photo");

            }
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            if (mTimer > 2F)
                Trigger("Photo");

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Perception.HumanDetector.OnDetect.Remove(HumanDetected);
        }
    }
}