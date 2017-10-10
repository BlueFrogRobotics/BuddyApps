﻿using System.Collections;
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
            FILES_SAVED=4,
            MAIL_SENDING=5

        }

        public Action OnFilesSaved;

        private int mFPS = 20;

        private int mNbSecBefore = 3;
        private int mNbSecAfter = 12;

        private RGBCam mCam;

        private State mState;

        private Queue<byte[]> mListFrame;
        private Queue<AudioClip> mListAudio;

        private bool mNewFrame = true;
        private AndroidJavaObject currentActivity;
        
        private NoiseDetection mNoiseDetection;
        private MotionDetection mMotionDetection;
        private AudioClip mAudioClip;
        private float mTime = 0.0f;

        private EMail mMail;

        // Use this for initialization
        void Start()
        {
            mMotionDetection = BYOS.Instance.Perception.Motion;
            mNoiseDetection = BYOS.Instance.Perception.Noise;
            mState = State.DEFAULT;
            mListFrame = new Queue<byte[]>();
            mListAudio = new Queue<AudioClip>();
            mCam = BYOS.Instance.Primitive.RGBCam;
            mTime = 0.0f;
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            mNewFrame = true;
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("sample: " + mNoiseDetection.GetMicPosition());
            switch(mState)
            {
                case State.DEFAULT:
                    Debug.Log("etat lol 1");
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
                case State.MAIL_SENDING:
                    Debug.Log("etat lol 6");
                    SendingMail();
                    break;
                default:
                    Debug.Log("etat lol 7");
                    FillCircularBuffer();
                    break;
            }
        }

        public void Save(EMail iMail)
        {
            if (mState == State.DEFAULT)
            {
                mMail = iMail;
                mState = State.ASKED;
                mTime = 0.0f;
            }
        }

        public void VideoSaved(string message)
        {
            Debug.Log("message: "+message);
            mListFrame.Clear();
            mListAudio.Clear();
            mState = State.FILES_SAVED;
            currentActivity.Call("clearPicture");
            mNewFrame = true;
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
                Debug.Log("1 buffer");
                if(mNoiseDetection.GetMicPosition() < 122000)
                    mNewFrame = true;
                else if ( mNewFrame && mNoiseDetection.GetMicPosition() >122000 && mNoiseDetection.GetAudioClip().length>2 && mNoiseDetection.GetMicData()!=null)
                {
                    Debug.Log("2 buffer");
                    mNewFrame = false;
                    AudioClip lAudioClip = AudioClip.Create(mNoiseDetection.GetAudioClip().name, mNoiseDetection.GetAudioClip().samples, mNoiseDetection.GetAudioClip().channels, mNoiseDetection.GetAudioClip().frequency, false, false);
                    float[] samples = new float[mNoiseDetection.GetAudioClip().samples * mNoiseDetection.GetAudioClip().channels];
                    mNoiseDetection.GetAudioClip().GetData(samples, 0);
                    lAudioClip.SetData(samples, 0);
                    mListAudio.Enqueue(lAudioClip); 
                    if (mListAudio.Count>1)
                        mListAudio.Dequeue();
                }
            }
        }

        private void FillNextBuffer()
        {
            if (mCam.IsOpen)
            {
                mListFrame.Enqueue(mCam.FrameTexture2D.EncodeToPNG());
                //if (mListFrame.Count > mFPS * (mNbSecBefore + mNbSecAfter))
                 //   mState = State.BUFFER_FILLED;
            }
            
            mTime += Time.deltaTime;

            if (mNoiseDetection.enabled)
            {
                if (mNoiseDetection.GetMicPosition() < 122000)
                    mNewFrame = true;
                if ( mNewFrame && mNoiseDetection.GetMicPosition() > 122000 && mNoiseDetection.GetAudioClip().length > 2 && mNoiseDetection.GetMicData()!=null)
                {
                    mNewFrame = false;
                    AudioClip lAudioClip = AudioClip.Create(mNoiseDetection.GetAudioClip().name, mNoiseDetection.GetAudioClip().samples, mNoiseDetection.GetAudioClip().channels, mNoiseDetection.GetAudioClip().frequency, false, false);
                    float[] samples = new float[mNoiseDetection.GetAudioClip().samples * mNoiseDetection.GetAudioClip().channels];
                    mNoiseDetection.GetAudioClip().GetData(samples, 0);
                    lAudioClip.SetData(samples, 0);
                    mListAudio.Enqueue(lAudioClip);
                    if (mListAudio.Count > 4)
                        mListAudio.Dequeue();
                }
            }
            if (mTime > mNbSecAfter)
                mState = State.BUFFER_FILLED;
        }

        private void SaveFiles()
        {
            byte[][] lArrayFrames = mListFrame.ToArray();
            
            
            string lDirectoryPath = Path.GetDirectoryName(BYOS.Instance.Resources.PathToRaw("monitoring.mp4"));
            Directory.CreateDirectory(lDirectoryPath);
            //Utils.Save(BYOS.Instance.Resources.PathToRaw("son.wav"), mAudioClip);
            for (int i = 0; i < lArrayFrames.Length; i++)
            {
                currentActivity.Call("addPicture", lArrayFrames[i]);
            }
            Debug.Log("fini call");
            mFPS = mListFrame.Count / (mNbSecAfter+mNbSecBefore);
            currentActivity.Call("saveVideo", mFPS, BYOS.Instance.Resources.PathToRaw("monitoring.mp4"), "AIBehaviour", "VideoSaved");
            Debug.Log("apres call: "+ mListAudio.Count);
            Utils.Save(BYOS.Instance.Resources.PathToRaw("audio.wav"), Utils.Combine(mListAudio.ToArray()));
            Debug.Log("apres audio");
            //currentActivity.Call("saveVideo", mFPS, BYOS.Instance.Resources.PathToRaw("monitoring.mp4"), lArray, lArrayFrames.Length, maxLength, "AIBehaviour", "VideoSaved");
            mState = State.WAIT_SAVE;
        }

        private void WaitForSave()
        {
            //mState = State.FILES_SAVED;
        }

        private void SendingMail()
        {

        }

        private void SavesComplete()
        {
            BYOS.Instance.WebService.EMailSender.enabled = true;
            mMail.AddFile(BYOS.Instance.Resources.PathToRaw("monitoring.mp4"));
            mMail.AddFile(BYOS.Instance.Resources.PathToRaw("audio.wav"));
            BYOS.Instance.WebService.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, mMail, OnMailSent);
            BYOS.Instance.WebService.EMailSender.enabled = true;
            if (OnFilesSaved != null)
                OnFilesSaved();
            mState = State.MAIL_SENDING;
        }

        private void OnMailSent()
        {
            mState = State.DEFAULT;
            BYOS.Instance.WebService.EMailSender.enabled = false;
            mMail = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}