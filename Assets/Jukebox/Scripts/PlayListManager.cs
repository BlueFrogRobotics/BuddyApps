using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayListManager : MonoBehaviour
{

   
    private AudioClip[] mMyMusicsRock;
    private AudioClip[] mMyMusicsRap;
    private AudioClip[] mMyMusicsElectro;
    private AudioClip[] mMyMusics; 
    [SerializeField]
    private AudioSource mSource;
    private bool mIsRandom;
    private List<string> mLastMusicPlayed;

    private bool mIsMusicRock;
    private bool mIsMusicRap;
    private bool mIsMusicElectro;

    private float mVolume;

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

    private int mActualIndexMusic;

    private bool mIsNext;
    private bool mIsPrevious;

    /// <summary>
    /// UI
    /// </summary>
    [SerializeField] private GameObject mCanvas;
    [SerializeField] private Text mTypeFolder;
    [SerializeField] private Text mNumberMusicFolder;

    private enum mMusicType
    {
        Rap = 0,
        Electro = 1,
        Rock = 2,
        Default = Rap
    }

    void Awake()
    {
        //récupère les musiques de resources et les place dans des list définies de base
        mMyMusicsRap = Resources.LoadAll<AudioClip>("Music/Rap");
        mMyMusicsRock = Resources.LoadAll<AudioClip>("Music/Rock");
        mMyMusicsElectro = Resources.LoadAll<AudioClip>("Music/Electro");
        mMyMusics = Resources.LoadAll<AudioClip>("Music");
        mLastMusicPlayed = new List<string>();

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

        mActualIndexMusic = 0;

        mIsNext = false;
        mIsPrevious = false;

    }

    void Start()
    {
        mIsRandom = true;
        mIsMusicElectro = false;
        mIsMusicRock = false;
        mIsMusicRap = false;
        mVolume = 1f;
        mSource.volume = 0.1f;

        fillMusicName(mElectroMusicName, mMyMusicsElectro);
        fillMusicName(mRapMusicName, mMyMusicsRap);
        fillMusicName(mRockMusicName, mMyMusicsRock);
        fillMusicName(mMusicName, mMyMusics);

        mNumberRapMusic = mMyMusicsRap.Length;
        mNumberEletroMusic = mMyMusicsElectro.Length;
        mNumberRockMusic = mMyMusicsRock.Length;
        mNumberMusic = mMyMusics.Length;
    }


    void Update()
    {
        

        if(mIsRandom)
        {
            if(mIsMusicRap)
            {
               
            }
            else if(mIsMusicElectro)
            {
               
            }
            else if(mIsMusicRock)
            {
                
            }
            else
            {
                
            }
        }else if(!mIsRandom)
        {
            if (mIsMusicRap)
            {
                //PlayMusic(mMyMusicsRap);
            }
            else if (mIsMusicElectro)
            {
                //PlayMusic(mMyMusicsElectro);
            }
            else if (mIsMusicRock)
            {
                //PlayMusic(mMyMusicsRock);
            }
            else
            {
                //PlayMusic(mMyMusics);
            }
        }
    }

    void PlayMusic(AudioClip[] IAudioClip, int iIndex)
    {
        if(!mSource.isPlaying)
        {
            for (int i = 0; i < IAudioClip.Length - 1; ++i)
            {
                mSource.PlayOneShot(IAudioClip[Random.Range(0, IAudioClip.Length - 1)], mVolume);
            }
        }
    }

    void PlayRandomMusicArr(AudioClip[] iAudioClip, AudioClip[] iAudioClipRandom, int iIndex)
    {
        if(!mSource.isPlaying)
        if(iIndex == 0)
        {
            iAudioClipRandom = iAudioClip;
            ShuffleMusic<AudioClip>(iAudioClipRandom);
        }
        
        for(int i = iIndex; i < iAudioClipRandom.Length - 1; ++i)
        {
            mSource.PlayOneShot(iAudioClipRandom[i], this.mVolume);
            mActualIndexMusic = i;
        }
            
    }

    public static void ShuffleMusic<T>(T[] iArr)
    {
        for(int i = iArr.Length - 1; i > 0; --i)
        {
            int lRandom = Random.Range(0, i + 1);
            T lTmp = iArr[i];
            iArr[i] = iArr[lRandom];
            iArr[lRandom] = lTmp;
        }
    }

    public void UpVolume()
    {
        //use buddy api speaker
        mSource.volume += 0.05f;
    }

    public void DownVolume()
    {
        //use buddy api speaker
        mSource.volume -= 0.05f;
    }

    public void IsRandom()
    {
        mIsRandom = !mIsRandom;
    }

    public void NextMusic(AudioClip[] iAudioClip, AudioClip[] iAudioClipRandom)
    {
        mSource.Stop();
        if (mIsRandom)
            PlayRandomMusicArr(iAudioClip, iAudioClipRandom, mActualIndexMusic);
        else if (!mIsRandom)
            PlayMusic(iAudioClip, mActualIndexMusic);
    }

    private void fillMusicName(List<string> iMusicName, AudioClip[] iAudioCLip)
    {
        for (int i = 0; i < iAudioCLip.Length ; ++i)
            iMusicName.Add(iAudioCLip[i].ToString());
    }
}
