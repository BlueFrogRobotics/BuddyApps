using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

public class MotionGameBehave : MonoBehaviour {
    //[SerializeField]
   //BuddyFeature.Face.Regular.FaceManager mFace;
    [SerializeField]
    private GameObject mMotionGame;
    [SerializeField]
    private AudioSource mSpeaker;
    float mTime;
    private TextToSpeech mTTS;
    private Face mFace;
    [SerializeField]
    private AudioClip mMusic;
    [SerializeField]
    private GameObject mDefeat;
    [SerializeField]
    private GameObject mVictory;
    [SerializeField]
    private Animator mDefeatAnim;
    [SerializeField]
    private Animator mVictoryAnim;
    [SerializeField]
    private GameObject mPauseScreen;
    [SerializeField]
    private GameObject mPauseScreenText;
    [SerializeField]
    private GameObject mRestartScreen;
    [SerializeField]
    private Image ProgressBar;

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
    private float mHolyTime;
    private bool mIsSetRandomStop;
    private float mRandomStopDelay;
    private float mFullSongTime;
    private float mElapsedTime;
    // Use this for initialization
    void Awake()
    {
        mTTS = BYOS.Instance.TextToSpeech;
        mFace = BYOS.Instance.Face;
    }
    void Start ()
    {
        //mFace.Neutral();
        mSpeaker = gameObject.GetComponent<AudioSource>();
        mIsSad = false;
        mIsOnGame = false;
        mSayOnce = false;
        mChrono = true;
        mStartMusic = false;
        mIsSetRandomStop = false;
	}
	public void Restart()
    {
        mSayOnce = false;
        mChrono = true;
        mSpeaker.clip = null;
        mSpeaker.clip = mMusic;
        mStartMusic = false;
        mIsSad = false;
        mIsOnGame = false;
        mIsSetRandomStop = false;
        mFace.SetMood(FaceMood.NEUTRAL);
        //mFace.Neutral();
        mTime = Time.time;
        mPauseScreen.SetActive(false);
        mRestartScreen.SetActive(false);
        mVictory.SetActive(false);
        mDefeat.SetActive(false);
        StartMusic();
    }
	// Update is called once per frame
	void Update ()
    {
        float lHolyTime = Time.time;
        mIsMoving = mMotionGame.GetComponent<MotionGame>().IsMoving();
        if (mIsOnGame)
        {
            if (mSpeaker.isPlaying)
            {
                //mElapsedTime = Time.time - mHolyTime;
                mElapsedTime += Time.deltaTime;
                float valueX = mElapsedTime / mAudioClipLength;
                ProgressBar.GetComponent<RectTransform>().anchorMax = new Vector2(valueX, 0);
            }
            float lTime = Time.time;
            if (!mIsSetRandomStop)
                mRandomStopDelay = Random.Range(10, 30);
            if (lTime - mTime >mRandomStopDelay)
                RandomStop();
            if (!mStartMusic)
            {

                if (mIsMoving && !mIsOccupied && mPauseMusic)
                    StartCoroutine(SetAngry());
                if (!mIsMoving && !mIsOccupied && mPauseMusic)
                {
                    StartCoroutine(SetNeutral());
                    if (mChrono)
                        StartCoroutine(chrono());
                }
            }
        }

        if (mElapsedTime > mAudioClipLength&&!mIsSad&&mStartMusic)
        {
            mIsOnGame = false;
            if (!mSayOnce)
            {
                mFace.SetMood(FaceMood.HAPPY);
                mTTS.Say("Bravo, Tu a gagné");
                mPauseScreenText.SetActive(false);
                mVictoryAnim.SetBool("victory", true);
                StartCoroutine(RestartYESNO());
                mVictory.SetActive(true);
                mSayOnce = true;
            }
        }
        if (mIsSad)
        {
            mIsOnGame = false;
            if (!mSayOnce)
            {
                mTTS.Say("tu a perdu! dommage!");
                mPauseScreenText.SetActive(false);
                mDefeat.SetActive(true);
                StartCoroutine(RestartYESNO());
                mSayOnce = true;
            }
        }
    }
    IEnumerator SetAngry()
    {
        mIsOccupied = true;
        mFace.SetMood(FaceMood.SAD);
        mIsSad = true;
        yield return new WaitForSeconds(.3f);
        mIsOccupied = false;
    }
    IEnumerator SetNeutral()
    {
        mIsOccupied = true;
        mFace.SetMood(FaceMood.NEUTRAL);
        yield return new WaitForSeconds(.3f);
        mIsOccupied = false;
    }
    IEnumerator SetFocus()
    {
        mIsOccupied = true;
        mFace.SetMood(FaceMood.FOCUS);
        mTTS.Say("Pourquoi tu ne bouge pas?");
        yield return new WaitForSeconds(2f);
        mIsOccupied = false;
    }
    public void StartMusic()
    {
        if(mSpeaker.clip==null)
            mSpeaker.clip = mMusic;
        mAudioClipLength = mSpeaker.clip.length;
        mSpeaker.Play();
        mHolyTime = Time.time;
        mTime = Time.time;
        mStartMusic = true;
        mPauseMusic = false;
        mIsOnGame = true;
        mFullSongTime = mSpeaker.clip.length;
        mElapsedTime = 0;
    }
    public void RandomStop()
    {
        mTime = Time.time;
        mSpeaker.Pause();
        if (!mPauseScreen.activeSelf)
            mPauseScreen.SetActive(true);
        if (!mPauseScreenText.activeSelf)
            mPauseScreenText.SetActive(true);

        StartCoroutine(DelayAfterRandomStop());
    }
    public void RelaunchMusic()
    {
        if(!mIsSad)
        {
            mPauseMusic = false;
            mChrono = true;
            mStartMusic = true;
            if (mPauseScreen.activeSelf)
                mPauseScreen.SetActive(false);
            mSpeaker.UnPause();
            float lTime = Time.time;
            //mAudioClipLength += (lTime - mTime);
            //mElapsedTime-=(lTime-mTime);
        }

    }
    IEnumerator DelayAfterRandomStop()
    {
        yield return new WaitForSeconds(1.5f);
        mPauseMusic = true;
        mStartMusic = false;
    }
    IEnumerator chrono()
    {
        mChrono = false;
        //Debug.Log("here");
        yield return new WaitForSeconds(5f);
        RelaunchMusic();
    }
    IEnumerator RestartYESNO()
    {
        yield return new WaitForSeconds(5f);
        if (!mRestartScreen.activeSelf)
            mRestartScreen.SetActive(true);
    }
    IEnumerator LaunchDefeatAnim()
    {
        
        yield return new WaitForSeconds(1f);
        mDefeat.SetActive(true);
    }
    IEnumerator LaunchVictoryAnim()
    {
        yield return new WaitForSeconds(1f);
        mVictory.SetActive(true);
    }

}
