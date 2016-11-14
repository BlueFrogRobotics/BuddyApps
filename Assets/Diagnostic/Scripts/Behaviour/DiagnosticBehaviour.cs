using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class DiagnosticBehaviour : MonoBehaviour
    {
        private RGBCam mRGBCam;
        private DepthCam mDepthCam;
        private TextToSpeech mTTS;
        private SpeechToText mSTT;
        private SphinxTrigger mSphinx;
        private USSensor mLeftUSSensor;
        private USSensor mRightUSSensor;
        private USSensor mBackUSSensor;
        private IRSensor mLeftIRSensor;
        private IRSensor mMiddleIRSensor;
        private IRSensor mRightIRSensor;
        private ThermalSensor mThermalSensor;
        private LED mLED;
        private Face mFace;
        private Wheels mWheels;
        private Hinge mYesHinge;
        private Hinge mNoHinge;

        // Use this for initialization
        void Start()
        {
            mRGBCam = BYOS.Instance.RGBCam;
            mDepthCam = BYOS.Instance.DepthCam;
            mTTS = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;
            mSphinx = BYOS.Instance.SphinxTrigger;
            mLeftUSSensor = BYOS.Instance.USSensors.Left;
            mRightUSSensor = BYOS.Instance.USSensors.Right;
            mBackUSSensor = BYOS.Instance.USSensors.Back;
            mLeftIRSensor = BYOS.Instance.IRSensors.Left;
            mMiddleIRSensor = BYOS.Instance.IRSensors.Middle;
            mRightIRSensor = BYOS.Instance.IRSensors.Right;
            mThermalSensor = BYOS.Instance.ThermalSensor;
            mLED = BYOS.Instance.LED;
            mFace = BYOS.Instance.Face;
            mWheels = BYOS.Instance.Motors.Wheels;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}