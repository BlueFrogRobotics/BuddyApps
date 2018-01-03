using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using UnityEngine.UI;

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

        //[SerializeField]
        //private RawImage kikoo;

        /// <summary>
        /// String that describes the type of condition. Conditions : Fire, movement, obstacles in front of Buddy,
        /// obstacles at the left, obstacle at the right, obstacle behind Buddy, trigger, touch the face of Buddy,
        /// touch the left eye, touche right eye, touch the mouth, touch any part of Buddy, touch body of Buddy,
        /// say something.
        /// </summary>
        private string mConditionType;
        public string ConditionType { get { return mConditionType; } set { ClearEventTactile(); mConditionType = value; } }

        private string mParamCondition;
        public string ParamCondition { get { return mParamCondition; } set { mParamCondition = value; } }



        private bool mSubscribed;
        

        private float mTimer;
        //public float Timer { get { return mTimer; } set { mTimer = value; } }

        // Use this for initialization
        void Start()
        {
            mMotor = BYOS.Instance.Primitive.Motors;
            mFace = BYOS.Instance.Interaction.Face;
            mQRCodeDetect = BYOS.Instance.Perception.QRCode;
            mIRSensors = BYOS.Instance.Primitive.IRSensors;
            mIRSensorDetect = false;
            mTactileSubscribed = false;
            mIsTactileDetect = false;
            mIsFireDetect = false;
            mTimer = 0F;
            mSubscribed = false;
            mConditionType = "";
            mIsEventDone = false;
            LoadCondition();
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            //Debug.Log("CONDITION TYPE : " + mConditionType + " SUBSCRIBE : " + mSubscribed + " Event Done : " + mIsEventDone);
            LoadCondition();
            if (mIsTactileDetect)
                OnBuddyTactile();
            if (mIRSensorDetect)
                OnObstacleInFront();
        }

        private void LoadCondition()
        {
            //mIsEventDone = false;
            if (!mSubscribed)
            {
                switch (mConditionType)
                {
                    case "Fire":
                        Debug.Log("fire");
                        mFireDetection = BYOS.Instance.Perception.Thermal;
                        mFireDetection.OnDetect(OnThermalDetected, 50);
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
                        break;
                    case "HeadTactile":
                        Debug.Log("Head Motor move");
                        mIsTactileDetect = true;
                        mSubscribed = true;
                        break;
                    case "BodyTactile":
                        Debug.Log("Body Tactile Move");
                        mOriginRobotAngle = mMotor.Wheels.Odometry.z;
                        mIsTactileDetect = true;
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
                    default:
                        //Debug.Log("no sensors");
                        break;
                }
            }
        }

        private bool OnMovementDetected(MotionEntity[] iMotion)
        {
            if (mTimer > 1.5F && iMotion.Length > 2)
            {
                ResetParam();
                mCam.Close();
                mMotion.StopOnDetect(OnMovementDetected);
                return true;
            }
            return true;
        }

        private bool OnThermalDetected(ObjectEntity[] iObject)
        {
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
                    ResetParam();
                }
            }
            if(mIRSensor == IRSensor.LEFT)
            {
                if (mIRSensors.Left.Distance < OBSTACLE_DISTANCE && mIRSensors.Left.Distance != 0)
                {
                    ResetParam();
                }
            }
            if(mIRSensor == IRSensor.RIGHT)
            {
                if (mIRSensors.Right.Distance < OBSTACLE_DISTANCE && mIRSensors.Right.Distance != 0)
                {
                    ResetParam();
                }
            }
        }

        private void OnBuddyTactile()
        {
            Debug.Log("ONFACETOUCHED " + mConditionType);
            if(mTactile == TactileEvent.ALL_TACTILE)
            {

                Debug.Log("ALLTACTILE");
                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                {
                    Debug.Log("TOUCHED HEAD TACTILE");
                    ResetParam();
                }
            }
            if(mTactile == TactileEvent.LEFT_EYE && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                Debug.Log("LEFT EYE TACTILE");
                //ClearEventTactile();
                mFace.OnClickLeftEye.Add(OnLeftEyeClicked);
            }
            if(mTactile == TactileEvent.RIGHT_EYE && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                Debug.Log("RIGHT EYE TACTILE");
                //ClearEventTactile();
                mFace.OnClickRightEye.Add(OnRightEyeClicked);
            }
            if(mTactile == TactileEvent.MOUTH && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                Debug.Log("MOUTH TACTILE");
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
                ResetParam();
            }
        }

        private void MovingHead()
        {
            if (Mathf.Abs(mMotor.NoHinge.CurrentAnglePosition) > ANGLE_THRESH)
            {
                ResetParam();
            }
        }

        private void OnLeftEyeClicked()
        {
            ResetParam();
        }

        private void OnRightEyeClicked()
        {
            ResetParam();
        }

        private void OnMouthClicked()
        {
            ResetParam();
        }


        private void ResetParam()
        {
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
