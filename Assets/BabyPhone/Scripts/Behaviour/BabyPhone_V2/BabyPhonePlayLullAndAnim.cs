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

        //private BabyPhoneData mBabyPhoneData;

        private AudioClip[] mLullabies;

        private List<string> mLullabyName;

        //private float mFallingAssleepTime;
        //private float mTimeElapsed;
        //private float mSongSize;
        private int mLullIndice;
        private bool mIsPlaying;


        void Awake()
        {
            mLullabies = Resources.LoadAll<AudioClip>("Sounds");
            mLullabyName = new List<string>();
            FillMusicName(mLullabyName, mLullabies);                    
        }
        void OnEnable()
        {
            //mBabyPhoneData = BabyPhoneData.Instance;
            //BYOS.Instance.SoundManager.AddSound(mLullabyName[mLullIndice]);
            //BYOS.Instance.SoundManager.Play(mLullabyName[mLullIndice]);
            //mLullIndice = (int)mBabyPhoneData.LullabyToPlay;


            //mFallingAssleepTime = ((mBabyPhoneData.TimeBeforContact) * 60F); //convert from minutes to seconds
        }

        void OnDisable()
        {
            //mMovie = (MovieTexture)animation.mainTexture;
            //mMovie.loop = true;

            //mTimeElapsed = 0;


        }

        void Start()
        {
            //mMovie = (MovieTexture)animation.mainTexture;
            //mMovie.loop = true;

            //mTimeElapsed = 0;
        }

        void Update()
        {
            //if ((babyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("FallingAssleep"))
            //    && (babyPhoneAnimator.GetBool("DoPlayLullaby")))
            //    PlayLullabyAndAnimations();

            //if(mMovie.isPlaying)
            //{
            //    mTimeElapsed += Time.deltaTime;

            //    float lValueX = mTimeElapsed / mFallingAssleepTime;
            //    progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);

            //    if (mTimeElapsed >= mFallingAssleepTime)
            //        babyPhoneAnimator.SetTrigger("StartListening");
            //}
        }

        private void PlayLullabyAndAnimations()
        {
            //if (!mMovie.isPlaying)
            //    mMovie.Play();

        }

        public void Return()
        {
            //mMovie.Play();
            //mTimeElapsed = 0F;
        }

        public void Play()
        {
            //babyPhoneAnimator.SetBool("DoPlayLullaby", true);
        }

        public void Pause()
        {

            //if (mMovie.isPlaying)
            //    mMovie.Stop();
            //babyPhoneAnimator.SetBool("DoPlayLullaby", false);
        }

        private void FillMusicName(List<string> iMusicName, AudioClip[] iAudioCLip)
        {
            for (int i = 0; i < iAudioCLip.Length; ++i)
                iMusicName.Add(iAudioCLip[i].name.ToString());
        }
    }
}
