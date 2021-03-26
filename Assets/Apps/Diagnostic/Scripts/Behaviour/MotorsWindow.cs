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
        [SerializeField]
        private Text LeftCurrent;
        [SerializeField]
        private Text RightCurrent;
        [SerializeField]
        private Text LeftSpeed;
        [SerializeField]
        private Text RightSpeed;
        [SerializeField]
        private Text YesCurrent;
        [SerializeField]
        private Text YesSpeed;
        [SerializeField]
        private Text NoCurrent;
        [SerializeField]
        private Text NoSpeed;


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
        private Slider hingeSpeedYesSetter;
        [SerializeField]
        private Slider hingeSpeedNoSetter;

        [SerializeField]
        private Text yesAngleBack;
        [SerializeField]
        private Text noAngleBack;
        [SerializeField]
        private Text hingeSpeedYesBack;
        [SerializeField]
        private Text hingeSpeedNoBack;
        [SerializeField]
        private Text LastCommand;
        

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
            yesHingeAngleSetter.minValue = Buddy.Actuators.Head.Yes.AngleMin;
            yesHingeAngleSetter.maxValue = Buddy.Actuators.Head.Yes.AngleMax; ;

            noHingeAngleSetter.wholeNumbers = true;
            noHingeAngleSetter.value = 0F;
            noHingeAngleSetter.minValue = Buddy.Actuators.Head.No.AngleMin;
            noHingeAngleSetter.maxValue = Buddy.Actuators.Head.No.AngleMax;

            hingeSpeedYesSetter.wholeNumbers = true;
            hingeSpeedYesSetter.value = 15F;
            hingeSpeedYesSetter.minValue = 5F;
            hingeSpeedYesSetter.maxValue = 30F;


            hingeSpeedNoSetter.wholeNumbers = true;
            hingeSpeedNoSetter.value = 30F;
            hingeSpeedNoSetter.minValue = 20F;
            hingeSpeedNoSetter.maxValue = 80F;

            LastCommand.text = "";
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
            //hingeSpeedBack.text = (mDiagBehaviour.ExpScale(hingeSpeedSetter.value / 80D, 10D, 80D)).ToString("0");
            hingeSpeedYesBack.text = hingeSpeedYesSetter.value.ToString("0");
            hingeSpeedNoBack.text = hingeSpeedNoSetter.value.ToString("0");
            NoAngle.text = Buddy.Actuators.Head.No.Angle.ToString("f3") + " °";
            YesAngle.text = Buddy.Actuators.Head.Yes.Angle.ToString("f3") + " °";
            XOdom.text = Buddy.Actuators.Wheels.Odometry.x.ToString("f3") + "m";
            YOdom.text = Buddy.Actuators.Wheels.Odometry.y.ToString("f3") + "m";
            Cap.text = Buddy.Actuators.Wheels.Odometry.z.ToString("f3") + " °";


            LeftCurrent.text = Buddy.Actuators.Wheels.LeftCurrent.ToString() + "mA";
            RightCurrent.text = Buddy.Actuators.Wheels.RightCurrent.ToString() + "mA";
            LeftSpeed.text = Buddy.Actuators.Wheels.LeftRotationalSpeed.ToString("f1") + "°/s";
            RightSpeed.text = Buddy.Actuators.Wheels.RightRotationalSpeed.ToString("f1") + "°/s";
            YesCurrent.text = Buddy.Actuators.Head.Yes.Current.ToString() + "mA";
            YesSpeed.text = Buddy.Actuators.Head.Yes.Speed.ToString("f1") + "°/s";
            NoCurrent.text = Buddy.Actuators.Head.No.Current.ToString() + "mA";
            NoSpeed.text = Buddy.Actuators.Head.No.Speed.ToString("f1") + "°/s";


        }

        public void MoveDistance()
        {
            LastCommand.text = "MOVE for " + distance.text + "m at " + linearVelocity.text + "m/s";

            //Buddy.Navigation.Run<DisplacementStrategy>().Move(float.Parse(distance.text), float.Parse(linearVelocity.text), ObstacleAvoidanceType.NONE);
            Buddy.Actuators.Wheels.SetDistance(int.Parse(linearVelocity.text)*10, 0, float.Parse(distance.text)*100F, ()=> { LastCommand.text = "SetDistance command finished"; });
        }

        public void DelayedMoveDistance()
        {
            if (TimeMove.value != 0)
                StartCoroutine(DelayedNav());
        }

        private IEnumerator DelayedNav()
        {
            LastCommand.text = "MOVE for " + TimeMove.value + "s at " + linearVelocity.text + "m/s and " + AngularVelocityWheelsText + " °/s";
            yield return new WaitForSeconds(1F);
            Buddy.Actuators.Wheels.SetVelocities(float.Parse(linearVelocity.text), float.Parse(AngularVelocityWheelsText.text));
            float lTime = float.Parse(TimeMove.captionText.text.Remove(1));
            yield return new WaitForSeconds(lTime);
            Buddy.Navigation.Stop();
            // Reset dropdown
            TimeMove.value = 0;
        }

        public void TurnRelative()
        {
            LastCommand.text = "Rotate at " + angleBack.text + "° at " + AngularVelocityWheelsSetter.value + " °/s";
            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(float.Parse(angleBack.text), AngularVelocityWheelsSetter.value);

        }

        public void StopMotors()
        {
            LastCommand.text = "Stop head and wheels motors";
            Buddy.Actuators.Head.Stop();
            Buddy.Actuators.Wheels.Stop();
        }

        public void SetYesPos()
        {
            LastCommand.text = "Set head YES at " + yesHingeAngleSetter.value + "° at " + hingeSpeedYesBack.text + " °/s";
            //mYesHinge.SetPosition(yesHingeAngleSetter.value, float.Parse(hingeSpeedYesBack.text));
            //Buddy.Actuators.Head.MoveYes(yesHingeAngleSetter.value, 90F);
            Buddy.Actuators.Head.Yes.MoveYes(50F, yesHingeAngleSetter.value, () => { LastCommand.text = "MoveYes command finished"; });

        }

        public void SetNoPos()
        {
            LastCommand.text = "Set head NO at " + noHingeAngleSetter.value + "° at " + hingeSpeedNoBack.text + " °/s";
            //mNoHinge.SetPosition(noHingeAngleSetter.value, float.Parse(hingeSpeedNoBack.text));
            //Buddy.Actuators.Head.MoveNo(noHingeAngleSetter.value, 90F);
            Buddy.Actuators.Head.No.MoveNo(50F, noHingeAngleSetter.value, ()=> { LastCommand.text = "MoveNo command finished"; });
        }

        public void UnlockWheels()
        {
            LastCommand.text = "Unlock Wheels";
            Buddy.Actuators.Wheels.UnlockWheels();
        }

        private float DoubleToFloat(double iDouble)
        {
            return (float)iDouble;
        }
    }
}
