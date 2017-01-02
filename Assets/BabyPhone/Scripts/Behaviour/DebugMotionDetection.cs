using UnityEngine;
using UnityEngine.UI;
using BuddyOS.UI;
using BuddyOS;
using System.Collections.Generic;
using OpenCVUnity;

namespace BuddyApp.BabyPhone
{
    public class DebugMotionDetection : MonoBehaviour
    {
        private Animator debugCameraAnimator;

        [SerializeField]
        private BabyPhoneMotionDetection detector;

        [SerializeField]
        private RawImage mRaw;
        [SerializeField]
        private Gauge cameraSensitivity;

        [SerializeField]
        private Text labelMotion;

        private Dictionary mDictionary;
        private Mat mImage;
        private Mat mMatRed;
        private RGBCam mCam;
        private Texture2D mTexture;
        private RGBCam mRGBCam;

        //Mat lMatMouv; 
        Mat lMatView;
        Mat lMatCam; 

        void OnEnable()
        {
            mCam = BYOS.Instance.RGBCam;
            mDictionary = BYOS.Instance.Dictionary;
            debugCameraAnimator = GetComponent<Animator>();
            debugCameraAnimator.SetTrigger("Open_WDebugs");

            //lMatMouv = new Mat();
            lMatView = new Mat();
            lMatCam = new Mat();
        }

        void OnDisable()
        {
            debugCameraAnimator.SetTrigger("Close_WDebugs");
            mRGBCam.Close();
               
        }

        void Start()
        {
            Init();
            Labelize();
        }

        void Update()
        {
            mImage = detector.mBinaryIm;
            Imgproc.circle(mImage, detector.mPoint, 5, new Scalar(254, 254, 254), -1);
            Mat lMatMouv = new Mat();
            mMatRed.copyTo(lMatMouv, mImage);
            Imgproc.threshold(mImage, mImage, 200, 255, Imgproc.THRESH_BINARY_INV);
            mCam.FrameMat.copyTo(lMatCam, mImage);
            lMatView = lMatMouv + lMatCam;
            BuddyTools.Utils.MatToTexture2D(lMatView, mTexture);
            mRaw.texture = mTexture;
        }

        public void Labelize()
        {
            cameraSensitivity.Label.text = mDictionary.GetString("motiondetect");
            labelMotion.text = mDictionary.GetString("camsens");
        }

        public void Init()
        {
            ////sound detection
            cameraSensitivity.DisplayPercentage = true;
            cameraSensitivity.Slider.minValue = 20;
            cameraSensitivity.Slider.maxValue = 200;
            cameraSensitivity.Slider.value = BabyPhoneData.Instance.CameraSensitivity;
            cameraSensitivity.UpdateCommands.Add(new SetCamSensCmd());
        }


    }
}
