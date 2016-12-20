using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.BabyPhone
{
    public class PlayLullabyAndAnim : MonoBehaviour
    {
        [SerializeField]
        private RawImage mAnimation;
        [SerializeField]
        private GameObject mProgressBar;
        [SerializeField]
        private AudioSource mSource;
        [SerializeField]
        private Animator mBabyPhoneAnimator;

        private MovieTexture mMovie; 
            
        private BabyPhoneData mBabyPhoneData;

        private AudioClip[] mLullabies;
        private List<string> mLullabyName;

        private float mTimer;
        private float mTimeElapsed;
        private float mSongSize;

        void OnEnable()
        {

        }

        void OnDisable()
        {
            if (mSource.isPlaying)
                mSource.Stop();
        }

        void Start()
        {
            mLullabies = Resources.LoadAll<AudioClip>("Lullabies");
            //mMovie = (MovieTexture)mAnimation.mainTexture;
            ((MovieTexture)mAnimation.mainTexture).Play();
            mTimeElapsed = 0;
            //mSource.volume = mBabyPhoneData.Volume;
        }

        void Update()
        {
            if ((mBabyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("FallingAssleep")) 
                && (mBabyPhoneAnimator.GetBool("DoPlayLullaby")))
                PlayLullabyAndAnimations();

            if (mSource.isPlaying)
            {
                mSongSize = mSource.clip.length;

                mTimeElapsed += Time.deltaTime;
                float lValueX = mTimeElapsed / mSongSize;
                mProgressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);


                if (mTimeElapsed >= mSongSize)
                    mBabyPhoneAnimator.SetTrigger("StartListening");
            }
        }

        private void PlayLullabyAndAnimations()
        {
            if (!mSource.isPlaying)
                mSource.Play();

            //if (!mMovie.isPlaying)
                //mMovie.Play();

        }

        public void Return()
        {
            mSource.Play();
            //mMovie.Play();
            mTimeElapsed = 0F;
        }

        public void Play()
        {
            mBabyPhoneAnimator.SetBool("DoPlayLullaby", true);
        }

        public void Pause()
        {           
            if (mSource.isPlaying)
                mSource.Stop();

            //if (mMovie.isPlaying)
            //    mMovie.Stop();
            mBabyPhoneAnimator.SetBool("DoPlayLullaby", false);
        }
    }
}
