using UnityEngine;
using BuddyOS;

namespace BuddyApp.Remote
{
    public class SensorOTOSender : OTONetSender
    {
        [SerializeField]
        private OTONetwork OTO;

        private float mTime;
        private USSensors mUSSensors;
        private IRSensors mIRSensors;

        void Start()
        {
            mUSSensors = BYOS.Instance.USSensors;
            mIRSensors = BYOS.Instance.IRSensors;

            mTime = Time.time;
        }

        void Update()
        {
            if (!OTO.HasAPeer || Time.time - mTime < 0.1f)
                return;

            //Debug.Log("Sensor OTO has peer " + OTO.HasAPeer + " and sending");
            byte[] lSensorsValue = GetSensorsValue();
            SendData(lSensorsValue, lSensorsValue.Length);

            mTime = Time.time;
        }

        private byte[] GetSensorsValue()
        {
            float[] lSensorsDistance = new float[4];

            lSensorsDistance[0] = mUSSensors.Left.Distance;
            lSensorsDistance[1] = mIRSensors.Middle.Distance;
            lSensorsDistance[2] = mUSSensors.Right.Distance;
            lSensorsDistance[3] = mUSSensors.Back.Distance;

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
    }
}