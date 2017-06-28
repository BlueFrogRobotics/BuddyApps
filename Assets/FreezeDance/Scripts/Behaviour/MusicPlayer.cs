using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource speaker;

        [SerializeField]
        private AudioClip clip;

        [SerializeField]
        private Slider slider;

        //[SerializeField]
        private Sprite pauseSprite;

        private float mElapsedTime;
        private float mAudioClipLength;
        private bool mIsStopped = false;

        // Use this for initialization
        void Start()
        {
            speaker.clip = clip;
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
                slider.value = (mElapsedTime / mAudioClipLength) * slider.maxValue;
            }

            if(mElapsedTime> mAudioClipLength)
            {
                mIsStopped = true;
                speaker.Stop();
            }
        }

        public void Play()
        {
            mElapsedTime += Time.deltaTime;
            speaker.Play();
        }

        public void Pause()
        {
            speaker.Pause();
            pauseSprite = BYOS.Instance.Resources.GetSprite("Pause");
            BYOS.Instance.Toaster.Display<IconToast>().With("bouge pas!!!", pauseSprite, true);
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
            speaker.clip = null;
            speaker.clip = clip;
            mElapsedTime = 0.0f;
            mIsStopped = false;
            mAudioClipLength = speaker.clip.length;
            slider.value = 0;
        }
    }
}