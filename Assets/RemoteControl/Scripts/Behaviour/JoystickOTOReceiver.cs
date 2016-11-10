using UnityEngine;
using BuddyOS;


namespace BuddyApp.Remote
{

    public class JoystickOTOReceiver : OTONetReceiver
    {
        [SerializeField]
        private Motors motionMotors;

        //Motion's var
        private float mNoSpeed = 0f;
        private float mYesSpeed = 0f;
        private float mAngleYes = 0f;
        private float mAngleNo = 0f;
        private float mSpeedHead = 0f;
        private float mSpeedBody = 0f;

        void Start()
        {
            mSpeedHead = 200f;
            mSpeedBody = 200f;
        }

        public override void ReceiveData(byte[] data, int ndata)
        {

            ACommand cmd = ACommand.Deserialize(data);
            cmd.Execute();
        }

        private void ControlNoAxis(byte iXHead)
        {
            mAngleNo -= iXHead * 10f;
            if (Mathf.Abs(mAngleNo) > 76)
                mAngleNo = Mathf.Sign(mAngleNo) * 76;
            mNoSpeed = (iXHead * iXHead) * mSpeedHead * 3f;
            motionMotors.NoHinge.SetPosition(mAngleNo, mNoSpeed);
        }

        private void ControlYesAxis(byte iYHead)
        {
            mAngleYes += iYHead * 10f;
            if (mAngleNo < -30)
                mAngleNo = -30;
            if (mAngleNo > 60)
                mAngleNo = 60;
            mYesSpeed = (iYHead * iYHead) * mSpeedHead * 3f;
            motionMotors.YesHinge.SetPosition(mAngleYes, mYesSpeed);
        }

        private void ControlMobileBase(byte iXBody, byte iYBody)
        {
            float lRadius = Mathf.Sqrt(iXBody * iXBody + iYBody * iYBody);
            float lAngle = -(Mathf.Atan2(iYBody, iXBody) + Mathf.PI);
            float lSpeedLeft = 400 * (Mathf.Cos(lAngle) - Mathf.Sin(lAngle)) * lRadius;
            float lSpeedRight = 400 * (Mathf.Cos(lAngle) + Mathf.Sin(lAngle)) * lRadius;
            motionMotors.Wheels.SetWheelsSpeed(lSpeedLeft, lSpeedRight, 100);
        }
    }
}
