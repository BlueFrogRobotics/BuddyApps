using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEditor;

public class MusicPlaylistManager : MonoBehaviour {

    private AudioClip[] mMyMusicsRock;
    private AudioClip[] mMyMusicsRap;
    private AudioClip[] mMyMusicsElectro;
    private AudioClip[] mMyMusics;

    [SerializeField]
    private AudioSource mSource;

    [SerializeField]
    private Slider mSlider;

    [SerializeField]
    private GameObject mProgressBar;

    private GameObject test;

    /// <summary>
    /// List of music's name by folder
    /// </summary>
    private List<string> mElectroMusicName;
    private List<string> mRapMusicName;
    private List<string> mRockMusicName;
    private List<string> mMusicName;

    private List<AudioClip> mRandomMusicRap;
    private List<AudioClip> mRandomMusicRock;
    private List<AudioClip> mRandomMusicElectro;
    private List<AudioClip> mRandomMusic;

    private int mNumberRapMusic;
    private int mNumberEletroMusic;
    private int mNumberRockMusic;
    private int mNumberMusic;

    private bool mIsRap;
    private bool mIsElectro;
    private bool mIsRock;
    private bool mIsMusic;

    private bool mIsRandom;

    private float mVolume;
    private bool mIsPlay;
    private bool mIsPaused;

    private int mIndexMusic;

    private int mNumberOfMusics;

    private bool mIsNext;
    private bool mIsPrevious;
    
    private Font mArialFont;
    private Font mGothicFont;
    private Font[] mFonts;

    private float mTimeElapsed;
    private float mTime;
    private float mSongSize;

    [SerializeField]
    private Text mTextMusic;

    private ActivePause mActivePause;

    [SerializeField]
    Canvas mCanvas;

    [SerializeField]
    private GameObject mPause;

    [SerializeField]
    private GameObject mPlay;

    void Awake()
    {
        
        //récupère les musiques de resources et les place dans des list définies de base
        mMyMusicsRap = Resources.LoadAll<AudioClip>("Music/Rap");
        mMyMusicsRock = Resources.LoadAll<AudioClip>("Music/Rock");
        mMyMusicsElectro = Resources.LoadAll<AudioClip>("Music/Electro");
        mMyMusics = Resources.LoadAll<AudioClip>("Music");

        //LOAD ALL MUSIC WITH CLASS WWW 

        mRandomMusic = new List<AudioClip>();
        mRandomMusicElectro = new List<AudioClip>();
        mRandomMusicRap = new List<AudioClip>();
        mRandomMusicRock = new List<AudioClip>();

        mElectroMusicName = new List<string>();
        mRapMusicName = new List<string>();
        mRockMusicName = new List<string>();
        mMusicName = new List<string>();

        mNumberRapMusic = 0;
        mNumberEletroMusic = 0;
        mNumberRockMusic = 0;
        mNumberMusic = 0;

        mIndexMusic = 0;

        mNumberOfMusics = 0;

        //changer font après
        mArialFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        //mGothicFont = Resources.GetBuiltinResource(typeof(Font), "Fonts/GOTHIC.TTF") as Font; ;
        mFonts = Resources.LoadAll<Font>("Fonts");
        mGothicFont = mFonts[1];
    }

    void Start ()
    {
        fillMusicName(mElectroMusicName, mMyMusicsElectro);
        fillMusicName(mRapMusicName, mMyMusicsRap);
        fillMusicName(mRockMusicName, mMyMusicsRock);
        fillMusicName(mMusicName, mMyMusics);

        mNumberRapMusic = mMyMusicsRap.Length;
        mNumberEletroMusic = mMyMusicsElectro.Length;
        mNumberRockMusic = mMyMusicsRock.Length;
        mNumberMusic = mMyMusics.Length;

        mIsRap = false;
        mIsElectro = false;
        mIsRock = false;
        mIsMusic = true;
        
        mIsRandom = false;
        mIsPlay = false;
        mIsPaused = false;
        mIsNext = false;

        mSource.volume = 0.1f;
        
    }
	
	void Update ()
    {
        //Debug.Log(mIndexMusic);
        mSource.volume = mSlider.value;
        if(mIsRap)
        {
           
        }
        else if (mIsRock)
        {

        }
        else if (mIsElectro)
        {

        }
        else if (mIsMusic && mNumberOfMusics < mMyMusics.Length)
        {
            for(int i = 0; i < mMyMusics.Length ; ++i)
            {
                //CreateText(mCanvas.transform, -50.0f, 150.0f+(i * -25.0f), mMusicName[i], 14, Color.black);
                mNumberOfMusics++;
            }
        }

        //ResetIndexMusic();
        if(mSource.isPlaying)
        {
            mSongSize = mSource.clip.length;
            //mTimeElapsed = Time.time - mTime;
            mTimeElapsed +=Time.deltaTime;
            float lValueX = mTimeElapsed / mSongSize;
            mProgressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);
            if (lValueX > 1)
                NextMusic();

        }

        if (mIsPaused && mSource.isPlaying)
        {
            mSource.Pause();
        }
        else if (!mIsPaused)
        {
            mSource.UnPause();
        }

        if(mSource.isPlaying && mIsPlay && mIsNext && !mIsPaused)
        {
            mIsNext = false;
            mIsPrevious = false;
            mSource.clip = PlayMusic();
            //mSongSize = mSource.clip.length;
            mSource.Play();
            ResetNextIndexMusic();
            
        }


       if (mSource.isPlaying && mIsPlay && mIsPrevious && !mIsPaused)
        {
            mIsNext = false;
            mIsPrevious = false;
            mSource.clip = PlayMusic();
            //mSongSize = mSource.clip.length;
            mSource.Play();
            
            ResetPreviousIndexMusic();
        }

        if (!mSource.isPlaying && mIsPlay && !mIsPaused)
        {
            mIsNext = false;
            mIsPrevious = false;
            mSource.clip = PlayMusic();
            mSource.Play();


        }
    }

    private void fillMusicName(List<string> iMusicName, AudioClip[] iAudioCLip)
    {
        for (int i = 0; i < iAudioCLip.Length; ++i)
            iMusicName.Add(iAudioCLip[i].name.ToString());
    }


    AudioClip PlayMusic()
    {
        if (mIsRap && mIndexMusic < mMyMusicsRap.Length)
        {
            return mMyMusicsRap[mIndexMusic];
        }
        else if (mIsElectro && mIndexMusic < mMyMusicsElectro.Length)
        {
            return mMyMusicsElectro[mIndexMusic];
        }
        else if (mIsRock && mIndexMusic < mMyMusicsRock.Length)
        {
            return mMyMusicsRock[mIndexMusic];
        }
        else if (mIsMusic && mIndexMusic < mMyMusics.Length)
        {
            return mMyMusics[mIndexMusic];
        }
        else
            return null;
    }

    public void IsPlaying()
    {
        mTextMusic.text = mMusicName[mIndexMusic];
        mTime = Time.time;
        mIsPlay = true;
        mIsPaused = false;
    }

    public void IsPaused()
    {
        mIsPlay = false;
        mIsPaused = true;
    }

    GameObject CreateText(Transform iCanvasTransform, float x, float y, string iTextToPrint, int iFontSize, Color iTextColor)
    {
        GameObject lUITextGo = new GameObject(iTextToPrint);
        lUITextGo.transform.SetParent(iCanvasTransform);

        RectTransform trans = lUITextGo.AddComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(x, y);
        trans.sizeDelta = new Vector2(500, 100);
        

        Text text = lUITextGo.AddComponent<Text>();
        text.text = iTextToPrint;
        text.fontSize = iFontSize;
        text.color = iTextColor;
        //text.font = mArialFont;
        text.font = mGothicFont;

        return lUITextGo;
    }

    public void NextMusic()
    {
        mIsNext = !mIsNext;
        if(mIsNext)
        {
            ++mIndexMusic;
            ResetNextIndexMusic();
            mTextMusic.text = mMusicName[mIndexMusic];
            mTimeElapsed = 0;
            mSongSize = mMyMusics[mIndexMusic].length;
            mProgressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            //mSongSize = mSource.clip.length;
        }
    }

    public void StopMusic()
    {
        if (mPause.activeSelf)
        {
            mPause.SetActive(false);
            mPlay.SetActive(true);
        }
        mSource.Stop();
        mTextMusic.text = " ";
        mIsPlay = false;
        mIsPaused = true;
        mIndexMusic = 0;
        mTimeElapsed = 0;
        mProgressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
    }

    public void PreviousMusic()
    {
        mIsPrevious = !mIsPrevious;
       
        if (mIsPrevious)
        {
            --mIndexMusic;
            ResetPreviousIndexMusic();
            mTextMusic.text = mMusicName[mIndexMusic];
            mTimeElapsed = 0;
            mSongSize = mMyMusics[mIndexMusic].length;
            mProgressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        }
    }

    private void ResetNextIndexMusic()
    {
        if (mIndexMusic == mMyMusics.Length )
            mIndexMusic = 0;
        //else if (mIndexMusic > mMyMusicsRap.Length)
        //    mIndexMusic = 0;
        //else if (mIndexMusic > mMyMusicsElectro.Length)
        //    mIndexMusic = 0;
        //else if (mIndexMusic > mMyMusicsRock.Length)
        //    mIndexMusic = 0;
    }

    private void ResetPreviousIndexMusic()
    {
        if (mIndexMusic < 0)
            mIndexMusic = mMyMusics.Length - 1;
    }


}
