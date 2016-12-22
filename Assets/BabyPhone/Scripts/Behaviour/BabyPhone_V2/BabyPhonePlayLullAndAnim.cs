using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BuddyApp.BabyPhone
{
    public class BabyPhonePlayLullAndAnim : MonoBehaviour
    {
        [SerializeField]
        private RawImage animation;
        [SerializeField]
        private GameObject progressBar;
        [SerializeField]
        private AudioSource source;
        [SerializeField]
        private Animator babyPhoneAnimator;

        //private MovieTexture mMovie; 
            
        private BabyPhoneData mBabyPhoneData;

        private AudioClip[] mLullabies;

        private List<string> mLullabyName;

        private float mFallingAssleepTime;
        private float mTimeElapsed;
        private float mSongSize;


        void Awake()
        {
            mLullabies = Resources.LoadAll<AudioClip>("Sounds");
        }
        void OnEnable()
        {
            source.clip = mLullabies[(int)mBabyPhoneData.LullabyToPlay];
            mFallingAssleepTime = 60F;
        }

        void OnDisable()
        {
            
            //mMovie = (MovieTexture)mAnimation.mainTexture;
            //mMovie.loop = true;
            source.loop = true;
            mTimeElapsed = 0;

            if (source.isPlaying)
                source.Stop();
        }

        void Start()
        {
            mLullabies = Resources.LoadAll<AudioClip>("Lullabies");
            //mMovie = (MovieTexture)mAnimation.mainTexture;
            //mMovie.loop = true;
            source.loop = true;
            mTimeElapsed = 0;
            //mSource.volume = mBabyPhoneData.Volume;
        }

        void Update()
        {
            if ((babyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("FallingAssleep")) 
                && (babyPhoneAnimator.GetBool("DoPlayLullaby")))
                PlayLullabyAndAnimations();

            if (source.isPlaying)
            {
                mSongSize = source.clip.length;

                mTimeElapsed += Time.deltaTime;
                //float lValueX = mTimeElapsed / mSongSize;
                
                float lValueX = mTimeElapsed / mFallingAssleepTime;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);

                if (mTimeElapsed >= mFallingAssleepTime)
                    babyPhoneAnimator.SetTrigger("StartListening");
            }
        }

        private void PlayLullabyAndAnimations()
        {
            if (!source.isPlaying)
                source.Play();

            //if (!mMovie.isPlaying)
            //    mMovie.Play();

        }

        public void Return()
        {
            source.Play();
            //mMovie.Play();
            mTimeElapsed = 0F;
        }

        public void Play()
        {
            babyPhoneAnimator.SetBool("DoPlayLullaby", true);
        }

        public void Pause()
        {           
            if (source.isPlaying)
                source.Stop();

            //if (mMovie.isPlaying)
            //    mMovie.Stop();
            babyPhoneAnimator.SetBool("DoPlayLullaby", false);
        }
    }
}
