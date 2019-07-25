using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using System.Collections;

namespace BuddyApp.Diagnostic
{

    //loga : distance, linear velocity, angular velocity, rotation velocity
    public sealed class MotorsWindow : MonoBehaviour
    {
        private DiagnosticBehaviour mDiagBehaviour = new DiagnosticBehaviour();

        [SerializeField]
        private Text XOdom;
        [SerializeField]
        private Text YOdom;
        [SerializeField]
        private Text Cap;
        [SerializeField]
        private Text NoAngle;
        [SerializeField]
        private Text YesAngle;


        /// <summary>
        /// Variables from the hardware
        /// </summary>
        [SerializeField]
        private Image YesHingeAngleGetterFeedbackT;
        [SerializeField]
        private Image YesHingeAngleGetterFeedbackB;
        [SerializeField]
        private Image NoHingeAngleGetterFeedbackL;
        [SerializeField]
        private Image NoHingeAngleGetterFeedbackR;

        /// <summary>
        /// Parameters for setwheelspeed
        /// </summary>
        [SerializeField]
        private Text linearVelocity;
        [SerializeField]
        private Slider linearVelocitySetter;

        /// <summary>
        /// Old MoveDistance
        /// </summary>
        [SerializeField]
        private Text distance;
        [SerializeField]
        private Slider distanceSetter;
        [SerializeField]
        private Text AngularVelocityWheelsText;
        [SerializeField]
        private Slider AngularVelocityWheelsSetter;
        [SerializeField]
        private Dropdown TimeMove;


        [SerializeField]
        private Text angleBack;
        [SerializeField]
        private Slider anglePosSetter;


        [SerializeField]
        private Slider yesHingeAngleSetter;
        [SerializeField]
        private Slider noHingeAngleSetter;
        [SerializeField]
        private Slider hingeSpeedSetter;

        [SerializeField]
        private Text yesAngleBack;
        [SerializeField]
        private Text noAngleBack;
        [SerializeField]
        private Text hingeSpeedBack;

        private Wheels mWheels;
        private YesHeadHinge mYesHinge;
        private NoHeadHinge mNoHinge;

        void Start()
        {
            NoAngle.text = Buddy.Actuators.Head.No.Angle.ToString("0.000");
            YesAngle.text = Buddy.Actuators.Head.Yes.Angle.ToString("0.000");

            XOdom.text = Buddy.Actuators.Wheels.Odometry.x.ToString("f3") + "m";
            YOdom.text = Buddy.Actuators.Wheels.Odometry.y.ToString("f3") + "m";
            Cap.text = Buddy.Actuators.Wheels.Odometry.z.ToString("0.000");

            mWheels = Buddy.Actuators.Wheels;
            mYesHinge = Buddy.Actuators.Head.Yes;
            mNoHinge = Buddy.Actuators.Head.No;

            linearVelocitySetter.wholeNumbers = false;
            linearVelocitySetter.value = 0.0F;
            linearVelocitySetter.minValue = -0.6F;
            linearVelocitySetter.maxValue = 0.6F;

            TimeMove.AddOptions(new List<string>() { "MOVE", "1s", "2s", "3s", "5s", "7s", "10s" });

            AngularVelocityWheelsSetter.wholeNumbers = true;
            AngularVelocityWheelsSetter.value = 0.0F;
            AngularVelocityWheelsSetter.minValue = -100F;
            AngularVelocityWheelsSetter.maxValue = 100F;

            anglePosSetter.wholeNumbers = true;
            anglePosSetter.value = 0.0F;
            anglePosSetter.minValue = 0F;
            anglePosSetter.maxValue = 360F;

            distanceSetter.wholeNumbers = false;
            distanceSetter.value = 0.0F;
            distanceSetter.minValue = 0F;
            distanceSetter.maxValue = 10F;

            yesHingeAngleSetter.wholeNumbers = true;
            yesHingeAngleSetter.value = 0F;
            yesHingeAngleSetter.minValue = -50F;
            yesHingeAngleSetter.maxValue = 37F;

            noHingeAngleSetter.wholeNumbers = true;
            noHingeAngleSetter.value = 0F;
            noHingeAngleSetter.minValue = -100F;
            noHingeAngleSetter.maxValue = 100F;

            hingeSpeedSetter.wholeNumbers = true;
            hingeSpeedSetter.value = 0F;
            hingeSpeedSetter.minValue = 0F;
            hingeSpeedSetter.maxValue = 100F;
        }

        void Update()
        {
            // Draw Head No Angle Feedback
            NoAngle.text = mNoHinge.Angle.ToString();
            if (mNoHinge.Angle >= 0) {
                NoHingeAngleGetterFeedbackL.fillAmount = 0;
                NoHingeAngleGetterFeedbackL.fillAmount = (mNoHinge.Angle / 90.00f) * 0.25f;
                NoHingeAngleGetterFeedbackR.fillAmount = 0;
            }
            if (mNoHinge.Angle <= 0) {
                NoHingeAngleGetterFeedbackL.fillAmount = 0;
                NoHingeAngleGetterFeedbackR.fillAmount = 0;
                NoHingeAngleGetterFeedbackR.fillAmount = (mNoHinge.Angle / 90.00f) * 0.25f;
            }
            // Draw Head Yes Angle Feedback
            YesAngle.text = mYesHinge.Angle.ToString();
            if (mYesHinge.Angle >= 0) {
                YesHingeAngleGetterFeedbackT.fillAmount = 0;
                YesHingeAngleGetterFeedbackT.fillAmount = (mYesHinge.Angle / 80.00f) * 0.25f;
                YesHingeAngleGetterFeedbackB.fillAmount = 0;
            }
            if (mNoHinge.Angle <= 0) {
                YesHingeAngleGetterFeedbackT.fillAmount = 0;
                YesHingeAngleGetterFeedbackB.fillAmount = 0;
                YesHingeAngleGetterFeedbackB.fillAmount = (mYesHinge.Angle / 60.00f) * 0.15f;
            }

            linearVelocity.text = linearVelocitySetter.value.ToString("0.00");
            distance.text = mDiagBehaviour.ExpScale(Math.Round(distanceSetter.value, 2) / 10D, 2D, 10D).ToString("0.00");
            AngularVelocityWheelsText.text = AngularVelocityWheelsSetter.value.ToString("0.00");
            angleBack.text = (mDiagBehaviour.ExpScale(anglePosSetter.value / 360D, 40D, 360D)).ToString("0");
            noAngleBack.text = noHingeAngleSetter.value.ToString();
            yesAngleBack.text = yesHingeAngleSetter.value.ToString();
            hingeSpeedBack.text = (mDiagBehaviour.ExpScale(hingeSpeedSetter.value / 100D, 10D, 100D)).ToString("0.0");
            NoAngle.text = Buddy.Actuators.Head.No.Angle.ToString("f3") + " °";
            YesAngle.text = Buddy.Actuators.Head.Yes.Angle.ToString("f3") + " °";
            XOdom.text = Buddy.Actuators.Wheels.Odometry.x.ToString("f3") + "m";
            YOdom.text = Buddy.Actuators.Wheels.Odometry.y.ToString("f3") + "m";
            Cap.text = Buddy.Actuators.Wheels.Odometry.z.ToString("f3") + " °";

        }

        public void MoveDistance()
        {
            Buddy.Navigation.Run<DisplacementStrategy>().Move(float.Parse(distance.text), float.Parse(linearVelocity.text), ObstacleAvoidanceType.NONE);
        }

        public void DelayedMoveDistance()
        {
            if (TimeMove.value != 0)
                StartCoroutine(DelayedNav());
        }

        private IEnumerator DelayedNav()
        {
            yield return new WaitForSeconds(1F);
            Buddy.Actuators.Wheels.SetVelocities(float.Parse(linearVelocity.text), float.Parse(AngularVelocityWheelsText.text));
            float lTime = float.Parse(TimeMove.captionText.text.Remove(1));
            yield return new WaitForSeconds(lTime);
            Buddy.Navigation.Stop();
        }

        public void TurnRelative()
        {
            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(float.Parse(angleBack.text), AngularVelocityWheelsSetter.value);

        }

        public void StopMotors()
        {
            Buddy.Actuators.Head.Stop();
            Buddy.Actuators.Wheels.Stop();
        }

        public void SetYesPos()
        {
            mYesHinge.SetPosition(yesHingeAngleSetter.value, float.Parse(hingeSpeedBack.text));
        }

        public void SetNoPos()
        {
            mNoHinge.SetPosition(noHingeAngleSetter.value, float.Parse(hingeSpeedBack.text));
        }

        private float DoubleToFloat(double iDouble)
        {
            return (float)iDouble;
        }
    }
}
