using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;
using OpenCVUnity;

namespace BuddyApp.BuddyLab
{
    public sealed class ConditionManager : MonoBehaviour
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
        //private Buddy.Actuators. Motors mMotor;
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
        //private IRSensors mIRSensors;

        private bool mHeadMoving;
        private bool mBodyMoving;

        /// <summary>
        /// Variables for movement detection
        /// </summary>
        private MotionDetector mMotion;
        private HDCamera mCam;

        /// <summary>
        /// Variables for fire detection
        /// </summary>
        private ThermalDetector mFireDetection;
        private bool mIsFireDetect;

        private QRCodeDetector mQRCodeDetect;

        /// <summary>
        /// Boolean which says if the Event is done.
        /// </summary>
        private bool mIsEventDone;
        public bool IsEventDone { get { return mIsEventDone; } set { mIsEventDone = value; } }

        /// <summary>
        /// Variables for sound detection
        /// </summary>
        public const float MAX_SOUND_THRESHOLD = 0.05F; 
        private NoiseDetector mNoiseDetection;

        /// <summary>
        /// Variables for specific string to detect
        /// </summary>
        private bool mIsStringSaid;
        private bool mIsListening;
        private string mSpeechReco;
        //private SpeechToText mSTT;

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

        //[SerializeField]
        //private RawImage kikoo;

        /// <summary>
        /// Variables for color detection
        /// </summary>
        private ShadeProcessing mShade;
        private bool mIsColorDetection;
        private Mat mFrame;
        private ShadeEntity[] mShadeEntity;
        private float mAreaBoundingBoxSE;
        private Texture2D mTexture;
        private Mat lMat;
        private Mat lRoi;
        private Mat lResized;
        private Color32 mColorToDetect;

        // Use this for initialization
        void Start()
        {
            lRoi = new Mat();
            lResized = new Mat();
            lMat = new Mat();
            mFrame = new Mat();
            //mMotor = BYOS.Instance.Primitive.Motors;
            //mFace = BYOS.Instance.Interaction.Face;
            //mQRCodeDetect = BYOS.Instance.Perception.QRCode;
            //mIRSensors = BYOS.Instance.Primitive.IRSensors;
            //mNoiseDetection = BYOS.Instance.Perception.Noise;
            //mSTT = BYOS.Instance.Interaction.SpeechToText;
            //mShade = BYOS.Instance.Perception.Shade;
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
                if (mIsColorDetection)
                    ColorDetection();
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
                        mFireDetection = Buddy.Perception.ThermalDetector;
                        mFireDetection.OnDetect.AddP(OnThermalDetected, 35);
                        mSubscribed = true;
                        break;
                    case "Movement":
                        Debug.Log("movement");
                        //mMotion = BYOS.Instance.Perception.Motion;
                        //mCam = BYOS.Instance.Primitive.RGBCam;
                        Buddy.Sensors.HDCamera.Mode = HDCameraMode.COLOR_640x480_30FPS_RGB;
                        mTimer = 0F;
                        mSubscribed = true;
                        //mMotion.enabled = true;
                        //mMotion.OnDetect.AddP(OnMovementDetected, 10f);
                        mMotion.OnDetect.AddP(OnMovementDetected);
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
                        mCam = Buddy.Sensors.HDCamera;
                        mCam.Mode = HDCameraMode.COLOR_640x480_30FPS_RGB;
                        mQRCodeDetect.OnDetect.AddP(OnQrcodeDetected, QRCodePoints.THIRTEEN_POINTS);
                        mSubscribed = true;
                        break;
                    case "Color":
                        Debug.Log("Color");
                        mCam = Buddy.Sensors.HDCamera;
                        mCam.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);
                        if (mParamCondition == "Blue")
                            mColorToDetect = new Color32(0, 0, 255, 100);
                        else if (mParamCondition == "Green")
                            mColorToDetect = new Color32(0, 255, 0, 100);
                        else if (mParamCondition == "Yellow")
                            mColorToDetect = new Color32(255, 255, 0, 100);
                        else if (mParamCondition == "Orange")
                            mColorToDetect = new Color32(255, 69, 0, 100);
                        else if (mParamCondition == "Red")
                            mColorToDetect = new Color32(255, 0, 0, 100);
                        else if (mParamCondition == "Pink")
                            mColorToDetect = new Color32(255, 105, 180, 100);
                        else if (mParamCondition == "Purple")
                            mColorToDetect = new Color32(128, 0, 128, 100);
                        else if (mParamCondition == "DarkBlue")
                            mColorToDetect = new Color32(0, 0, 139, 100);
                        mIsColorDetection = true;
                        mSubscribed = true;
                        break;
                    case "HeadTactile":
                        Debug.Log("Head Motor move");
                        mIsTactileDetect = true;
                        mHeadMoving = true;
                        mSubscribed = true;
                        break;
                    case "BodyTactile":
                        Debug.Log("Body Tactile Move");
                        mOriginRobotAngle = Buddy.Actuators.Wheels.Odometry.z;
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
                        mNoiseDetection.OnDetect.AddP(OnSoundDetected);
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

            if (string.IsNullOrEmpty(mSpeechReco) && !Buddy.Vocal.IsBusy && !mSpeechReco.Equals(mParamCondition))
            {
                Debug.Log("TEXT TO SAY SI SPEECHRECO NULL: " + mSpeechReco);
                Buddy.Vocal.Listen();
                return;
            }
            if (Buddy.Vocal.IsBusy)
            {
                if (Buddy.Vocal.LastHeardInput.Utterance != null)
                {
                    mSpeechReco = Buddy.Vocal.LastHeardInput.Utterance;
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
            if (iSound > (1 - (0.4F / 100.0f)) * MAX_SOUND_THRESHOLD)
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

        //protected void Display(Mat iMatToDisplay)
        //{

        //    mTexture = Utils.ScaleTexture2DFromMat(iMatToDisplay, mTexture);
        //    Utils.MatToTexture2D(iMatToDisplay, mTexture);
        //    kikoo.texture = mTexture;
        //}

        private bool ColorDetection()
        {
            if (mCam.Frame != null)
            {
                mFrame = mCam.Frame.clone();
                OpenCVUnity.Rect mRec = new OpenCVUnity.Rect(mCam.Width / 4, mCam.Height / 4 , mCam.Width / 2, mCam.Height / 2);
                //Imgproc.cvtColor(mFrame, lGrayMat, Imgproc.COLOR_RGB2GRAY);
                //Mat lRoi = new Mat(lGrayMat, mRec);
                //Imgproc.resize(lRoi, lResized, lGrayMat.size());
                //Display(lResized);
                lRoi = new Mat(mFrame, mRec);
                //Imgproc.resize(lRoi, lResized, lMat.size());
                mShadeEntity = mShade.FindColor(lRoi, mColorToDetect);
                for (int i = 0; i < mShadeEntity.Length; ++i)
                {
                    mAreaBoundingBoxSE = mShadeEntity[i].RectInFrame.height * mShadeEntity[i].RectInFrame.width;
                    //Imgproc.rectangle(mFrame, new Point(mShadeEntity[i].RectInFrame.x + (mCam.Width / 4), mShadeEntity[i].RectInFrame.y + (mCam.Height / 4)), new Point(mShadeEntity[i].RectInFrame.x + mShadeEntity[i].RectInFrame.width, mShadeEntity[i].RectInFrame.y + mShadeEntity[i].RectInFrame.height), new Scalar(255, 0, 0), -1);
                    float mAreaMat = lRoi.width() * lRoi.height();
                    if (mAreaBoundingBoxSE / mAreaMat > 0.75)
                    {
                        Debug.Log("Good");
                        ResetParam();
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
                mMotion.OnDetect.RemoveP(OnMovementDetected);
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
            mFireDetection.OnDetect.RemoveP(OnThermalDetected);
            return true;
        }

        private bool OnQrcodeDetected(QRCodeEntity[] iQRCodeEntity)
        {

            for (int i = 0; i < iQRCodeEntity.Length; ++i)
            {
                //Texture2D text =  Utils.MatToTexture2D(iQRCodeEntity[i].MatInFrame);
                // kikoo.texture = text;
                Debug.Log("Label : " + iQRCodeEntity[i].Label + " et i : " + i + iQRCodeEntity[i].MatInFrame == null);
                if (iQRCodeEntity[i].Label == mParamCondition)
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
                    mQRCodeDetect.OnDetect.RemoveP(OnQrcodeDetected);
                    return true;
                }
            }

            return true;
        }

        //A REFAIRE AVEC LES NOUVEAUX SENSORS 
        private void OnObstacleInFront()
        {
            //if (mIRSensor == IRSensor.FRONT)
            //{
            //    if (mIRSensors.Middle.Distance < OBSTACLE_DISTANCE && mIRSensors.Middle.Distance != 0)
            //    {
            //        if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
            //        {
            //            mConditionType = "";
            //            ClearEventTactile();
            //            mLoopManager.ChangeIndex = true;
            //            mLoopManager.IsSensorLoopWithParam = false;
            //        }
            //        ResetParam();
            //    }
            //}
            //if (mIRSensor == IRSensor.LEFT)
            //{
            //    if (mIRSensors.Left.Distance < OBSTACLE_DISTANCE && mIRSensors.Left.Distance != 0)
            //    {
            //        if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
            //        {
            //            mConditionType = "";
            //            ClearEventTactile();
            //            mLoopManager.ChangeIndex = true;
            //            mLoopManager.IsSensorLoopWithParam = false;
            //        }
            //        ResetParam();
            //    }
            //}
            //if (mIRSensor == IRSensor.RIGHT)
            //{
            //    if (mIRSensors.Right.Distance < OBSTACLE_DISTANCE && mIRSensors.Right.Distance != 0)
            //    {
            //        if (mLoopManager.IsSensorLoopWithParam && !mIsInCondition)
            //        {
            //            mConditionType = "";
            //            ClearEventTactile();
            //            mLoopManager.ChangeIndex = true;
            //            mLoopManager.IsSensorLoopWithParam = false;
            //        }
            //        ResetParam();
            //    }
            //}
        }

        private void OnBuddyTactile()
        {
            if (mTactile == TactileEvent.ALL_TACTILE)
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
            if (mTactile == TactileEvent.LEFT_EYE && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                //ClearEventTactile();
                mFace.OnTouchLeftEye.Add(OnLeftEyeClicked);
            }
            if (mTactile == TactileEvent.RIGHT_EYE && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                //ClearEventTactile();
                mFace.OnTouchRightEye.Add(OnRightEyeClicked);
            }
            if (mTactile == TactileEvent.MOUTH && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                //ClearEventTactile();
                mFace.OnTouchMouth.Add(OnMouthClicked);
            }
            if (mTactile == TactileEvent.BODY_MOVING)
            {
                MovingWheels();
            }
            if (mTactile == TactileEvent.HEAD_MOVING)
            {
                MovingHead();
            }

        }

        private void MovingWheels()
        {
            if (Mathf.Abs(Buddy.Actuators.Wheels.Odometry.z - mOriginRobotAngle) > ANGLE_THRESH)
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
            if (Mathf.Abs(Buddy.Actuators.Head.No.Angle) > ANGLE_THRESH)
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
            mIsColorDetection = false;
            mIsEventDone = true;
            mTactile = TactileEvent.NONE;
            mSubscribed = false;
            mTimer = 0F;
            mConditionType = "";
            mTactileSubscribed = false;
        }

        private void ClearEventTactile()
        {
            mFace.OnTouchLeftEye.Clear();
            mFace.OnTouchMouth.Clear();
            mFace.OnTouchRightEye.Clear();
        }

    }
}
