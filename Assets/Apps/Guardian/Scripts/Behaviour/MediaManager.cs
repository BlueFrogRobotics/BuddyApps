using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Buddy;
using System.IO;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// Class used to save continuously video and audio and sent them by mail when an alert occurs
    /// </summary>
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

        /// <summary>
        /// number of seconds of video and audio before an alert
        /// </summary>
        private int mNbSecBefore = 3;

        /// <summary>
        /// number of seconds of video and audio after an alert
        /// </summary>
        private int mNbSecAfter = 9;

        private RGBCam mCam;

        private State mState;

        private Queue<byte[]> mListFrame;
        private Queue<AudioClip> mListAudio;

        private bool mNewFrame = true;
        private AndroidJavaObject currentActivity;
        
        private NoiseDetection mNoiseDetection;
        private float mTime = 0.0f;

        private EMail mMail;

        // Use this for initialization
        void Start()
        {
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
            switch(mState)
            {
                case State.DEFAULT:
                    FillCircularBuffer();
                    break;
                case State.ASKED:
                    FillNextBuffer();
                    break;
                case State.BUFFER_FILLED:
                    SaveFiles();
                    break;
                case State.WAIT_SAVE:
                    WaitForSave();
                    break;
                case State.FILES_SAVED:
                    StartSendmail();
                    break;
                case State.MAIL_SENDING:
                    SendingMail();
                    break;
                default:
                    FillCircularBuffer();
                    break;
            }
        }

        /// <summary>
        /// Start the recording of the video and audio. When the recording ends, it will the send the mail with the files attached.
        /// </summary>
        /// <param name="iMail">The mail that will be sent with the audio and video files attached</param>
        public void Save(EMail iMail)
        {
            if (mState == State.DEFAULT)
            {
                mMail = iMail;
                mState = State.ASKED;
                mTime = 0.0f;
            }
        }

        /// <summary>
        /// Function that will be called by the android plugin when the video encoding is over
        /// </summary>
        /// <param name="message">the message sent from android plugin</param>
        public void VideoSaved(string message)
        {
            Debug.Log("message: "+message);
            mListFrame.Clear();
            mListAudio.Clear();
            mState = State.FILES_SAVED;
            currentActivity.Call("clearPicture");
            mNewFrame = true;
        }

        /// <summary>
        /// Fills video and audio buffer continuously when save function has not been called
        /// </summary>
        private void FillCircularBuffer()
        {
            if (mCam.IsOpen)
            {
                mListFrame.Enqueue(mCam.FrameTexture2D.EncodeToPNG());
                if (mListFrame.Count > mFPS * mNbSecBefore)
                    mListFrame.Dequeue();
            }
            FillAudioBuffer(1);

        }

        /// <summary>
        /// Fills the buffer until a certain amount of time has been passed
        /// </summary>
        private void FillNextBuffer()
        {
            if (mCam.IsOpen)
            {
                mListFrame.Enqueue(mCam.FrameTexture2D.EncodeToPNG());
            }
            
            mTime += Time.deltaTime;
            FillAudioBuffer(4);

            if (mTime > mNbSecAfter)
                mState = State.BUFFER_FILLED;
        }

        /// <summary>
        /// Fills the audio clip queue with a number of audioclip. Each audioclip is set to 3 seconds.
        /// </summary>
        /// <param name="iNbOfClipToKeep">the number of audioclip to add to the queue</param>
        private void FillAudioBuffer(int iNbOfClipToKeep)
        {
            if (mNoiseDetection.enabled)
            {
                if (mNoiseDetection.GetMicPosition() < 122000)
                    mNewFrame = true;
                if (mNewFrame && mNoiseDetection.GetMicPosition() > 122000 && mNoiseDetection.GetAudioClip().length > 2 && mNoiseDetection.GetMicData() != null)
                {
                    mNewFrame = false;
                    AudioClip lAudioClip = AudioClip.Create(mNoiseDetection.GetAudioClip().name, mNoiseDetection.GetAudioClip().samples, mNoiseDetection.GetAudioClip().channels, mNoiseDetection.GetAudioClip().frequency, false);
                    float[] samples = new float[mNoiseDetection.GetAudioClip().samples * mNoiseDetection.GetAudioClip().channels];
                    mNoiseDetection.GetAudioClip().GetData(samples, 0);
                    lAudioClip.SetData(samples, 0);
                    mListAudio.Enqueue(lAudioClip);
                    if (mListAudio.Count > iNbOfClipToKeep)
                        mListAudio.Dequeue();
                }
            }
        }

        /// <summary>
        /// Saves the audio and video buffers respectively in a wav and a mp4 file
        /// </summary>
        private void SaveFiles()
        {
            byte[][] lArrayFrames = mListFrame.ToArray();
             
            string lDirectoryPath = Path.GetDirectoryName(BYOS.Instance.Resources.PathToRaw("monitoring.mp4"));
            Directory.CreateDirectory(lDirectoryPath);

            for (int i = 0; i < lArrayFrames.Length; i++)
            {
                currentActivity.Call("addPicture", lArrayFrames[i]);
            }
            mFPS = mListFrame.Count / (mNbSecAfter+mNbSecBefore);
            currentActivity.Call("saveVideo", mFPS, BYOS.Instance.Resources.PathToRaw("monitoring.mp4"), "AIBehaviour", "VideoSaved");
            Utils.Save(BYOS.Instance.Resources.PathToRaw("audio.wav"), Utils.Combine(mListAudio.ToArray()));
            mState = State.WAIT_SAVE;
        }

        /// <summary>
        /// Called in update when the video is being encoded
        /// </summary>
        private void WaitForSave()
        {
            //mState = State.FILES_SAVED;
        }

        /// <summary>
        /// Called in update when the mail is being sent
        /// </summary>
        private void SendingMail()
        {

        }

        /// <summary>
        /// Function that attached the audio and video files and attached them to the eamil before sending it
        /// </summary>
        private void StartSendmail()
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

        /// <summary>
        /// Function that will be called when the email has beent sent
        /// </summary>
        private void OnMailSent()
        {
            mState = State.DEFAULT;
            BYOS.Instance.WebService.EMailSender.enabled = false;
            mMail = null;
        }
    }
}