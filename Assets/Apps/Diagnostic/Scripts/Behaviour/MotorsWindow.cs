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
        private Text speedDisplacementStrategy;
        [SerializeField]
        private Slider speedDisplacementStrategySetter;




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

            linearVelocitySetter.wholeNumbers = true;
            linearVelocitySetter.minValue = -1F;
            linearVelocitySetter.maxValue = 1F;

            angularVelocitySetter.wholeNumbers = true;
            angularVelocitySetter.minValue = -3.14F;
            angularVelocitySetter.maxValue = 3.14F;

            //timerSetter.wholeNumbers = true;
            //timerSetter.minValue = 500F;
            //timerSetter.maxValue = 10000F;

            speedDisplacementStrategySetter.wholeNumbers = true;
            speedDisplacementStrategySetter.minValue = 0F;
            speedDisplacementStrategySetter.maxValue = 180F;

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
            yesHingeAngleSetter.minValue = -30F;
            yesHingeAngleSetter.maxValue = 70F;

            noHingeAngleSetter.wholeNumbers = true;
            noHingeAngleSetter.minValue = -74F;
            noHingeAngleSetter.maxValue = 74F;

            hingeSpeedSetter.wholeNumbers = true;
            hingeSpeedSetter.minValue = 0F;
            hingeSpeedSetter.maxValue = 400F;
        }

        void Update()
        {
            positionGetter.text = "Position : " + mWheels.Odometry;
            leftSpeedGetter.text = "Left speed : " + mWheels.LeftRotationalSpeed;
            rightSpeedGetter.text = "Right speed : " + mWheels.RightRotationalSpeed;
            noHingeAngleGetter.text = "No angle : " + mNoHinge.Angle;
            yesHingeAngleGetter.text = "Yes angle : " + mYesHinge.Angle;

            linearVelocity.text = "Linear Velocity: " + linearVelocitySetter.value;
            angularVelocity.text = "Angular Velocity : " + angularVelocitySetter.value;

            distanceBack.text = "Dist : " + distanceSetter.value;
            speedDisplacementStrategy.text = "Timer : " + speedDisplacementStrategySetter.value;

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
            
            mWheels.SetVelocities(mWheels.LinearVelocity, mWheels.AngularVelocity);
        }

        public void MoveDistance()
        {
            //mWheels.MoveDistance(leftSpeedSetter.value,
            //                    rightSpeedSetter.value,
            //                    distanceSetter.value,
            //                    toleranceSetter.value);
            Debug.Log("NOT IMPLEMENTED YET");
            mNavigation.Run<DisplacementStrategy>().Move(distanceSetter.value, speedDisplacementStrategySetter.value);
            
        }

        public void TurnAbsolute()
        {
            //mWheels.TurnAbsoluteAngle(anglePosSetter.value,
            //                    (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
            //                    toleranceSetter.value);
            Debug.Log("NOT IMPLEMENTED YET");
            mNavigation.Run<DisplacementStrategy>().RotateTo(anglePosSetter.value, speedDisplacementStrategySetter.value);
        }

        public void TurnRelative()
        {
            //mWheels.TurnAngle(anglePosSetter.value,
            //                (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
            //                toleranceSetter.value);
            Debug.Log("NOT IMPLEMENTED YET");
            mNavigation.Run<DisplacementStrategy>().Rotate(anglePosSetter.value, speedDisplacementStrategySetter.value);
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
        }

        public void SetNoPos()
        {
            mNoHinge.SetPosition(noHingeAngleSetter.value, hingeSpeedSetter.value);
        }
    }
}
