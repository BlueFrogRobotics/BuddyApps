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
        private Text noHingeAngleGetter;


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
        private Text distanceBack;
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
        private BlueQuark.Navigation mNavigation;

        void Start()
        {

            mWheels = Buddy.Actuators.Wheels;
            mYesHinge = Buddy.Actuators.Head.Yes;
            mNoHinge = Buddy.Actuators.Head.No;
            mNavigation = Buddy.Navigation;

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
            positionGetter.text = "Position : " + mWheels.Odometry;
            leftSpeedGetter.text = "Left speed : " + mWheels.LeftRotationalSpeed;
            rightSpeedGetter.text = "Right speed : " + mWheels.RightRotationalSpeed;
            noHingeAngleGetter.text = "No angle : " + mNoHinge.Angle;
            yesHingeAngleGetter.text = "Yes angle : " + mYesHinge.Angle;

            linearVelocity.text = "Linear Velocity: " + Math.Round(linearVelocitySetter.value, 2);
            angularVelocity.text = "Angular Velocity : " + angularVelocitySetter.value;

            distanceBack.text = "Dist : " + Math.Round(distanceSetter.value, 2);
            AngularVelocityWheelsText.text = "Spd : " + AngularVelocityWheelsSetter.value;

            angleBack.text = "Angle Th : " + anglePosSetter.value;
            //toleranceBack.text = "Tol : " + toleranceSetter.value;

            noAngleBack.text = "No ang : " + noHingeAngleSetter.value;
            yesAngleBack.text = "Yes ang : " + yesHingeAngleSetter.value;
            hingeSpeedBack.text = "Spd : " + hingeSpeedSetter.value;
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
            Buddy.Navigation.Run<DisplacementStrategy>().Move((float)mDistance, (float)mLinearVelocity);

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
            Buddy.Navigation.Run<DisplacementStrategy>().Move((float)mDistance, (float)mLinearVelocity);
        }

        public void TurnAbsolute()
        {
            //mWheels.TurnAbsoluteAngle(anglePosSetter.value,
            //                    (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
            //                    toleranceSetter.value);
            Debug.Log("NOT IMPLEMENTED YET");
            mNavigation.Run<DisplacementStrategy>().RotateTo(anglePosSetter.value, AngularVelocityWheelsSetter.value);
        }

        public void TurnRelative()
        {
            //mWheels.TurnAngle(anglePosSetter.value,
            //                (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
            //                toleranceSetter.value);
            Debug.Log("NOT IMPLEMENTED YET");
            mNavigation.Run<DisplacementStrategy>().Rotate(anglePosSetter.value, AngularVelocityWheelsSetter.value);
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
