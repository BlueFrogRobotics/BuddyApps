using UnityEngine;
using System.Collections;

namespace BuddyApp.BabyPhone
{
    public class InputMicro : MonoBehaviour
    {
        private AudioClip mClipRecord;
        private string mDevice;
        private float[] mWaveData;
        private int mSampleWindow;
        private bool mIsInitialized;

        public float Loudness { get; set; }

        void Start()
        {
            mSampleWindow = 128;
            mClipRecord = new AudioClip();
            mWaveData = new float[mSampleWindow];
        }

        void Update()
        {
            Loudness = LevelMax();
        }

        // start mic when scene starts
        void OnEnable()
        {
            InitMic();
            mIsInitialized = true;
        }

        /// <summary>
        /// Stop mic when loading a new level or quit application
        /// </summary>
        void OnDisable()
        {
            StopMicrophone();
        }

        void OnDestroy()
        {
            StopMicrophone();
        }

        /// <summary>
        /// Make sure the mic gets started & stopped when application gets focused
        /// </summary>
        /// <param name="iIsFocus"></param>
        public void OnApplicationFocus(bool iIsFocus)
        {
            if (iIsFocus) {
                if (!mIsInitialized) {
                    InitMic();
                    mIsInitialized = true;
                }
            }
            if (!iIsFocus) {
                StopMicrophone();
                mIsInitialized = false;
            }
        }

        /// <summary>
        /// Init microphone 
        /// </summary>
        public void InitMic()
        {
            if (mDevice == null)
                mDevice = Microphone.devices[0];
            mClipRecord = Microphone.Start(mDevice, true, 10, 44100); //999
        }

        /// <summary>
        /// Get data from microphone into audioclip
        /// </summary>
        /// <returns></returns>
        public float LevelMax()
        {
            float lLevelMax = 0;
            int lMicroPosition = Microphone.GetPosition(null) - (mSampleWindow + 1); // null means the first microphone
            if (lMicroPosition < 0)
                return 0;
            if (mClipRecord != null)
                mClipRecord.GetData(mWaveData, lMicroPosition);
            else
                InitMic();

            // Getting a peak on the last 128 samples
            for (int i = 0; i < mSampleWindow; ++i) {
                float lWave = mWaveData[i];
                float lWavePeak = lWave * lWave;
                if (lLevelMax < lWavePeak)
                    lLevelMax = lWavePeak;
            }
            return lLevelMax;
        }

        public void StopMicrophone()
        {
            Microphone.End(mDevice);
        }
    }
}