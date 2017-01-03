using UnityEngine;
using UnityEngine.UI;
using BuddyOS.UI;
using BuddyOS;
using System.Collections.Generic;
using OpenCVUnity;

namespace BuddyApp.BabyPhone
{
    [RequireComponent(typeof(InputMicro))]
    [RequireComponent(typeof(AudioSource))]
    public class DebugSoundDetection : MonoBehaviour
    {
        [SerializeField]
        private RawImage soundViewer;

        [SerializeField]
        private Gauge microSensitivity;

        [SerializeField]
        private Text labelSound;

        private Animator debugSoundAnimator;
        private Dictionary mDictionary;

        private float mMaxThreshold;
        private InputMicro mInputMicro;
        private Texture2D mTexture;
        private float mSound;
        private Mat mMatShow;
        private float mTimer;

        void OnEnable()
        {
            mDictionary = BYOS.Instance.Dictionary;
            debugSoundAnimator = GetComponent<Animator>();
            mInputMicro = GetComponent<InputMicro>();
            debugSoundAnimator.SetTrigger("Open_WDebugs");
            mMatShow = new Mat(480, 640, CvType.CV_8UC3, new Scalar(255, 255, 255, 255));
            mTimer = 0;
        }

        void OnDisable()
        {
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
            microSensitivity.Slider.minValue = 5;
            microSensitivity.Slider.maxValue = 20;
            microSensitivity.Slider.value = BabyPhoneData.Instance.MicrophoneSensitivity;
            microSensitivity.UpdateCommands.Add(new SetMicroSensCmd());
            mMaxThreshold = microSensitivity.Slider.maxValue;
        }

        void Upadte()
        {
            mTimer += Time.deltaTime;

            if (mTimer > 0.1f)
            {
                float lLevelSound = mInputMicro.Loudness;
                float lThreshold = microSensitivity.Slider.value;

                Imgproc.rectangle(mMatShow, new Point(0, 480), new Point(640, 480.0f - lLevelSound), new Scalar(0, 212, 209, 255), -1);
                Imgproc.line(mMatShow, new Point(0, 480.0f - lThreshold * 400 / microSensitivity.Slider.maxValue), new Point(640, 480.0f - lThreshold * 400 / mMaxThreshold), new Scalar(237, 27, 36, 255), 3);
                Debug.Log("niveau: " + lThreshold);

                BuddyTools.Utils.MatToTexture2D(mMatShow, mTexture);
                soundViewer.texture = mTexture;
            }

        }
    }
}
