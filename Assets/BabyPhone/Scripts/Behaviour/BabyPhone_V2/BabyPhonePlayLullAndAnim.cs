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

        [SerializeField]
        private Animator cartoonAnimator;

        [SerializeField]
        private GameObject notifications;

        [SerializeField]
        private GameObject play;

        [SerializeField]
        private GameObject previous;

        [SerializeField]
        private GameObject pause;

        //private MovieTexture mMovie;

        private bool mIsVolumeOn;
        private Speaker mSpeaker;
        private AudioClip[] mLullabies;
        private List<string> mLullabyName;
        private int mLullIndice;
        private bool mIsLullabyPlaying;

        private float mFallingAssleepTime;
        private float mTimeElapsed;

        private bool mIsAnimationOn;

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
            mFallingAssleepTime = ((BabyPhoneData.Instance.TimeBeforContact) * 10F); //convert from minutes to seconds

            mIsVolumeOn = BabyPhoneData.Instance.IsVolumeOn;
            mIsAnimationOn = BabyPhoneData.Instance.IsAnimationOn;

            mSpeaker.Media.Volume = ((BabyPhoneData.Instance.LullabyVolume)/100F);

            if (mIsVolumeOn)
            {
                mSpeaker.Media.Play(mLullabies[mLullIndice]);
                mIsLullabyPlaying = true;
                play.SetActive(true);
                previous.SetActive(true);
                pause.SetActive(true);
            }
            else
                mIsLullabyPlaying = false;

            // si l'animation et la berceuse sont désactivée, passer directement à l'écoute 
            if((!mIsVolumeOn) && (!mIsAnimationOn))
                    babyPhoneAnimator.SetTrigger("StartListening");

            if (babyPhoneAnimator.GetInteger("CountNotifications") >= 1)
                notifications.SetActive(true);
        }

        void OnDisable()
        {
            if (mIsLullabyPlaying)
                mSpeaker.Media.Stop();
                
            mIsLullabyPlaying = false;
            mTimeElapsed = 0;
        }

        void Start()
        {           
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

            if((mIsAnimationOn) && (!mIsVolumeOn))
            {
                mTimeElapsed += Time.deltaTime;
                play.SetActive(false);
                previous.SetActive(false);
                pause.SetActive(false);
                float lValueX = mTimeElapsed / mFallingAssleepTime;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);

                if (mTimeElapsed >= mFallingAssleepTime)
                    babyPhoneAnimator.SetTrigger("StartListening");
            }


            #endregion
        }

        private void PlayLullabyAndAnimations()
        {
            if(!mIsLullabyPlaying)
                mSpeaker.Media.Play(mLullabies[mLullIndice]);
        }

        public void Return()
        {
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
            if (mIsVolumeOn)
                mSpeaker.Media.Pause();

            mIsLullabyPlaying = false;

            babyPhoneAnimator.SetBool("DoPlayLullaby", false);
        }

        private void FillMusicName(List<string> iMusicName, AudioClip[] iAudioCLip)
        {
            for (int i = 0; i < iAudioCLip.Length; ++i)
                iMusicName.Add("Lullabies/" + iAudioCLip[i].name.ToString());
        }
    }
}
