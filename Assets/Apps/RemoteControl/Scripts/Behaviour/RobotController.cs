using BlueQuark;

using UnityEngine;

using UnityEngine.UI;

using System.Collections.Generic;

namespace BuddyApp.RemoteControl
{
    public class RobotController : MonoBehaviour
    {
        // Unused Code - Delete it after the test phase if no regression has occured

        //public enum KindOfControl { None = 0, Remote = 1 };
        //public enum ControllerMode { Simplify = 0, Medium = 1, Expert = 2 };

        //public KindOfControl mActualKindOfControl = KindOfControl.Remote;
        //public ControllerMode mControllerMode = ControllerMode.Simplify;

        private const int REFRESH_TIMER = 1;

        [SerializeField]
        private Webrtc mWebRtc;

        private float mTime;

        private UltrasonicSensors mUSSensors;
        private TimeOfFlightSensors mTOFSensors;

        private bool mBuddyMovement = true;

        // Use this for initialization
        void Start()
        {
            mTime = Time.time;
            mUSSensors = Buddy.Sensors.UltrasonicSensors;
            mTOFSensors = Buddy.Sensors.TimeOfFlightSensors;
        }

        // Update is called once per frame
        void Update()
        {
            if (mWebRtc.ConnectionState == Webrtc.CONNECTION.CONNECTING && Time.time - mTime > REFRESH_TIMER) {
                byte[] lData = GetSensorsValue();
                string lStringData = System.Convert.ToBase64String(lData);
                mWebRtc.SendWithDataChannel(lStringData);
                mTime = Time.time;
            }
        }

        public void DisableMovement()
        {
            mBuddyMovement = !mBuddyMovement;
            Debug.Log("Robot movement is set to:" + mBuddyMovement);
        }

        // This function is called when a webrtc data is received
        public void onMessage(string iMessage)
        {
            Debug.Log("new data : " + iMessage);
            //ARemoteCommand lCmd = ARemoteCommand.Deserialize(GetBytes(iMessage));
            //if (mBuddyMovement == true)
                //lCmd.Execute();
        }

        private byte[] GetSensorsValue()
        {
            float[] lSensorsDistance = new float[4];

            lSensorsDistance[0] = mTOFSensors.Left.Value;
            lSensorsDistance[1] = mTOFSensors.Front.Value;
            lSensorsDistance[2] = mTOFSensors.Right.Value;
            lSensorsDistance[3] = mTOFSensors.Back.Value;

            return SensorLvl(lSensorsDistance);
        }

        private byte[] SensorLvl(float[] iSensors)
        {
            byte[] lSensorsLevel = new byte[4];

            for (int i = 0; i < iSensors.Length; i++) {
                float lSensorValue = iSensors[i];
                if (lSensorValue >= .70f)
                    lSensorsLevel[i] = 1;
                else if (lSensorValue < .70f && lSensorValue >= .50f)
                    lSensorsLevel[i] = 2;
                else if (lSensorValue < .50f && lSensorValue >= .30f)
                    lSensorsLevel[i] = 3;
                else if (lSensorValue < .30f && lSensorValue > 0)
                    lSensorsLevel[i] = 4;
                else
                    lSensorsLevel[i] = 0;
            }
            return lSensorsLevel;
        }

        private byte[] GetBytes(string iStr)
        {
            return System.Convert.FromBase64String(iStr);
        }
    }
}