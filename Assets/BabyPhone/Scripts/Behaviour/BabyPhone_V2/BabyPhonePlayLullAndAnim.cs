using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class BabyPhonePlayLullAndAnim : MonoBehaviour
    {
        [SerializeField]
        private RawImage animation;
        [SerializeField]
        private GameObject progressBar;

        [SerializeField]
        private Animator babyPhoneAnimator;

		#if UNITY_ANDROID
			//Handheld.PlayFullScreenMovie("");
		#else
			private MovieTexture mMovie;
		#endif
        

        private BabyPhoneData mBabyPhoneData;

        private AudioClip[] mLullabies;
        private Speaker mSpeaker; 

        private List<string> mLullabyName;

        private float mFallingAssleepTime;
        private float mTimeElapsed;
        private float mSongSize;
        private int mLullIndice;
        private bool mIsPlaying;


        void Awake()
        {
            mLullabies = Resources.LoadAll<AudioClip>("Sounds");
            mLullabyName = new List<string>();
            FillMusicName(mLullabyName, mLullabies);
            Debug.Log((int)mBabyPhoneData.LullabyToPlay);
            //mLullIndice = (int)mBabyPhoneData.LullabyToPlay;
            //mIsPlaying = false;
            
        }
        void OnEnable()
        {
            //BYOS.Instance.SoundManager.AddSound(mLullabyName[mLullIndice]);
            //BYOS.Instance.SoundManager.Play(mLullabyName[mLullIndice]);
            mSpeaker.Media.Load(mLullabyName[1]);
            mSpeaker.Media.Play(mLullabyName[1]);
            Debug.Log("play music");
            //source.clip = mLullabies[(int)mBabyPhoneData.LullabyToPlay];

            //mSource.LoadSource(mLullabyName[2]);
            mBabyPhoneData = BabyPhoneData.Instance;
            mFallingAssleepTime = ((mBabyPhoneData.TimeBeforContact) * 60F); //convert from minutes to seconds
        }

        void OnDisable()
        {
			#if UNITY_ANDROID
				//Handheld.PlayFullScreenMovie("");				
			#else 
				mMovie = (MovieTexture)animation.mainTexture;
				mMovie.loop = true;
				//source.loop = true;
				//mTimeElapsed = 0;

				//if (source.isPlaying)
				//    source.Stop();

				////mSource.Stop();
				//mIsPlaying = false;
			#endif

        }

        void Start()
        {
			#if UNITY_ANDROID
				//Handheld.PlayFullScreenMovie("");
			#else
				mMovie = (MovieTexture)animation.mainTexture;
				mMovie.loop = true;
				//source.loop = true;
				//mSource.Loop = true;

				//mTimeElapsed = 0;
				//source.volume = ((mBabyPhoneData.LullabyVolume)/100F);
			#endif
        }

        void Update()
        {
            if ((babyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("FallingAssleep"))
                && (babyPhoneAnimator.GetBool("DoPlayLullaby")))
                PlayLullabyAndAnimations();

            //if (source.isPlaying)
			/*
            if(mMovie.isPlaying)
            {
                //mSongSize = source.clip.length;

                mTimeElapsed += Time.deltaTime;
                //float lValueX = mTimeElapsed / mSongSize;

                float lValueX = mTimeElapsed / mFallingAssleepTime;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);

                if (mTimeElapsed >= mFallingAssleepTime)
                    babyPhoneAnimator.SetTrigger("StartListening");
            }*/
        }

        private void PlayLullabyAndAnimations()
        {
            //if (!source.isPlaying)
            //    source.Play();

            //mSource.Play();
            //mIsPlaying = true;

            //if (!mMovie.isPlaying)
            //    mMovie.Play();

        }

        public void Return()
        {
            //source.Play();

            //mSource.Play();
            //mIsPlaying = true;
            //mMovie.Play();
            //mTimeElapsed = 0F;
        }

        public void Play()
        {
            babyPhoneAnimator.SetBool("DoPlayLullaby", true);
        }

        public void Pause()
        {
            //if (source.isPlaying)
            //    source.Stop();

            //mSource.Stop();
            //mIsPlaying = false;

           // if (mMovie.isPlaying)
           //     mMovie.Stop();
            babyPhoneAnimator.SetBool("DoPlayLullaby", false);
        }

        private void FillMusicName(List<string> iMusicName, AudioClip[] iAudioCLip)
        {
            //for (int i = 0; i < iAudioCLip.Length; ++i)
            //    iMusicName.Add(iAudioCLip[i].name.ToString());
        }
    }
}
