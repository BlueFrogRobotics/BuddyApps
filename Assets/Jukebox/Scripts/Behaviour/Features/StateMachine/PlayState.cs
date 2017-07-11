using UnityEngine;
using System.Collections;
using Buddy;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public class PlayState : AStateMachineBehaviour
    {
        private AudioSource mMusicPlay;
        private float mTime;
        private float mSongLength;
        private float mDiff;
        private bool mIsMusicDone;

        private bool mFromPauseState;
        public bool FromPauseState { get { return mFromPauseState; } set { mFromPauseState = value; } }

        private bool mIsStop;
        public bool IsStop { get { return mIsStop; } set { mIsStop = value; } }

        public override void Start()
        {
            
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mIsStop = false;
            mIsMusicDone = false;
            if(!mFromPauseState)
            {
                mTime = 0.0F;
                mSongLength = 0.0F;
            }
            mMusicPlay = GetGameObject(0).GetComponent<AudioSource>();
            GetGameObject(1).GetComponent<Text>().text = iAnimator.GetBehaviour<LoadingMusicState>().MusicName;
            Debug.Log("ON ENTER¨PLAY STATE : " + iAnimator.GetBehaviour<LoadingMusicState>().MusicName);
            mMusicPlay.volume = 0.8F;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMusicPlay.volume = GetGameObject(5).GetComponent<Slider>().value;
            if (mMusicPlay.isPlaying)
            {
                mSongLength = mMusicPlay.clip.length;
                mTime += Time.deltaTime;
                mDiff = mTime / mSongLength;
                GetGameObject(4).GetComponent<RectTransform>().anchorMax = new Vector2(mDiff, 0);
                if (mDiff > 1)
                    mIsMusicDone = true;
            }
            
            if (!mMusicPlay.isPlaying)
            {
                mMusicPlay.clip = iAnimator.GetBehaviour<LoadingMusicState>().OneMusic;
                GetGameObject(2).SetActive(false);
                GetGameObject(3).SetActive(true);
                mMusicPlay.Play();
            }
            
            if (mIsMusicDone)
            {
                iAnimator.SetTrigger("Next");
                mIsMusicDone = false;
            }
            
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mFromPauseState = false;
        }
        
    }

}
