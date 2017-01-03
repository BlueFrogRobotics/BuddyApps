using UnityEngine;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class BabyPhonePlayLullAndAnim : MonoBehaviour
    {
        /// <summary>
        /// the progress bar for to view the remaining state time
        /// </summary>
        [SerializeField]
        private GameObject progressBar;

        /// <summary>
        /// the global animator of the application
        /// </summary>
        [SerializeField]
        private Animator babyPhoneAnimator;

        /// <summary>
        /// the animator of 
        /// </summary>
        [SerializeField]
        private Animator cartoonAnimator;

        /// <summary>
        /// play button for lullabies
        /// </summary>
        [SerializeField]
        private GameObject play;

        /// <summary>
        /// previous button for lullabies
        /// </summary>
        [SerializeField]
        private GameObject previous;

        /// <summary>
        /// pause button for lullabies
        /// </summary>
        [SerializeField]
        private GameObject pause;

        /// <summary>
        /// speaker, for lullabies player
        /// </summary>
        private Speaker mSpeaker;

        private AudioClip[] mLullabies;

        /// <summary>
        /// indice of selected lullaby to play
        /// </summary>
        private int mLullIndice;

        /// <summary>
        /// selected time by the user for lullaby and animation playing
        /// </summary>
        private float mFallingAssleepTime;
        private float mTimeElapsed;

        /// <summary>
        /// is the animation is activated by the user ?
        /// </summary>
        private bool mIsAnimationOn;
        /// <summary>
        /// is the lullaby playing is activated by the user? 
        /// </summary>
        private bool mIsVolumeOn;
        private bool mIsLullabyPlaying;

        void Awake()
        {
            #region lullabies loading 
           
            mLullabies = Resources.LoadAll<AudioClip>("Sounds/Lullabies");

            mSpeaker = BYOS.Instance.Speaker;
            mSpeaker.Media.Load("Sounds/Lullabies");

            #endregion
        }

        void OnEnable()
        {
            #region collect user configuration

            mLullIndice = (int)BabyPhoneData.Instance.LullabyToPlay;

            // convert timebeforcontact from minutes to seconds 
            mFallingAssleepTime = ((BabyPhoneData.Instance.TimeBeforContact) * 60F); //convert from minutes to seconds

            // lullabyVolume [0-100], so we convert it to [0-1] for speaker needs
            mSpeaker.Media.Volume = ((BabyPhoneData.Instance.LullabyVolume) / 100F); 

            mIsVolumeOn = BabyPhoneData.Instance.IsVolumeOn;
            mIsAnimationOn = BabyPhoneData.Instance.IsAnimationOn;

            #endregion

            // if lullaby is enabled, so enable play/pause/previous buttons and start playing
            if (mIsVolumeOn)
            {
                mSpeaker.Media.Play(mLullabies[mLullIndice]);        
                mSpeaker.Media.Loop = true;

                mIsLullabyPlaying = true;

                // play, pause and previous buttons are enabled
                play.SetActive(true);
                previous.SetActive(true);
                pause.SetActive(true);
            }
            else
                mIsLullabyPlaying = false;

            // if lullaby and animation are disabled, go to listening state 
            if((!mIsVolumeOn) && (!mIsAnimationOn))
                    babyPhoneAnimator.SetTrigger("StartListening");

            mTimeElapsed = 0;
        }

        void OnDisable()
        {
            if (mIsLullabyPlaying)
                mSpeaker.Media.Stop();
                
            mIsLullabyPlaying = false;
            mTimeElapsed = 0;
        }

        void Update()
        {
            // sheck if we are in the correct state, and have correct configuration to play lullaby
            if ((babyPhoneAnimator.GetCurrentAnimatorStateInfo(0).IsName("FallingAssleep"))
                && (babyPhoneAnimator.GetBool("DoPlayLullaby")) && (mIsVolumeOn))
                PlayLullaby();

            #region progress bar Udpate

            if ( (mIsLullabyPlaying) || ((mIsAnimationOn) && (!mIsVolumeOn)))
            {
                mTimeElapsed += Time.deltaTime;

                float lValueX = mTimeElapsed / mFallingAssleepTime;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);

                if (mTimeElapsed >= mFallingAssleepTime)
                    babyPhoneAnimator.SetTrigger("StartListening");
            }                     

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        private void PlayLullaby()
        {
            if(!mIsLullabyPlaying)
                mSpeaker.Media.Play(mLullabies[mLullIndice]);
        }

        /// <summary>
        /// Play lullaby from the begin 
        /// </summary>
        public void Previous()
        {          
            if(mIsVolumeOn)
                mSpeaker.Media.Play(mLullabies[mLullIndice]);

            mTimeElapsed = 0F;
        }

        /// <summary>
        /// Play lullabies if pause button has already been pressed
        /// </summary>
        public void Replay()
        {
            if (mIsVolumeOn)
                mSpeaker.Media.Resume();

            mIsLullabyPlaying = true;

            babyPhoneAnimator.SetBool("DoPlayLullaby", true);
        }

        /// <summary>
        /// Pause lullaby 
        /// </summary>
        public void Pause()
        {
            if (mIsVolumeOn)
                mSpeaker.Media.Pause();

            mIsLullabyPlaying = false;

            babyPhoneAnimator.SetBool("DoPlayLullaby", false);
        }
    }
}
