using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

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
        //[SerializeField]
        //private Text angularVelocity;
        //[SerializeField]
        //private Slider angularVelocitySetter;

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
            NoAngle.text = Buddy.Actuators.Head.No.Angle.ToString();
            YesAngle.text = Buddy.Actuators.Head.Yes.Angle.ToString();

            XOdom.text = Buddy.Actuators.Wheels.Odometry.x.ToString();
            YOdom.text = Buddy.Actuators.Wheels.Odometry.y.ToString();
            Cap.text = Buddy.Actuators.Wheels.Odometry.z.ToString();

            mWheels = Buddy.Actuators.Wheels;
            mYesHinge = Buddy.Actuators.Head.Yes;
            mNoHinge = Buddy.Actuators.Head.No;

            linearVelocitySetter.wholeNumbers = false;
            linearVelocitySetter.minValue = -1F;
            linearVelocitySetter.maxValue = 1F;

            //angularVelocitySetter.wholeNumbers = true;
            //angularVelocitySetter.minValue = -3.14F;
            //angularVelocitySetter.maxValue = 3.14F;

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
            yesHingeAngleSetter.minValue = -50F;
            yesHingeAngleSetter.maxValue = 37F;

            noHingeAngleSetter.wholeNumbers = true;
            noHingeAngleSetter.minValue = -100F;
            noHingeAngleSetter.maxValue = 100F;

            hingeSpeedSetter.wholeNumbers = true;
            hingeSpeedSetter.minValue = 0F;
            hingeSpeedSetter.maxValue = 96F;
        }

        void Update()
        {
            //Debug.Log("Width slider : " + hingeSpeedSetter.gameObject.transform.);
            leftSpeedGetter.text = "" + mWheels.LeftRotationalSpeed;
            rightSpeedGetter.text = "" + mWheels.RightRotationalSpeed;
            // Draw Head No Angle Feedback
            noHingeAngleGetter.text = mNoHinge.Angle.ToString();
            if(mNoHinge.Angle >= 0) {
                noHingeAngleGetterFeedbackL.fillAmount = 0;
                noHingeAngleGetterFeedbackL.fillAmount = (mNoHinge.Angle / 90.00f) * 0.25f;
                noHingeAngleGetterFeedbackR.fillAmount = 0;
            }
            if (mNoHinge.Angle <= 0)
            {
                noHingeAngleGetterFeedbackL.fillAmount = 0;
                noHingeAngleGetterFeedbackR.fillAmount = 0;
                noHingeAngleGetterFeedbackR.fillAmount = (mNoHinge.Angle / 90.00f) * 0.25f;
            }
            // Draw Head Yes Angle Feedback
            yesHingeAngleGetter.text = mYesHinge.Angle.ToString();
            if (mYesHinge.Angle >= 0)
            {
                yesHingeAngleGetterFeedbackT.fillAmount = 0;
                yesHingeAngleGetterFeedbackT.fillAmount = (mYesHinge.Angle / 80.00f) * 0.25f;
                yesHingeAngleGetterFeedbackB.fillAmount = 0;
            }
            if (mNoHinge.Angle <= 0)
            {
                yesHingeAngleGetterFeedbackT.fillAmount = 0;
                yesHingeAngleGetterFeedbackB.fillAmount = 0;
                yesHingeAngleGetterFeedbackB.fillAmount = (mYesHinge.Angle / 60.00f) * 0.15f;
            }

            linearVelocity.text = (mDiagBehaviour.ExpScale(Math.Round(linearVelocitySetter.value + 1d, 2)/2d, 0.5d, 2d)-1d).ToString("0.00") /*+ " m/s"*/;

            //angularVelocity.text =  (angularVelocitySetter.value ).ToString();

            distance.text = mDiagBehaviour.ExpScale( Math.Round(distanceSetter.value, 2)/10D, 2D, 10D).ToString("0.00")/* + " m"*/;

            AngularVelocityWheelsText.text = (mDiagBehaviour.ExpScale((AngularVelocityWheelsSetter.value) / 250D, 40D, 250D)).ToString("0.00")/* + " °/s"*/;

            angleBack.text = anglePosSetter.value.ToString() /*+ " °"*/;
            //toleranceBack.text = "Tol : " + toleranceSetter.value;

            noAngleBack.text = noHingeAngleSetter.value.ToString() /*+ " °"*/;
            yesAngleBack.text = yesHingeAngleSetter.value.ToString() /*+ " °"*/;
            hingeSpeedBack.text =  (mDiagBehaviour.ExpScale( hingeSpeedSetter.value / 96D, 10D, 96D)).ToString("0.0F");
            NoAngle.text = Buddy.Actuators.Head.No.Angle.ToString("f3")/* + " °"*/;
            YesAngle.text = Buddy.Actuators.Head.Yes.Angle.ToString("f3") /*+" °"*/;
            Cap.text = Buddy.Actuators.Wheels.Odometry.z.ToString("f3")/* + " °"*/;

            //Debug.Log("angular velocity : " + );
        }

        public void SetWheelsSpeed()
        {
            //mWheels.SetWheelsSpeed(leftSpeedSetter.value,
            //                        rightSpeedSetter.value,
            //                        (int)timerSetter.value);

            //mWheels.SetVelocities(mWheels.LinearVelocity, mWheels.AngularVelocity);
            //mWheels.SetVelocities(linearVelocitySetter.value, angularVelocitySetter.value);
            double mDistance = mDiagBehaviour.ExpScale(Math.Round(distanceSetter.value, 4) / 10D, 2D, 10D);
            double mLinearVelocity = (mDiagBehaviour.ExpScale(Math.Round(linearVelocitySetter.value + 1d, 4) / 2d, 0.5d, 2d) - 1d);
            Buddy.Navigation.Run<DisplacementStrategy>().Move((float)mDistance, (float)mLinearVelocity, ObstacleAvoidanceType.NONE);

        }

        public void MoveDistance()
        {
            //mWheels.MoveDistance(leftSpeedSetter.value,
            //                    rightSpeedSetter.value,
            //                    distanceSetter.value,
            //                    toleranceSetter.value);
            //Debug.Log("NOT IMPLEMENTED YET");
            //mNavigation.Run<DisplacementStrategy>().Move(distanceSetter.value, speedDisplacementStrategySetter.value);
            double mDistance = mDiagBehaviour.ExpScale(Math.Round(distanceSetter.value, 4) / 10D, 2D, 10D);
            double mLinearVelocity = (mDiagBehaviour.ExpScale(Math.Round(linearVelocitySetter.value + 1d, 4) / 2d, 0.5d, 2d) - 1d);
            Buddy.Navigation.Run<DisplacementStrategy>().Move((float)mDistance, (float)mLinearVelocity, ObstacleAvoidanceType.NONE);
        }

        public void TurnAbsolute()
        {
            //mWheels.TurnAbsoluteAngle(anglePosSetter.value,
            //                    (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
            //                    toleranceSetter.value);
            Buddy.Navigation.Run<DisplacementStrategy>().RotateTo(anglePosSetter.value, DoubleToFloat(mDiagBehaviour.ExpScale((AngularVelocityWheelsSetter.value) / 250D, 40D, 250D)));
            Debug.Log("angular  velocity rotate to : " + AngularVelocityWheelsSetter.value);
        }

        public void TurnRelative()
        {
            //mWheels.TurnAngle(anglePosSetter.value,
            //                (leftSpeedSetter.value + rightSpeedSetter.value) / 2,
            //                toleranceSetter.value);
            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(anglePosSetter.value, DoubleToFloat(mDiagBehaviour.ExpScale((AngularVelocityWheelsSetter.value) / 250D, 40D, 250D)));
            Debug.Log("angular  velocity rotate : " + AngularVelocityWheelsSetter.value);

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

        public void StopMotors()
        {
            Buddy.Actuators.Head.Stop();
            Buddy.Actuators.Wheels.Stop();
        }

        public void SetYesPos()
        {
            mYesHinge.SetPosition(yesHingeAngleSetter.value, DoubleToFloat(mDiagBehaviour.ExpScale(hingeSpeedSetter.value / 96D, 10D, 96D)));
            //mYesHinge.SetPosition(yesHingeAngleSetter.value);
        }

        public void SetNoPos()
        {
            mNoHinge.SetPosition(noHingeAngleSetter.value, DoubleToFloat(mDiagBehaviour.ExpScale(hingeSpeedSetter.value / 96D, 10D, 96D)));
        }

        private float DoubleToFloat(double iDouble)
        {
            return (float)iDouble;
        }
    }
}
