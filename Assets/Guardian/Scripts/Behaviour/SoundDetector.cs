using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

namespace BuddyApp.Guardian
{
    public class SoundDetector : MonoBehaviour, IDetector
    {

        public event Action OnDetection;

        private bool mIsASoundDetected;
        private AudioClip mClipBefore;
        private AudioClip mClipMedium;
        private AudioClip mClipRecord = new AudioClip();
        private string mDevice;

        private Queue myQ = new Queue();
        private float mGlobalMean;

        private bool mIsInit = false;

        private float mMinThreshold = 0.03f;
        private float mMaxThreshold = 0.3f;
        private float mThreshold = 0.1f;

        private int mPosMicroActuel = 0;

        private int mNumSeq = 0;
        private float[] mDatabefore;
        private float[] mDatamedium;
        private bool mIsRecording = false;
        private AudioClip[] mArrayAudioClip = new AudioClip[3];
        private bool mSoundSaved = false;
        private float mActValue = 0.0f;

        public bool SoundSaved
        {
            get
            {
                bool lResult = mSoundSaved;
                mSoundSaved = false;
                return lResult;
            }
        }


        public bool IsInit { get { return mIsInit; } }
        public float Value { get { return mActValue; } }
        public bool IsASoundDetected { get { return mIsASoundDetected; } }


        // Use this for initialization
        void Start()
        {
            //Init();
            mMinThreshold = 0.03f;
            mMaxThreshold = 0.5f;
            mThreshold = 0.1f;
        }




        // Update is called once per frame
        void Update()
        {
            if (mIsInit)
            {
                // here we aquire the microphone sound,
                // we do the mean of the value of the stack (filtering)
                float lSoundLevelReceived = GetMicValues();

                // we put it in a FIFO stack
                myQ.Enqueue(lSoundLevelReceived);
                if (myQ.Count < 100) return;
                if (myQ.Count > 100) myQ.Dequeue();

                object[] lTempStack = myQ.ToArray();
                float lGlobalSum = 0;
                for (int i = 0; i < myQ.Count; i++)
                {
                    lGlobalSum += (float)lTempStack[i];
                }
                mGlobalMean = lGlobalSum / myQ.Count;

                mActValue = Mathf.Abs(lSoundLevelReceived - mGlobalMean);
                if (mActValue > mThreshold)
                {
                    mIsASoundDetected = true;
                    Debug.Log("detection bruit");
                    if (OnDetection != null)
                        OnDetection();

                    mIsRecording = true;
                    mPosMicroActuel = Microphone.GetPosition(mDevice);
                    mNumSeq = 0;
                }
                else
                {
                    //Debug.Log("position micro: " + Microphone.GetPosition(_device));
                    mIsASoundDetected = false;
                    if (mPosMicroActuel <= Microphone.GetPosition(mDevice))
                    {
                        mPosMicroActuel = Microphone.GetPosition(mDevice);
                    }
                    else
                    {
                        mPosMicroActuel = Microphone.GetPosition(mDevice);
                        for (int i = 0; i < mArrayAudioClip.Length - 1; i++)
                        {
                            mArrayAudioClip[i + 1].GetData(mDatabefore, 0);
                            mArrayAudioClip[i].SetData(mDatabefore, 0);
                        }
                        mClipRecord.GetData(mDatabefore, 0);
                        mArrayAudioClip[2].SetData(mDatabefore, 0);


                        if (mIsRecording)
                        {
                            mNumSeq++;
                            if (mNumSeq > 1)
                            {
                                SaveWav("noise.wav");
                                mIsRecording = false;
                                mNumSeq = 0;
                                mSoundSaved = true;
                            }
                        }
                        Debug.Log("num: " + mNumSeq);
                    }
                }
                // we compare that to a bigger mean (compare a value to a global mean value)
                // if there is a relative difference, this mean there is a sound
            }
        }

        public void Init()
        {
            mIsASoundDetected = false;
            if (mDevice == null) mDevice = Microphone.devices[0];
            mClipRecord = Microphone.Start(mDevice, true, 5, 44100);
            mGlobalMean = 0;
            mIsInit = true;
            mDatabefore = new float[mClipRecord.samples * mClipRecord.channels];
            mDatamedium = new float[mClipRecord.samples * mClipRecord.channels];
            for (int i = 0; i < mArrayAudioClip.Length; i++)
            {
                mArrayAudioClip[i] = AudioClip.Create("noise " + i, mClipRecord.samples, mClipRecord.channels, 44100, false);
            }

        }

        public void Stop()
        {
            Microphone.End(mDevice);
            mIsASoundDetected = false;
            mIsInit = false;
        }



        public void SaveWav(string iFileName)
        {
            SavWav.Save(iFileName, Combine(mArrayAudioClip));
        }



        public static AudioClip Combine(params AudioClip[] iClips)
        {
            if (iClips == null || iClips.Length == 0)
                return null;

            int lLength = 0;
            for (int i = 0; i < iClips.Length; i++)
            {
                lLength += iClips[i].samples * iClips[i].channels;
            }

            float[] lData = new float[lLength];
            lLength = 0;
            for (int i = 0; i < iClips.Length; i++)
            {

                float[] lBuffer = new float[iClips[i].samples * iClips[i].channels];
                iClips[i].GetData(lBuffer, 0);
                lBuffer.CopyTo(lData, lLength);
                lLength += lBuffer.Length;
            }

            if (lLength == 0)
                return null;

            AudioClip lResult = AudioClip.Create("Combine", lLength, 1, 44100, false, false);
            lResult.SetData(lData, 0);

            return lResult;
        }



        public float GetMinThreshold()
        {
            return mMinThreshold;
        }

        public float GetMaxThreshold()
        {
            return mMaxThreshold;
        }

        public float GetThreshold()
        {
            return mThreshold;
        }

        public void SetThreshold(float iThreshold)
        {
            if (iThreshold < mMinThreshold)
                mThreshold = mMinThreshold;
            else if (iThreshold > mMaxThreshold)
                mThreshold = mMaxThreshold;
            else
                mThreshold = iThreshold;

        }

        private float GetMicValues()
        {
            int lSampleSizeWindow = 128;
            float[] lWaveData = new float[lSampleSizeWindow];
            int lMicPosition = Microphone.GetPosition(null) - (lSampleSizeWindow + 1); // null means the first microphone
            if (lMicPosition < 0) return 0; // ????? Probleme ????

            mClipRecord.GetData(lWaveData, lMicPosition);
            // Getting a peak on the last 128 samples
            float lSumWave = 0;
            for (int i = 0; i < lSampleSizeWindow; i++)
            {
                float lWavePeak = Mathf.Abs(lWaveData[i]);
                lSumWave += lWavePeak;
            }
            float lFiltredWave = lSumWave / lSampleSizeWindow;

            return lFiltredWave;
        }
    }
}