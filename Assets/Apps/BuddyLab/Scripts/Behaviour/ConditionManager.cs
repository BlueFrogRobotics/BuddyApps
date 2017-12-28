using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;


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
            HEAD,
            MOUTH,
            NONE
        }
        private bool mIsTactileDetect;
        private TactileEvent mTactile;
        private Face mFace;
        private bool mTactileSubscribed;

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
        /// String that describes the type of condition. Conditions : Fire, movement, obstacles in front of Buddy,
        /// obstacles at the left, obstacle at the right, obstacle behind Buddy, trigger, touch the face of Buddy,
        /// touch the left eye, touche right eye, touch the mouth, touch any part of Buddy, touch body of Buddy,
        /// say something.
        /// </summary>
        private string mConditionType;
        public string ConditionType { get { return mConditionType; } set { mConditionType = value; } }

        private string mParamCondition;
        public string ParamCondition { get { return mParamCondition; } set { mParamCondition = value; } }


        private bool mSubscribed;

        private float mTimer;
        //public float Timer { get { return mTimer; } set { mTimer = value; } }

        // Use this for initialization
        void Start()
        {
            mFace = BYOS.Instance.Interaction.Face;
            mQRCodeDetect = BYOS.Instance.Perception.QRCode;
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
                OnFaceTouched();
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
                    case "HeadTactile":
                        Debug.Log("HeadTactile");
                        mTactile = TactileEvent.HEAD;
                        mIsTactileDetect = true;
                        break;
                    case "RightEye":
                        Debug.Log("RightEye");
                        mTactile = TactileEvent.RIGHT_EYE;
                        mIsTactileDetect = true;
                        break;
                    case "LeftEye":
                        Debug.Log("LeftEye");
                        mTactile = TactileEvent.LEFT_EYE;
                        mIsTactileDetect = true;
                        break;
                    case "Mouth":
                        Debug.Log("Mouth");
                        mTactile = TactileEvent.MOUTH;
                        mIsTactileDetect = true;
                        break;
                    case "QRCode":
                        Debug.Log("QRCODE");
                        mCam = BYOS.Instance.Primitive.RGBCam;
                        mCam.Resolution = RGBCamResolution.W_320_H_240;
                        mQRCodeDetect.OnDetect(OnQrcodeDetected, QRCodePoints.THIRTEEN_POINTS);
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
                if (iQRCodeEntity[i].Label == mParamCondition)
                {
                    ResetParam();
                    mCam.Close();
                    mQRCodeDetect.StopOnDetect(OnQrcodeDetected);
                    return true;
                }
            }
            
            return false;
        }

        private void OnFaceTouched()
        {
            Debug.Log("ONFACETOUCHED " + mConditionType);
            if(mTactile == TactileEvent.HEAD && !mTactileSubscribed)
            {

                Debug.Log("HEAD TACTILE");
                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                {
                    
                    ResetParam();
                }
            }
            if(mTactile == TactileEvent.LEFT_EYE && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                Debug.Log("LEFT EYE TACTILE");
                mFace.OnClickLeftEye.Add(OnLeftEyeClicked);
            }
            if(mTactile == TactileEvent.RIGHT_EYE && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                Debug.Log("RIGHT EYE TACTILE");
                mFace.OnClickRightEye.Add(OnRightEyeClicked);
            }
            if(mTactile == TactileEvent.MOUTH && !mTactileSubscribed)
            {
                mTactileSubscribed = true;
                Debug.Log("MOUTH TACTILE");
                mFace.OnClickMouth.Add(OnMouthClicked);
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
            //if(mTactile == TactileEvent.LEFT_EYE)
                mFace.OnClickLeftEye.Remove(OnLeftEyeClicked);
            //else if (mTactile == TactileEvent.RIGHT_EYE)
                mFace.OnClickRightEye.Remove(OnRightEyeClicked);
            //else if (mTactile == TactileEvent.MOUTH)
                mFace.OnClickMouth.Remove(OnMouthClicked);
            mIsTactileDetect = false;
            mIsEventDone = true;
            mTactile = TactileEvent.NONE;
            mSubscribed = false;
            mTimer = 0F;
            mConditionType = "";
        }
    }
}
