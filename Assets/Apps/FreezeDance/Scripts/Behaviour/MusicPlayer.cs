using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;


namespace BuddyApp.FreezeDance
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource speaker;

        [SerializeField]
        private AudioClip clip;

        [SerializeField]
        private AudioClip[] clips;

        [SerializeField]
        private Scrollbar slider;

        //[SerializeField]
        private Sprite pauseSprite;

        private float mElapsedTime;
        private float mAudioClipLength;
        private bool mIsStopped = false;

		public int NbClips { get { return clips.Length; } }

        // Use this for initialization
        void Start()
        {
            speaker.clip = clips[0];
            mElapsedTime = 0.0f;
            mIsStopped = false;
            mAudioClipLength = speaker.clip.length;
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("time1: " + mElapsedTime + " time2: " + mAudioClipLength);
            if(speaker.isPlaying)
            {
                mElapsedTime += Time.deltaTime;
                slider.size = (mElapsedTime / mAudioClipLength);// * slider.maxValue;
            }

            if(mElapsedTime> mAudioClipLength)
            {
                Debug.Log(" mElapsedTime> mAudioClipLength " + mElapsedTime + mAudioClipLength);
                mIsStopped = true;
                speaker.Stop();
            }
        }

        public void Play()
        {
            Debug.Log("play music");
            mElapsedTime += Time.deltaTime;
            speaker.Play();
        }

        /// <summary>
        /// Pause the music and show the pause icon
        /// </summary>
        /// <param name="iTitle">title that will be shown above the pause icon</param>
        public void Pause(string iTitle)
        {
            speaker.Pause();
            Buddy.GUI.Toaster.Display<IconToast>().With(Buddy.Resources.Get<Sprite>("os_icon_pause_big", Context.OS));
        }

        public bool IsPlaying()
        {
            return speaker.isPlaying;
        }

        public bool IsStopped()
        {
            return mIsStopped;
        }

        public void Restart()
        {
            Debug.Log("restart music");
            speaker.clip = null;
            speaker.clip = clips[0];
            mElapsedTime = 0.0f;
            mIsStopped = false;
            mAudioClipLength = speaker.clip.length;
            slider.size = 0;
        }

        public void ReinitMusic(int iMusicId)
        {
            Debug.Log("reinit music");
            if (iMusicId < clips.Length && iMusicId > 0)
            {
                clip = clips[iMusicId];
                speaker.clip = clips[iMusicId];
                //mElapsedTime = 0.0f;
                mIsStopped = false;
                mAudioClipLength = speaker.clip.length;
            }
        }
    }
}