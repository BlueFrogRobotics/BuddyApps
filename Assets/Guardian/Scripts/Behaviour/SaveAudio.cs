using UnityEngine;
using System.Collections;
using BuddyFeature.Media;

namespace BuddyApp.Guardian
{
    public class SaveAudio : MonoBehaviour
    {

        private BuddyFeature.Detection.SoundDetector mSoundDetector;
        private AudioClip[] mArrayAudioClip = new AudioClip[3];
        private AudioClip mAudioClip;
        private int mPosMicroActuel = 0;
        private string mDevice;
        private float[] mDatabefore;
        private bool mIsRecording = false;
        private int mNumSeq = 0;
        public bool CanSave { get; set; }
        private bool mIsInit = false;

        // Use this for initialization
        void Start()
        {
            mSoundDetector = GetComponent<BuddyFeature.Detection.SoundDetector>();
        }

        // Update is called once per frame
        void Update()
        {
            if (mSoundDetector.HasStarted && !mIsInit)
            {
                Init();
                mIsInit = true;
            }
            else
                RecordInArray();
 
        }

        void Init()
        {
            
            mAudioClip = mSoundDetector.Clip;
            mDevice = Microphone.devices[0];
            for (int i = 0; i < mArrayAudioClip.Length; i++)
            {
                mArrayAudioClip[i] = AudioClip.Create("noise " + i, mAudioClip.samples, mAudioClip.channels, 44100, false);
            }
            mDatabefore = new float[mAudioClip.samples * mAudioClip.channels];
        }

        private void RecordInArray()
        {
            if (mSoundDetector.IsASoundDetected)
            {

                mIsRecording = true;
                mPosMicroActuel = Microphone.GetPosition(mDevice);
                mNumSeq = 0;
            }

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
                mAudioClip.GetData(mDatabefore, 0);
                mArrayAudioClip[2].SetData(mDatabefore, 0);

                if (mIsRecording)
                {
                    mNumSeq++;
                    if (mNumSeq > 1)
                    {
                        if (CanSave)
                        {
                            CanSave = false;
                            Debug.Log("avant can save: " + CanSave);
                            //AudioRecorder.Save("noise.wav", AudioRecorder.Combine(mArrayAudioClip));
                            //SaveWav("noise.wav");

                        }
                        mIsRecording = false;
                        mNumSeq = 0;
                        //mSoundSaved = true;
                    }
                }
            }
        }


        public void Save()
        {
            CanSave = true;
            AudioRecorder.Save("noise.wav", AudioRecorder.Combine(mArrayAudioClip));
        }
    }
}