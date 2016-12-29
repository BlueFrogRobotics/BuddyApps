using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class BabyPhonePlayLullAndAnim : MonoBehaviour
    {
        //[SerializeField]
        //private RawImage animation;

        [SerializeField]
        private GameObject progressBar;

        [SerializeField]
        private Animator babyPhoneAnimator;

        //private MovieTexture mMovie;

        private bool mIsVolumeOn;
        private Speaker mSpeaker;
        private AudioClip[] mLullabies;
        private List<string> mLullabyName;
        private int mLullIndice;
        private bool mIsLullabyPlaying;

        private float mFallingAssleepTime;
        private float mTimeElapsed;

        private bool isAnimatioOn;

        void Awake()
        {
            mSpeaker = BYOS.Instance.Speaker;
            mLullabies = Resources.LoadAll<AudioClip>("Sounds/Lullabies");
            mSpeaker.Media.Load("Sounds/Lullabies");
            mLullabyName = new List<string>();
            FillMusicName(mLullabyName, mLullabies);
        }
        void OnEnable()
        {    
            mLullIndice = (int)BabyPhoneData.Instance.LullabyToPlay;
            mFallingAssleepTime = ((BabyPhoneData.Instance.TimeBeforContact) * 60F); //convert from minutes to seconds

            mIsVolumeOn = BabyPhoneData.Instance.IsVolumeOn;
            isAnimatioOn = BabyPhoneData.Instance.IsAnimationOn;

            mSpeaker.Media.Volume = ((BabyPhoneData.Instance.LullabyVolume)/100F);

            if (mIsVolumeOn)
            {
                mSpeaker.Media.Play(mLullabies[mLullIndice]);
                mIsLullabyPlaying = true;
            }
            else
                mIsLullabyPlaying = false;
        }

        void OnDisable()
        {
            //mMovie = (MovieTexture)animation.mainTexture;
            //mMovie.loop = true;
            if (mIsLullabyPlaying)
                mSpeaker.Media.Stop();
                
            mIsLullabyPlaying = false;
            mTimeElapsed = 0;
        }

        void Start()
        {
            //mMovie = (MovieTexture)animation.mainTexture;
            //mMovie.loop = true;
            
            mSpeaker.Media.Loop = true;
            mTimeElapsed = 0;
        }

        void Update()
        {
            if ((babyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("FallingAssleep"))
                && (babyPhoneAnimator.GetBool("DoPlayLullaby")) && (mIsVolumeOn))
                PlayLullabyAndAnimations();


            #region Slider Udpate
            if (mIsLullabyPlaying) 
            {
                mTimeElapsed += Time.deltaTime;

                float lValueX = mTimeElapsed / mFallingAssleepTime;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);

                if (mTimeElapsed >= mFallingAssleepTime)
                    babyPhoneAnimator.SetTrigger("StartListening");
            }

        #endregion
    }

        private void PlayLullabyAndAnimations()
        {
            //if (!mMovie.isPlaying)
            //    mMovie.Play();

            if(!mIsLullabyPlaying)
                mSpeaker.Media.Play(mLullabies[mLullIndice]);
        }

        public void Return()
        {
            //mMovie.Play();

            mTimeElapsed = 0F;

            if(mIsVolumeOn)
                mSpeaker.Media.Play(mLullabies[mLullIndice]);
        }

        public void Replay()
        {
            if (mIsVolumeOn)
                mSpeaker.Media.Resume();

            mIsLullabyPlaying = true;

            babyPhoneAnimator.SetBool("DoPlayLullaby", true);
        }

        public void Pause()
        {
            //mSpeaker.Media.Stop() ;

            if (mIsVolumeOn)
                mSpeaker.Media.Pause();

            mIsLullabyPlaying = false;

            //if (mMovie.isPlaying)
            //    mMovie.Stop();

            babyPhoneAnimator.SetBool("DoPlayLullaby", false);
        }

        private void FillMusicName(List<string> iMusicName, AudioClip[] iAudioCLip)
        {
            for (int i = 0; i < iAudioCLip.Length; ++i)
                iMusicName.Add("Lullabies/" + iAudioCLip[i].name.ToString());
        }
    }
}
