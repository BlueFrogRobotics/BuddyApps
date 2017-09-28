using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Buddy;
using System.IO;

namespace BuddyApp.Guardian
{

    public class MediaManager : MonoBehaviour
    {
        private enum State : int
        {
            DEFAULT=0,
            ASKED=1,
            BUFFER_FILLED=2,
            WAIT_SAVE=3,
            FILES_SAVED=4

        }

        public Action OnFilesSaved;

        private int mFPS = 20;

        private int mNbSecBefore = 2;
        private int mNbSecAfter = 10;

        private RGBCam mCam;

        private State mState;

        private Queue<byte[]> mListFrame;
        private bool mSaving = false;
        private bool mSaved = false;
        private AndroidJavaObject currentActivity;
        
        private NoiseDetection mNoiseDetection;
        private MotionDetection mMotionDetection;
        private AudioClip mAudioClip;
        private int micPosition = 0;
        private float mTime = 0.0f;

        // Use this for initialization
        void Start()
        {
            mMotionDetection = BYOS.Instance.Perception.Motion;
            mNoiseDetection = BYOS.Instance.Perception.Noise;
            mAudioClip = AudioClip.Create("sound", 12*44100, 1, 44100, false);

            mState = State.DEFAULT;
            mListFrame = new Queue<byte[]>();
            mSaved = false;
            mSaving = false;
            mCam = BYOS.Instance.Primitive.RGBCam;
            mTime = 0.0f;
            //AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            // if (!mCam.IsOpen)
            //    mCam.Open(RGBCamResolution.W_320_H_240);
        }

        // Update is called once per frame
        void Update()
        {
            switch(mState)
            {
                case State.DEFAULT:
                    //Debug.Log("etat lol 1");
                    FillCircularBuffer();
                    break;
                case State.ASKED:
                    Debug.Log("etat lol 2");
                    FillNextBuffer();
                    break;
                case State.BUFFER_FILLED:
                    Debug.Log("etat lol 3");
                    SaveFiles();
                    break;
                case State.WAIT_SAVE:
                    Debug.Log("etat lol 4");
                    WaitForSave();
                    break;
                case State.FILES_SAVED:
                    Debug.Log("etat lol 5");
                    SavesComplete();
                    break;
                default:
                    Debug.Log("etat lol 6");
                    FillCircularBuffer();
                    break;
            }
        }

        public void Save()
        {
            if (mState == State.DEFAULT)
            {
                mState = State.ASKED;
                mTime = 0.0f;
            }
        }

        public void VideoSaved(string message)
        {
            Debug.Log(message);
            mListFrame.Clear();
            mState = State.FILES_SAVED;
            //BYOS.Instance.Header.SpinningWheel = false;
        }

        private void FillCircularBuffer()
        {
            if (mCam.IsOpen)
            {
                mListFrame.Enqueue(mCam.FrameTexture2D.EncodeToPNG());
                if (mListFrame.Count > mFPS * mNbSecBefore)
                    mListFrame.Dequeue();
            }
            if (mNoiseDetection.enabled)
            {
                mAudioClip.SetData(mNoiseDetection.GetMicData(), mNoiseDetection.GetMicPosition());
                if(micPosition> mNoiseDetection.GetMicPosition())
                    Debug.Log("mic position: " + mNoiseDetection.GetMicPosition());
                micPosition = mNoiseDetection.GetMicPosition();
            }
        }

        private void FillNextBuffer()
        {
            if (mCam.IsOpen)
            {
                mListFrame.Enqueue(mCam.FrameTexture2D.EncodeToPNG());
                if (mListFrame.Count > mFPS * (mNbSecBefore + mNbSecAfter))
                    mState = State.BUFFER_FILLED;
            }
            
            mTime += Time.deltaTime;
            int mMulti =  (int)(mTime / 3) + 1;
            if (mNoiseDetection.enabled)
                mAudioClip.SetData(mNoiseDetection.GetMicData(), mNoiseDetection.GetMicPosition()+ 44100 * mMulti);
            if (mTime > 10.0f)
                mState = State.BUFFER_FILLED;
        }

        private void SaveFiles()
        {
            byte[][] lArrayFrames = mListFrame.ToArray();
            
            
            string lDirectoryPath = Path.GetDirectoryName(BYOS.Instance.Resources.PathToRaw("monitoring.mp4"));
            Directory.CreateDirectory(lDirectoryPath);
            Utils.Save(BYOS.Instance.Resources.PathToRaw("son.wav"), mAudioClip);
            //for (int i=0; i< lArrayFrames.Length; i++)
            //{
            //    currentActivity.Call("addPicture", lArrayFrames[i]);
            //}
            Debug.Log("fini call");
            //currentActivity.Call("saveVideo", mFPS, BYOS.Instance.Resources.PathToRaw("monitoring.mp4"), "AIBehaviour", "VideoSaved");
            //currentActivity.Call("saveVideo", mFPS, BYOS.Instance.Resources.PathToRaw("monitoring.mp4"), lArray, lArrayFrames.Length, maxLength, "AIBehaviour", "VideoSaved");
            mState = State.WAIT_SAVE;
        }

        private void WaitForSave()
        {
            mState = State.FILES_SAVED;
        }

        private void SavesComplete()
        {
            BYOS.Instance.WebService.EMailSender.enabled = true;
            EMail lMail = new EMail("Guardian logs", "video truc");
            lMail.AddTo("tigrejounin@gmail.com");
            //lMail.AddFile(BYOS.Instance.Resources.PathToRaw("monitoring.mp4"));
            lMail.AddFile(BYOS.Instance.Resources.PathToRaw("son.wav"));
            BYOS.Instance.WebService.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail);
            BYOS.Instance.WebService.EMailSender.enabled = true;
            if (OnFilesSaved != null)
                OnFilesSaved();
            mState = State.DEFAULT;
        }
    }
}