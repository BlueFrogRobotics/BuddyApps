using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    //joue la musique et l'animation, appliquer le volume désiré, le temps de jeu 
    public class FallingAssleep : AStateMachineBehaviour
    {
        private BabyPhoneData mBabyPhoneData;

        private GameObject mFallingAssleep;
        private GameObject mWindoAppOverBlack;
        private GameObject mProgressBar;

        private Button mGoToParametersButton;
        private Button mQuitButton;

        private Button mPlayButton;
        private Button mReturnButton;
        private Button mPauseButton;

        private AudioClip[] mLullabies;
        private List<string> mLullabyName;
        private AudioSource mSource;
        private float mTimer;
        private float mTimeElapsed;
        private float mSongSize;

        private bool mDoExitApp;
        private bool mDoGoToParameters;

        public override void Init()
        {
            mBabyPhoneData = BabyPhoneData.Instance;

            mWindoAppOverBlack = GetGameObject(10);
            mFallingAssleep = GetGameObject(20);
            mProgressBar = GetGameObject(21);

            mQuitButton = GetGameObject(11).GetComponent<Button>();
            mGoToParametersButton = GetGameObject(19).GetComponent<Button>();

            mPlayButton = GetGameObject(16).GetComponent<Button>();
            mReturnButton = GetGameObject(17).GetComponent<Button>();
            mPauseButton = GetGameObject(18).GetComponent<Button>();

            mLullabies = Resources.LoadAll<AudioClip>("Lullabies");

            mSource = mFallingAssleep.GetComponent<AudioSource>();

            mQuitButton.onClick.AddListener(Quit);
            mGoToParametersButton.onClick.AddListener(GoToParameters);

            mReturnButton.onClick.AddListener(Return);
            mPauseButton.onClick.AddListener(Pause);
            mPlayButton.onClick.AddListener(Play);

            mDoExitApp = false;
            mDoGoToParameters = false;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mFallingAssleep.SetActive(true);
            mWindoAppOverBlack.SetActive(true);
            //mSource.volume = mBabyPhoneData.Volume;
            PlayLullabyAndAnimations();
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mFallingAssleep.SetActive(false);
            mWindoAppOverBlack.SetActive(false);

            if (mSource.isPlaying)
                mSource.Stop();

            iAnimator.SetBool("DoStartFallingAsleep", false);
            iAnimator.SetFloat("ForwardState", 3);

            //envoyer l'email d'information 

        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //if (mTimer >= mBabyPhoneData.TimeBeforSartListening) 
            //    iAnimator.SetBool("DoStartListening", true);

            if (mSource.isPlaying)
            {
                mSongSize = mSource.clip.length;
               
                mTimeElapsed += Time.deltaTime;
                float lValueX = mTimeElapsed / mSongSize;
                mProgressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);
            }
        }

        private void PlayLullabyAndAnimations()
        {
            if (!mSource.isPlaying)
                mSource.Play();
        }

        public void GoToParameters()
        {
            mDoGoToParameters = true;
            mDoExitApp = false;
        }
        public void Quit()
        {
            mDoExitApp = true;
            mDoGoToParameters = false;
        }

        public void Return()
        {
            mSource.Play();
            mTimeElapsed = 0F;
        }

        public void Play()
        {
            if (!mSource.isPlaying)
                mSource.Play();
        }

        public void Pause()
        {
            if (mSource.isPlaying)
                mSource.Stop();
        }

    }
}
