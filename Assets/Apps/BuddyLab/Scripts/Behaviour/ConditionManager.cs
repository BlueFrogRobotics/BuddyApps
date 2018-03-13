using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using UnityEngine.UI;
using OpenCVUnity;

namespace BuddyApp.BuddyLab
{
    public class ConditionManager : MonoBehaviour
    {
        /// <summary>
        /// Variables linked to the tactile
        /// </summary>
        private enum TactileEvent : int
        {
            LEFT_EYE,
            RIGHT_EYE,
            ALL_TACTILE,
            MOUTH,
            BODY_MOVING,
            HEAD_MOVING,
            NONE
        }
        private bool mIsTactileDetect;
        private TactileEvent mTactile;
        private Face mFace;
        private bool mTactileSubscribed;
        private Motors mMotor;
        public const float ANGLE_THRESH = 10.0F;
        private float mOriginRobotAngle;

        /// <summary>
        /// Variables linked to obstacle detection
        /// </summary>
        private const float OBSTACLE_DISTANCE = 1.1f;
        private enum IRSensor : int
        {
            FRONT,
            RIGHT,
            LEFT,
            NONE
        }
        private IRSensor mIRSensor;
        private bool mIRSensorDetect;
        private IRSensors mIRSensors;

        private bool mHeadMoving;
        private bool mBodyMoving;

        /// <summary>
        /// Variables for movement detection
        /// </summary>
        private MotionDetection mMotion;
        private RGBCam mCam;

        /// <summary>
        /// Variables for fire detection
        /// </summary>
        private ThermalDetection mFireDetection;
        private bool mIsFireDetect;

        private QRCodeDetection mQRCodeDetect;

        /// <summary>
        /// Boolean which says if the Event is done.
        /// </summary>
        private bool mIsEventDone;
        public bool IsEventDone { get { return mIsEventDone; } set { mIsEventDone = value; } }

        /// <summary>
        /// Variables for sound detection
        /// </summary>
        public const float MAX_SOUND_THRESHOLD = 0.3F;
        private NoiseDetection mNoiseDetection;

        /// <summary>
        /// Variables for specific string to detect
        /// </summary>
        private bool mIsStringSaid;
        private bool mIsListening;
        private string mSpeechReco;
        private SpeechToText mSTT;

        /// <summary>
        /// String that describes the type of condition. Conditions : Fire, movement, obstacles in front of Buddy,
        /// obstacles at the left, obstacle at the right, obstacle behind Buddy, trigger, touch the face of Buddy,
        /// touch the left eye, touche right eye, touch the mouth, touch any part of Buddy, touch body of Buddy,
        /// say something.
        /// </summary>
        private string mConditionType;
        public string ConditionType
        {
            get { return mConditionType; }
            set
            {
                Debug.Log("condition set to: " + value);
                ClearEventTactile();
                if (value == "")
                {
                    Debug.Log("CONDITIONMANAGER LOL RESETPARAM");
                    ResetParam();
                }
                    
                mConditionType = value;
            }
        }

        private string mParamCondition;
        public string ParamCondition { get { return mParamCondition; } set { mParamCondition = value; } }

        [SerializeField]
        private LoopManager mLoopManager;
        //private bool mLoopSensor;
        //public bool LoopSensor { get { return mLoopSensor; } set { mLoopSensor = value; } }

        private bool mSubscribed;
        private float mTimer;
        private float mTimerBis;
        //public float Timer { get { return mTimer; } set { mTimer = value; } }

        private bool mIsInCondition;
        public bool IsInCondition { get { return mIsInCondition; } set { mIsInCondition = value; } }

        [SerializeField]
        private RawImage kikoo;

        /// <summary>
        /// Variables for color detection
        /// </summary>
        private ShadeProcessing mShade;
        private bool mIsColorDetection;
        private Mat mFrame;
        private ShadeEntity[] mShadeEntity;
        private int mAreaBoundingBoxSE;

        // Use this for initialization
        void Start()
        {
            mFrame = new Mat();
            mMotor = BYOS.Instance.Primitive.Motors;
            mFace = BYOS.Instance.Interaction.Face;
            mQRCodeDetect = BYOS.Instance.Perception.QRCode;
            mIRSensors = BYOS.Instance.Primitive.IRSensors;
            mNoiseDetection = BYOS.Instance.Perception.Noise;
            mSTT = BYOS.Instance.Interaction.SpeechToText;
            mShade = BYOS.Instance.Perception.Shade;
            mSpeechReco = "";
            mIsStringSaid = false;
            mHeadMoving = false;
            mBodyMoving = false;
            mIRSensorDetect = false;
            mTactileSubscribed = false;
            mIsTactileDetect = false;
            mIsFireDetect = false;
            mIsColorDetection = false;
            mTimer = 0F;
            mTimerBis = 0;
            mSubscribed = false;
            mConditionType = "";
            mIsEventDone = false;
            LoadCondition();
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            mTimerBis += Time.deltaTime;

            //Debug.Log("CONDITION TYPE : " + mConditionType + " SUBSCRIBE : " + mSubscribed + " Event Done : " + mIsEventDone);
            if (mTimerBis > 1.0F)
            {
                mTimerBis = 0.0F;
                LoadCondition();
                if (mIsTactileDetect)
                {
                    OnBuddyTactile();
                }
                if (mIRSensorDetect)
                    OnObstacleInFront();
                if (mHeadMoving)
                    MovingHead();
                if (mBodyMoving)
                    MovingWheels();
                if (mIsStringSaid)
                {
                    mIsListening = false;
                    TextToSay();
                }
                //if (mIsColorDetection)
                //    ColorDetection();
            }
        }

        private void LoadCondition()
        {
            
            //mIsEventDone = false;
            if (!mSubscribed)
            {
                //Debug.Log("CM : LOADCONDITION SUBSCRIBED");
                switch (mConditionType)
                {
                    case "Fire":
                        Debug.Log("fire");
                        mFireDetection = BYOS.Instance.Perception.Thermal;
                        mFireDetection.OnDetect(OnThermalDetected, 35);
                        mSubscribed = true;
                        break;
                    case "Movement":
                        Debug.Log("movement");
                        mMotion = BYOS.Instance.Perception.Motion;
                        mCam = BYOS.Instance.Primitive.RGBCam;
                        mCam.Resolution = RGBCamResolution.W_320_H_240;
                        mTimer = 0F;
                        mSubscribed = true;
                        mMotion.enabled = true;
                        mMotion.OnDetect(OnMovementDetected, 10f);
                        break;
                    case "AllTactile":
                        Debug.Log("AllTactile");
                        mTactile = TactileEvent.ALL_TACTILE;
                        //ClearEventTactile();
                        mIsTactileDetect = true;
                        mSubscribed = true;
                        break;
                    case "RightEye":
                        Debug.Log("RightEye");
                        mTactile = TactileEvent.RIGHT_EYE;
                        //ClearEventTactile();
                        mIsTactileDetect = true;
                        mSubscribed = true;
                        break;
                    case "LeftEye":
                        Debug.Log("LeftEye");
                        mTactile = TactileEvent.LEFT_EYE;
                        //ClearEventTactile();
                        mIsTactileDetect = true;
                        mSubscribed = true;
                        break;
                    case "Mouth":
                        Debug.Log("Mouth");
                        mTactile = TactileEvent.MOUTH;
                        //ClearEventTactile();
                        mIsTactileDetect = true;
                        mSubscribed = true;
                        break;
                    case "QRCode":
                        Debug.Log("QRCODE");
                        mCam = BYOS.Instance.Primitive.RGBCam;
                        mCam.Resolution = RGBCamResolution.W_320_H_240;
                        mQRCodeDetect.OnDetect(OnQrcodeDetected, QRCodePoints.THIRTEEN_POINTS);
                        mSubscribed = true;
                        break;
                    case "Color":
                        Debug.Log("Color");
                        //mCam = BYOS.Instance.Primitive.RGBCam;
                        //mCam.Open(RGBCamResolution.W_320_H_240);
                        //mIsColorDetection = true;
                        //mSubscribed = true;
                        break;
                    case "HeadTactile":
                        Debug.Log("Head Motor move");
                        mIsTactileDetect = true;
                        mHeadMoving = true;
                        mSubscribed = true;
                        break;
                    case "BodyTactile":
                        Debug.Log("Body Tactile Move");
                        mOriginRobotAngle = mMotor.Wheels.Odometry.z;
                        mIsTactileDetect = true;
                        mBodyMoving = true;
                        mSubscribed = true;
                        break;
                    case "LeftSensor":
                        Debug.Log("Left sensor IR");
                        mIRSensor = IRSensor.LEFT;
                        mIRSensorDetect = true;
                        mSubscribed = true;
                        break;
                    case "RightSensor":
                        Debug.Log("Right sensor IR");
                        mIRSensor = IRSensor.RIGHT;
                        mIRSensorDetect = true;
                        mSubscribed = true;
                        break;
                    case "FrontSensor":
                        Debug.Log("Front sensor IR");
                        mIRSensor = IRSensor.FRONT;
                        mIRSensorDetect = true;
                        mSubscribed = true;
                        break;
                    case "AllSound":
                        Debug.Log("Sound");
                        mNoiseDetection.OnDetect(OnSoundDetected);
                        mSubscribed = true;
                        break;
                    case "SomethingSound":
                        Debug.Log("Sound something");
                        mIsStringSaid = true;
                        mSubscribed = true;
                        break;
                    default:
                        //Debug.Log("no sensors");
                        break;
                }
            }
        }

        private void TextToSay()
        {

            if (string.IsNullOrEmpty(mSpeechReco) && mSTT.HasFinished && !mSpeechReco.Equals(mParamCondition))
            {
                Debug.Log("TEXT TO SAY SI SPEECHRECO NULL: " + mSpeechReco);
                mSTT.Request();
                return;
            }
            Debug.Log("TTS 2");
            if (!mSTT.HasFinished)
            {
                if (mSTT.LastAnswer != null)
                {
                    mSpeechReco = mSTT.LastAnswer;
                }
            }
            if (mSpeechReco.Equals(mParamCondition))
            {
                if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
                {
                    mConditionType = "";
                    ClearEventTactile();
                    mLoopManager.ChangeIndex = true;
                    mLoopManager.IsSensorLoopWithParam = false;
                }
                ResetParam();
            }
            else
                mSpeechReco = "";
        }

        private bool OnSoundDetected(float iSound)
        {
            if (iSound > (1 - (0.4F/ 100.0f)) * MAX_SOUND_THRESHOLD)
            {
                Debug.Log("Sound DETECTED");
                if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
                {
                    mConditionType = "";
                    ClearEventTactile();
                    mLoopManager.ChangeIndex = true;
                    mLoopManager.IsSensorLoopWithParam = false;
                }
                ResetParam();
                return true;
            }
            return true;
        }

        private bool ColorDetection()
        {
            if (mCam.FrameMat != null)
            {
                mFrame = mCam.FrameMat.clone();
                //OpenCVUnity.Rect mRec = new OpenCVUnity.Rect(80, 60, 160, 120);
                //Mat mRoi = mFrame.submat(mRec);
                mShadeEntity = mShade.FindColor(mFrame, new Color32(255, 0, 0, 100));
                for (int i = 0; i < mShadeEntity.Length; ++i)
                {
                    mAreaBoundingBoxSE = mShadeEntity[i].RectInFrame.height * mShadeEntity[i].RectInFrame.width;
                    Imgproc.circle(mFrame, Utils.Center(mShadeEntity[i].RectInFrame), 3, new Scalar(0, 255, 0), 3);
                    kikoo.texture = Utils.MatToTexture2D(mFrame);
                    int mAreaMat = mFrame.width() * mFrame.height();
                    if (mAreaBoundingBoxSE / mAreaMat >= 0.5)
                    {
                        Debug.Log("Good");
                        return true;
                    }
                }
            }
            return false;
        }

        private bool OnMovementDetected(MotionEntity[] iMotion)
        {
            //Debug.Log("ONMOVEMENTDETECTED 1 : ");
            //if(iMotion.Length == 0 || iMotion == null)
            //    Debug.Log("ONMOVEMENTDETECTED NULL IMOTION : ");
            if (mTimer > 1.5F && iMotion.Length > 2)
            {
                //Debug.Log("ONMOVEMENTDETECTED 2 : ");
                if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
                {
                    mConditionType = "";
                    ClearEventTactile();
                    mLoopManager.ChangeIndex = true;
                    mLoopManager.IsSensorLoopWithParam = false;
                }
                ResetParam();
                mCam.Close();
                mMotion.StopOnDetect(OnMovementDetected);
                return true;
            }
            return true;
        }

        private bool OnThermalDetected(ObjectEntity[] iObject)
        {
            Debug.Log("FIRE DETECTED");
            if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
            {
                mConditionType = "";
                ClearEventTactile();
                mLoopManager.ChangeIndex = true;
                mLoopManager.IsSensorLoopWithParam = false;
            }
            ResetParam();
            mFireDetection.StopOnDetect(OnThermalDetected);
            return true;
        }

        private bool OnQrcodeDetected(QRCodeEntity[] iQRCodeEntity)
        {

            for(int i = 0; i < iQRCodeEntity.Length; ++i)
            {
               //Texture2D text =  Utils.MatToTexture2D(iQRCodeEntity[i].MatInFrame);
               // kikoo.texture = text;
                Debug.Log("Label : " + iQRCodeEntity[i].Label + " et i : " + i + iQRCodeEntity[i].MatInFrame == null);
                if (iQRCodeEntity[i].Label == mParamCondition )
                {
                    //if (mLoopManager.IsSensorLoopWithParam)
                    //{
                    //    mConditionType = "";
                    //    ClearEventTactile();
                    //    mLoopManager.ChangeIndex = true;
                    //    mLoopManager.IsSensorLoopWithParam = false;
                    //}
                    ResetParam();
                    mCam.Close();
                    mQRCodeDetect.StopOnDetect(OnQrcodeDetected);
                    return true;
                }
            }
            
            return true;
        }

        private void OnObstacleInFront()
        {
            if(mIRSensor == IRSensor.FRONT)
            {
                if (mIRSensors.Middle.Distance < OBSTACLE_DISTANCE && mIRSensors.Middle.Distance != 0)
                {
                    if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
                    {
                        mConditionType = "";
                        ClearEventTactile();
                        mLoopManager.ChangeIndex = true;
                        mLoopManager.IsSensorLoopWithParam = false;
                    }
                    ResetParam();
                }
            }
            if(mIRSensor == IRSensor.LEFT)
            {
                if (mIRSensors.Left.Distance < OBSTACLE_DISTANCE && mIRSensors.Left.Distance != 0)
                {
                    if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
                    {
                        mConditionType = "";
                        ClearEventTactile();
                        mLoopManager.ChangeIndex = true;
                        mLoopManager.IsSensorLoopWithParam = false;
                    }
                    ResetParam();
                }
            }
            if(mIRSensor == IRSensor.RIGHT)
            {
                if (mIRSensors.Right.Distance < OBSTACLE_DISTANCE && mIRSensors.Right.Distance != 0)
                {
                    if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
                    {
                        mConditionType = "";
                        ClearEventTactile();
                        mLoopManager.ChangeIndex = true;
                        mLoopManager.IsSensorLoopWithParam = false;
                    }
                    ResetParam();
                }
            }
        }

        private void OnBuddyTactile()
        {
            if(mTactile == TactileEvent.ALL_TACTILE)
            {
                //Debug.Log("IN THE TACTILE MAGGLE");
                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("KIKOO ALL TACTILE MODAFUKA");
                    if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
                    {
                        mConditionType = "";
                        ClearEventTactile();
                        mLoopManager.ChangeIndex = true;
                        mLoopManager.IsSensorLoopWithParam = false;
                    }
                    ResetParam();
                }
            }
            if(mTactile == TactileEvent.LEFT_EYE && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                //ClearEventTactile();
                mFace.OnClickLeftEye.Add(OnLeftEyeClicked);
            }
            if(mTactile == TactileEvent.RIGHT_EYE && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                //ClearEventTactile();
                mFace.OnClickRightEye.Add(OnRightEyeClicked);
            }
            if(mTactile == TactileEvent.MOUTH && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                //ClearEventTactile();
                mFace.OnClickMouth.Add(OnMouthClicked);
            }
            if(mTactile == TactileEvent.BODY_MOVING)
            {
                MovingWheels();
            }
            if(mTactile == TactileEvent.HEAD_MOVING)
            {
                MovingHead();
            }
            
        }

        private void MovingWheels()
        {
            if (Mathf.Abs(mMotor.Wheels.Odometry.z - mOriginRobotAngle) > ANGLE_THRESH)
            {
                if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
                {
                    mConditionType = "";
                    ClearEventTactile();
                    mLoopManager.ChangeIndex = true;
                    mLoopManager.IsSensorLoopWithParam = false;
                }
                ResetParam();
            }
        }

        private void MovingHead()
        {
            if (Mathf.Abs(mMotor.NoHinge.CurrentAnglePosition) > ANGLE_THRESH)
            {
                if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
                {
                    mConditionType = "";
                    ClearEventTactile();
                    mLoopManager.ChangeIndex = true;
                    mLoopManager.IsSensorLoopWithParam = false;
                }
                ResetParam();
            }
        }

        private void OnLeftEyeClicked()
        {
            if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
            {
                mConditionType = "";
                ClearEventTactile();
                mLoopManager.ChangeIndex = true;
                mLoopManager.IsSensorLoopWithParam = false;
            }
            ResetParam();
        }

        private void OnRightEyeClicked()
        {
            if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
            {
                mConditionType = "";
                ClearEventTactile();
                mLoopManager.ChangeIndex = true;
                mLoopManager.IsSensorLoopWithParam = false;
            }
            ResetParam();
        }

        private void OnMouthClicked()
        {
            //Debug.Log("mouth clicked KIKOO CONDITION MANAGER");
            if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
            {
                mConditionType = "";
                ClearEventTactile();
                mLoopManager.ChangeIndex = true;
                mLoopManager.IsSensorLoopWithParam = false;
            }
            ResetParam();
        }

        private void ResetParam()
        {
            Debug.Log("RESETPARAM ");
            mIsStringSaid = false;
            mSpeechReco = "";
            mHeadMoving = false;
            mBodyMoving = false;
            mIRSensorDetect = false;
            mIsTactileDetect = false;
            mIsEventDone = true;
            mTactile = TactileEvent.NONE;
            mSubscribed = false;
            mTimer = 0F;
            mConditionType = "";
            mTactileSubscribed = false;
        }

        private void ClearEventTactile()
        {
            mFace.OnClickLeftEye.Clear();
            mFace.OnClickMouth.Clear();
            mFace.OnClickRightEye.Clear();
        }
       
    }
}
