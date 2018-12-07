using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public sealed class MotorsWindow : MonoBehaviour
    {
        /// <summary>
        /// Variables from the hardware
        /// </summary>
        [SerializeField]
        private Text positionGetter;
        [SerializeField]
        private Text leftSpeedGetter;
        [SerializeField]
        private Text rightSpeedGetter;
        [SerializeField]
        private Text yesHingeAngleGetter;
        [SerializeField]
        private Image yesHingeAngleGetterFeedbackT;
        [SerializeField]
        private Image yesHingeAngleGetterFeedbackB;
        [SerializeField]
        private Text noHingeAngleGetter;
        [SerializeField]
        private Image noHingeAngleGetterFeedbackL;
        [SerializeField]
        private Image noHingeAngleGetterFeedbackR;

        /// <summary>
        /// Parameters for setwheelspeed
        /// </summary>
        [SerializeField]
        private Text linearVelocity;
        [SerializeField]
        private Slider linearVelocitySetter;
        [SerializeField]
        private Text angularVelocity;
        [SerializeField]
        private Slider angularVelocitySetter;

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
        private Text angleBack;
        [SerializeField]
        private Slider anglePosSetter;

        [SerializeField]
        private InputField xPosSetter;

        [SerializeField]
        private InputField yPosSetter;

        //[SerializeField]
        //private Slider toleranceSetter;

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

        [SerializeField]
        private Text toleranceBack;

        private Wheels mWheels;
        private YesHeadHinge mYesHinge;
        private NoHeadHinge mNoHinge;
        //private BlueQuark.Navigation mNavigation;

        void Start()
        {

            mWheels = Buddy.Actuators.Wheels;
            mYesHinge = Buddy.Actuators.Head.Yes;
            mNoHinge = Buddy.Actuators.Head.No;

            linearVelocitySetter.wholeNumbers = false;
            linearVelocitySetter.minValue = -1F;
            linearVelocitySetter.maxValue = 1F;

            angularVelocitySetter.wholeNumbers = true;
            angularVelocitySetter.minValue = -3.14F;
            angularVelocitySetter.maxValue = 3.14F;

            //timerSetter.wholeNumbers = true;
            //timerSetter.minValue = 500F;
            //timerSetter.maxValue = 10000F;

            AngularVelocityWheelsSetter.wholeNumbers = true;
            AngularVelocityWheelsSetter.minValue = 0F;
            AngularVelocityWheelsSetter.maxValue = 250F;

            anglePosSetter.wholeNumbers = true;
            anglePosSetter.minValue = -180F;
            anglePosSetter.maxValue = 180F;

            //toleranceSetter.wholeNumbers = false;
            //toleranceSetter.minValue = 0F;
            //toleranceSetter.maxValue = 5F;

            distanceSetter.wholeNumbers = false;
            distanceSetter.minValue = 0F;
            distanceSetter.maxValue = 10F;

            yesHingeAngleSetter.wholeNumbers = true;
            yesHingeAngleSetter.minValue = -60F;
            yesHingeAngleSetter.maxValue = 30F;

            noHingeAngleSetter.wholeNumbers = true;
            noHingeAngleSetter.minValue = -90F;
            noHingeAngleSetter.maxValue = 90F;

            hingeSpeedSetter.wholeNumbers = true;
            hingeSpeedSetter.minValue = 0F;
            hingeSpeedSetter.maxValue = 96F;
        }

        void Update()
        {
            positionGetter.text = "" + mWheels.Odometry;
            leftSpeedGetter.text = "" + mWheels.LeftRotationalSpeed;
            rightSpeedGetter.text = "" + mWheels.RightRotationalSpeed;
            // Draw Head No Angle Feedback
            noHingeAngleGetter.text = mNoHinge.Angle + " °";
            if(mNoHinge.Angle >= 0) {
                noHingeAngleGetterFeedbackL.fillAmount =  mNoHinge.Angle;
                noHingeAngleGetterFeedbackR.fillAmount = 0;
            }
            if (mNoHinge.Angle <= 0)
            {
                noHingeAngleGetterFeedbackL.fillAmount = 0;
                noHingeAngleGetterFeedbackR.fillAmount = mNoHinge.Angle;
            }
            // Draw Head Yes Angle Feedback
            yesHingeAngleGetter.text = mYesHinge.Angle + " °";
            if (mYesHinge.Angle >= 0)
            {
                yesHingeAngleGetterFeedbackT.fillAmount = mYesHinge.Angle;
                yesHingeAngleGetterFeedbackB.fillAmount = 0;
            }
            if (mNoHinge.Angle <= 0)
            {
                yesHingeAngleGetterFeedbackT.fillAmount = 0;
                yesHingeAngleGetterFeedbackB.fillAmount = mYesHinge.Angle;
            }

            linearVelocity.text = Math.Round(linearVelocitySetter.value, 2) + " M/s";

            angularVelocity.text = "" + angularVelocitySetter.value;

            distance.text = Math.Round(distanceSetter.value, 2) + " M";

            AngularVelocityWheelsText.text = AngularVelocityWheelsSetter.value + " °/s";

            angleBack.text = anglePosSetter.value + " °";
            //toleranceBack.text = "Tol : " + toleranceSetter.value;

            noAngleBack.text = noHingeAngleSetter.value + " °";
            yesAngleBack.text = yesHingeAngleSetter.value + " °";
            hingeSpeedBack.text = "" + hingeSpeedSetter.value;
        }

        public void SetWheelsSpeed()
        {
            //mWheels.SetWheelsSpeed(leftSpeedSetter.value,
            //                        rightSpeedSetter.value,
            //                        (int)timerSetter.value);

            //mWheels.SetVelocities(mWheels.LinearVelocity, mWheels.AngularVelocity);
            //mWheels.SetVelocities(linearVelocitySetter.value, angularVelocitySetter.value);
            double mDistance = Math.Round(distanceSetter.value, 4);
            double mLinearVelocity = Math.Round(linearVelocitySetter.value, 4);
            Buddy.Navigation.Run<DisplacementStrategy>().Move((float)mDistance, (float)mLinearVelocity, false);

        }

        public void MoveDistance()
        {
            //mWheels.MoveDistance(leftSpeedSetter.value,
            //                    rightSpeedSetter.value,
            //                    distanceSetter.value,
            //                    toleranceSetter.value);
            //Debug.Log("NOT IMPLEMENTED YET");
            //mNavigation.Run<DisplacementStrategy>().Move(distanceSetter.value, speedDisplacementStrategySetter.value);
            double mDistance = Math.Round(distanceSetter.value, 4);
            double mLinearVelocity = Math.Round(linearVelocitySetter.value, 4);
            Buddy.Navigation.Run<DisplacementStrategy>().Move((float)mDistance, (float)mLinearVelocity, false);
        }

        public void TurnAbsolute()
        {
            //mWheels.TurnAbsoluteAngle(anglePosSetter.value,
            //                    (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
            //                    toleranceSetter.value);
            Debug.Log("NOT IMPLEMENTED YET");
            Buddy.Navigation.Run<DisplacementStrategy>().RotateTo(anglePosSetter.value, AngularVelocityWheelsSetter.value);
        }

        public void TurnRelative()
        {
            //mWheels.TurnAngle(anglePosSetter.value,
            //                (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
            //                toleranceSetter.value);
            Debug.Log("NOT IMPLEMENTED YET");
            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(anglePosSetter.value, AngularVelocityWheelsSetter.value);
        }

        public void GoTo()
        {
            //mWheels.MoveToAbsolutePosition(new Vector2(float.Parse(xPosSetter.text),
            //                            float.Parse(yPosSetter.text)),
            //                            (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
            //                            toleranceSetter.value);
            Debug.Log("NOT IMPLEMENTED YET");
            //Buddy.Navigation.Run<LocationStrategy>().To(new Vector2(float.Parse(xPosSetter.text),
            //                            float.Parse(yPosSetter.text)));
        }

        public void SetYesPos()
        {
            mYesHinge.SetPosition(yesHingeAngleSetter.value, hingeSpeedSetter.value);
            //mYesHinge.SetPosition(yesHingeAngleSetter.value);
        }

        public void SetNoPos()
        {
            mNoHinge.SetPosition(noHingeAngleSetter.value, hingeSpeedSetter.value);
        }
    }
}
