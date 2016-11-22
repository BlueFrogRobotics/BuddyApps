using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class MotorsWindow : MonoBehaviour
    {
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

        [SerializeField]
        private InputField xPosSetter;

        [SerializeField]
        private InputField yPosSetter;

        [SerializeField]
        private Slider anglePosSetter;

        [SerializeField]
        private Slider distanceSetter;

        [SerializeField]
        private Slider leftSpeedSetter;

        [SerializeField]
        private Slider rightSpeedSetter;

        [SerializeField]
        private Slider timerSetter;

        [SerializeField]
        private Slider toleranceSetter;

        [SerializeField]
        private Slider yesHingeAngleSetter;

        [SerializeField]
        private Slider noHingeAngleSetter;

        [SerializeField]
        private Slider hingeSpeedSetter;

        [SerializeField]
        private Text leftSpeedBack;

        [SerializeField]
        private Text rightSpeedBack;

        [SerializeField]
        private Text timerBack;

        [SerializeField]
        private Text angleBack;

        [SerializeField]
        private Text distanceBack;

        [SerializeField]
        private Text yesAngleBack;

        [SerializeField]
        private Text noAngleBack;

        [SerializeField]
        private Text hingeSpeedBack;

        [SerializeField]
        private Text toleranceBack;

        private Wheels mWheels;
        private Hinge mYesHinge;
        private Hinge mNoHinge;

        void Start()
        {
            mWheels = BYOS.Instance.Motors.Wheels;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
            mNoHinge = BYOS.Instance.Motors.NoHinge;

            leftSpeedSetter.wholeNumbers = true;
            leftSpeedSetter.minValue = 0F;
            leftSpeedSetter.maxValue = 720F;

            rightSpeedSetter.wholeNumbers = true;
            rightSpeedSetter.minValue = 0F;
            rightSpeedSetter.maxValue = 720F;

            timerSetter.wholeNumbers = true;
            timerSetter.minValue = 500F;
            timerSetter.maxValue = 10000F;

            anglePosSetter.wholeNumbers = true;
            anglePosSetter.minValue = -180F;
            anglePosSetter.maxValue = 180F;

            toleranceSetter.wholeNumbers = false;
            toleranceSetter.minValue = 0F;
            toleranceSetter.maxValue = 5F;

            distanceSetter.wholeNumbers = false;
            distanceSetter.minValue = 0F;
            distanceSetter.maxValue = 10F;

            yesHingeAngleSetter.wholeNumbers = true;
            yesHingeAngleSetter.minValue = -30F;
            yesHingeAngleSetter.maxValue = 70F;

            noHingeAngleSetter.wholeNumbers = true;
            noHingeAngleSetter.minValue = -74F;
            noHingeAngleSetter.maxValue = 74F;
        }

        void Update()
        {
            positionGetter.text = "Position : " + mWheels.GetRobotPoseBelieves();
            leftSpeedGetter.text = "Left speed : " + mWheels.LeftWheelAngularVelocity;
            rightSpeedGetter.text = "Right speed : " + mWheels.RightWheelAngularVelocity;
            noHingeAngleGetter.text = "No angle : " + mNoHinge.CurrentAnglePosition;
            yesHingeAngleGetter.text = "Yes angle : " + mYesHinge.CurrentAnglePosition;

            leftSpeedBack.text = "Left spd : " + leftSpeedSetter.value;
            rightSpeedBack.text = "Right spd : " + rightSpeedSetter.value;
            distanceBack.text = "Dist : " + distanceSetter.value;
            timerBack.text = "Timer : " + timerSetter.value;
            angleBack.text = "Th : " + anglePosSetter.value;
            noAngleBack.text = "No ang : " + noHingeAngleSetter.value;
            yesAngleBack.text = "Yes ang : " + yesHingeAngleSetter.value;
            hingeSpeedBack.text = "Spd : " + hingeSpeedSetter.value;
            toleranceBack.text = "Tol : " + toleranceSetter.value;
        }

        public void SetWheelsSpeed()
        {
            mWheels.SetWheelsSpeed(leftSpeedSetter.value,
                                    rightSpeedSetter.value,
                                    (int)timerSetter.value);
        }

        public void MoveDistance()
        {
            mWheels.MoveDistance(leftSpeedSetter.value,
                                rightSpeedSetter.value,
                                distanceSetter.value,
                                toleranceSetter.value);
        }

        public void TurnAbsolute()
        {
            mWheels.TurnAbsoluteAngle(anglePosSetter.value,
                                (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
                                toleranceSetter.value);
        }

        public void TurnRelative()
        {
            mWheels.TurnAngle(anglePosSetter.value,
                            (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
                            toleranceSetter.value);
        }

        public void GoTo()
        {
            mWheels.MoveToAbsolutePosition(new Vector2(float.Parse(xPosSetter.text),
                                        float.Parse(yPosSetter.text)),
                                        (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
                                        toleranceSetter.value);
        }

        public void SetYesPos()
        {
            mYesHinge.SetPosition(yesHingeAngleSetter.value, hingeSpeedSetter.value);
        }

        public void SetNoPos()
        {
            mYesHinge.SetPosition(noHingeAngleSetter.value, hingeSpeedSetter.value);
        }
    }
}
