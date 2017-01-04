using UnityEngine;
using UnityEngine.UI;
using BuddyOS.UI;
using BuddyOS;
using System.Collections.Generic;
using OpenCVUnity;

namespace BuddyApp.BabyPhone
{
    //[RequireComponent(typeof(InputMicro))]
    [RequireComponent(typeof(AudioSource))]
    public class DebugSoundDetection : MonoBehaviour
    {
        [SerializeField]
        private RawImage soundViewer;

        [SerializeField]
        private Gauge microSensitivity;

        [SerializeField]
        private Text labelSound;

        [SerializeField]
        private SoundDetect mSoundDetector;

        private Animator debugSoundAnimator;
        private Dictionary mDictionary;

        private float mMaxThreshold;

        //private InputMicro mInputMicro;
        private Texture2D mTexture;
        private float mSound;
        private Mat mMatShow;
        private float mTimer;

        void OnEnable()
        {         
            mDictionary = BYOS.Instance.Dictionary;
            debugSoundAnimator = GetComponent<Animator>();

            mSoundDetector.Init();

            mTexture = new Texture2D(640, 480);
            debugSoundAnimator.SetTrigger("Open_WDebugs");
            mTimer = 0;
        }

        void OnDisable()
        {
            mSoundDetector.Stop();
            debugSoundAnimator.SetTrigger("Close_WDebugs");
        }

        void Start()
        {
            Init();
            Labelize();
        }

        public void Labelize()
        {
            microSensitivity.Label.text = mDictionary.GetString("soundetect");
            labelSound.text = mDictionary.GetString("microsens");
        }

        public void Init()
        {
            ////sound detection
            microSensitivity.DisplayPercentage = true;
            microSensitivity.Slider.minValue = 0;
            microSensitivity.Slider.maxValue = 10;
            microSensitivity.Slider.value = BabyPhoneData.Instance.MicrophoneSensitivity;
            microSensitivity.UpdateCommands.Add(new SetMicroSensCmd());
            mMaxThreshold = microSensitivity.Slider.maxValue;
        }

        void Update()
        {
            mTimer += Time.deltaTime;
            if (mTimer > 0.04f)
            {
                mTimer = 0.0f;
                mMatShow = new Mat(480, 640, CvType.CV_8UC3, new Scalar(255, 255, 255, 255));

                float lMaxThreshold = mSoundDetector.GetMaxThreshold();
                float lThreshold = (1.0f - microSensitivity.Slider.value / microSensitivity.Slider.maxValue) * lMaxThreshold;

                mSoundDetector.SetThreshold(lThreshold);
 

                float lLevelSound = (mSoundDetector.Value) * 400.0f / lMaxThreshold;
                //Imgproc.line(mMatShow, new Point(0, 480.0f - lLevelSound), new Point(640, 480.0f - lLevelSound), new Scalar(0, 0, 255, 255));
                Imgproc.rectangle(mMatShow, new Point(0, 480), new Point(640, 480.0f - lLevelSound), new Scalar(0, 212, 209, 255), -1);
                Imgproc.line(mMatShow, new Point(0, 480.0f - lThreshold * 400 / lMaxThreshold), new Point(640, 480.0f - lThreshold * 400 / lMaxThreshold), new Scalar(237, 27, 36, 255), 3);
                //Debug.Log("niveau: " + lThreshold);
                BuddyTools.Utils.MatToTexture2D(mMatShow, mTexture);
                soundViewer.texture = mTexture;
            }
        }
    }
}
