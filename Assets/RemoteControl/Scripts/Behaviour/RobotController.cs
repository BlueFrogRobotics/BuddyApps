using UnityEngine;
using System.Collections.Generic;
using Buddy;
using Buddy.Command;

namespace BuddyApp.RemoteControl
{
	public class RobotController : MonoBehaviour
	{

	    public enum KindOfControl { None = 0, Remote = 1 };
	    public enum ControllerMode { Simplify = 0, Medium = 1, Expert = 2 };

	    public KindOfControl mActualKindOfControl = KindOfControl.Remote;
	    public ControllerMode mControllerMode = ControllerMode.Simplify;

	    [SerializeField]
	    private Webrtc webrtc;

	    private float mTime;

	    private USSensors mUSSensors;
	    private IRSensors mIRSensors;

	    // Use this for initialization
	    void Start()
	    {
	        mTime = Time.time;
	        mUSSensors = BYOS.Instance.Primitive.USSensors;
			mIRSensors = BYOS.Instance.Primitive.IRSensors;
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        if (webrtc.ConnectionState == Webrtc.CONNECTION.CONNECTING && Time.time - mTime > 1F)
	        {
	            byte[] lData = GetSensorsValue();
	            string lStringData = System.Convert.ToBase64String(lData);
	            webrtc.SendWithDataChannel(lStringData);
	            mTime = Time.time;
	        }
	    }

	    // This function is called when a webrtc data is received
	    public void onMessage(string iMessage)
	    {
	        ACommand lCmd = ACommand.Deserialize(GetBytes(iMessage));
	        lCmd.Execute();
	    }

	    private byte[] GetSensorsValue()
	    {
	        float[] lSensorsDistance = new float[4];

	        lSensorsDistance[0] = mIRSensors.Left.Distance;
	        lSensorsDistance[1] = mIRSensors.Middle.Distance;
	        lSensorsDistance[2] = mIRSensors.Right.Distance;
	        lSensorsDistance[3] = mUSSensors.Back.Distance;

	        return SensorLvl(lSensorsDistance);
	    }

	    private byte[] SensorLvl(float[] iSensors)
	    {
	        byte[] lSensorsLevel = new byte[4];

	        for (int i = 0; i < iSensors.Length; i++)
	        {
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