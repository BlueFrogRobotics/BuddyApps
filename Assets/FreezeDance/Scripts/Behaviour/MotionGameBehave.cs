using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

namespace BuddyApp.FreezeDance
{
    public class MotionGameBehave : MonoBehaviour
    {
        [SerializeField]
        private GameObject motionGame;

        [SerializeField]
        private AudioSource speaker;

        [SerializeField]
        private AudioClip music;

        [SerializeField]
        private GameObject defeat;

        [SerializeField]
        private GameObject victory;

        [SerializeField]
        private Animator defeatAnim;

        [SerializeField]
        private Animator victoryAnim;

        [SerializeField]
        private Animator restartAnim;

        [SerializeField]
        private GameObject pauseScreen;

        [SerializeField]
        private GameObject pauseScreenText;

        [SerializeField]
        private GameObject restartScreen;

        [SerializeField]
        private Image progressBar;

        private TextToSpeech mTTS;
        private Face mFace;
        private float mTime;

        private bool mIsMoving;
        private bool mIsOccupied;
        private bool mStartMusic;
        private bool mPauseMusic;
        private bool mIsSad;
        private bool mNeutral;
        private bool mIsOnGame;
        private bool mSayOnce;
        private bool mChrono;
        private float mAudioClipLength;
        private bool mIsSetRandomStop;
        private float mRandomStopDelay;
        private float mElapsedTime;

        // Use this for initialization
        void Awake()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mFace = BYOS.Instance.Face;
        }
        void Start()
        {
            //mFace.Neutral();
            speaker = gameObject.GetComponent<AudioSource>();
            mIsSad = false;
            mIsOnGame = false;
            mSayOnce = false;
            mChrono = true;
            mStartMusic = false;
            mIsSetRandomStop = false;
        }

        // Update is called once per frame
        void Update()
        {
            mIsMoving = motionGame.GetComponent<MotionGame>().IsMoving();
            if (mIsOnGame) {

                if (speaker.isPlaying) {
                    mElapsedTime += Time.deltaTime;
                    float valueX = mElapsedTime / mAudioClipLength;
                    progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(valueX, 0);
                }
                float lTime = Time.time;
                if (!mIsSetRandomStop)
                    mRandomStopDelay = Random.Range(10, 30);
                if (lTime - mTime > mRandomStopDelay)
                    RandomStop();
                if (!mStartMusic) {

                    if (mIsMoving && !mIsOccupied && mPauseMusic)
                        StartCoroutine(SetAngry());
                    if (!mIsMoving && !mIsOccupied && mPauseMusic) {
                        StartCoroutine(SetNeutral());
                        if (mChrono)
                            StartCoroutine(chrono());
                    }
                }
            }

            if (mElapsedTime > mAudioClipLength && !mIsSad && mStartMusic) {
                mIsOnGame = false;
                if (!mSayOnce) {
                    mFace.SetExpression(MoodType.HAPPY);
                    mTTS.Say("Bravo, Tu a gagné");
                    pauseScreenText.SetActive(false);
                    victoryAnim.SetBool("victory", true);
                    StartCoroutine(RestartYESNO());
                    victory.SetActive(true);
                    victoryAnim.SetTrigger("Open");
                    mSayOnce = true;
                }
            }

            if (mIsSad) {
                mIsOnGame = false;
                if (!mSayOnce) {
                    mTTS.Say("tu a perdu! dommage!");
                    pauseScreenText.SetActive(false);
                    defeat.SetActive(true);
                    defeatAnim.SetTrigger("Open");
                    StartCoroutine(RestartYESNO());
                    mSayOnce = true;
                }
            }
        }

        public void Restart()
        {
            mSayOnce = false;
            mChrono = true;
            speaker.clip = null;
            speaker.clip = music;
            mStartMusic = false;
            mIsSad = false;
            mIsOnGame = false;
            mIsSetRandomStop = false;
            mFace.SetExpression(MoodType.NEUTRAL);
            //mFace.Neutral();
            mTime = Time.time;
            pauseScreen.SetActive(false);
            restartScreen.SetActive(false);
            restartAnim.SetTrigger("Close_WQuestion");
            victory.SetActive(false);
            victoryAnim.SetTrigger("Close");
            defeat.SetActive(false);
            defeatAnim.SetTrigger("Close");
            StartMusic();
        }

        private IEnumerator SetAngry()
        {
            mIsOccupied = true;
            mFace.SetExpression(MoodType.SAD);
            mIsSad = true;
            yield return new WaitForSeconds(0.3F);
            mIsOccupied = false;
        }

        private IEnumerator SetNeutral()
        {
            mIsOccupied = true;
            mFace.SetExpression(MoodType.NEUTRAL);
            yield return new WaitForSeconds(0.3F);
            mIsOccupied = false;
        }

        private IEnumerator SetFocus()
        {
            mIsOccupied = true;
            mFace.SetExpression(MoodType.THINKING);
            mTTS.Say("Pourquoi tu ne bouge pas?");
            yield return new WaitForSeconds(2F);
            mIsOccupied = false;
        }

        public void StartMusic()
        {
            if (speaker.clip == null)
                speaker.clip = music;
            mAudioClipLength = speaker.clip.length;
            speaker.Play();
            mTime = Time.time;
            mStartMusic = true;
            mPauseMusic = false;
            mIsOnGame = true;
            mElapsedTime = 0F;
        }

        public void RandomStop()
        {
            mTime = Time.time;
            speaker.Pause();
            if (!pauseScreen.activeSelf)
                pauseScreen.SetActive(true);
            if (!pauseScreenText.activeSelf)
                pauseScreenText.SetActive(true);

            StartCoroutine(DelayAfterRandomStop());
        }

        public void RelaunchMusic()
        {
            if (!mIsSad) {
                mPauseMusic = false;
                mChrono = true;
                mStartMusic = true;
                if (pauseScreen.activeSelf)
                    pauseScreen.SetActive(false);
                speaker.UnPause();
            }
        }

        public void Quit()
        {
            new BuddyOS.Command.HomeCmd().Execute();
        }

        private IEnumerator DelayAfterRandomStop()
        {
            yield return new WaitForSeconds(1.5f);
            mPauseMusic = true;
            mStartMusic = false;
        }

        private IEnumerator chrono()
        {
            mChrono = false;
            yield return new WaitForSeconds(5f);
            RelaunchMusic();
        }

        private IEnumerator RestartYESNO()
        {
            yield return new WaitForSeconds(5f);
            if (!restartScreen.activeSelf)
                restartScreen.SetActive(true);
            restartAnim.SetTrigger("Open_WQuestion");
        }

        private IEnumerator LaunchDefeatAnim()
        {
            yield return new WaitForSeconds(1F);
            defeat.SetActive(true);
            defeatAnim.SetTrigger("Open");
        }

        private IEnumerator LaunchVictoryAnim()
        {
            yield return new WaitForSeconds(1F);
            victory.SetActive(true);
            victoryAnim.SetTrigger("Open");
        }
    }
}