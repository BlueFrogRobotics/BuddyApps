using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

namespace BuddyApp.Jukebox
{
    public class MusicPlaylistManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource source;

        [SerializeField]
        private Slider slider;

        [SerializeField]
        private GameObject progressBar;

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private GameObject pause;

        [SerializeField]
        private GameObject play;

        [SerializeField]
        private Text textMusic;

        private AudioClip[] mMyMusicsRock;
        private AudioClip[] mMyMusicsRap;
        private AudioClip[] mMyMusicsElectro;
        private AudioClip[] mMyMusics;

        /// <summary>
        /// List of music's name by folder
        /// </summary>
        private List<string> mElectroMusicName;
        private List<string> mRapMusicName;
        private List<string> mRockMusicName;
        private List<string> mMusicName;

        private bool mIsRap;
        private bool mIsElectro;
        private bool mIsRock;
        private bool mIsMusic;

        private float mVolume;
        private bool mIsPlay;
        private bool mIsPaused;

        private int mIndexMusic;

        private int mNumberOfMusics;

        private bool mIsNext;
        private bool mIsPrevious;

        private Font mGothicFont;
        private Font[] mFonts;

        private float mTimeElapsed;
        private float mSongSize;

        private ActivePause mActivePause;

        ///// <summary>
        ///// TEST LOAD MUSIC
        ///// </summary>
        private string mCurrentPath;
        private List<string> mCurrentDirectoryFolderPath;
        private string mURLAndroid;
        [SerializeField]
        private Button playButton;
        private WWW[] www;

        /// <summary>
        /// TEST LOAD MUSIC ONE BY ONE
        /// </summary>
        private WWW mWWW;
        private AudioClip mMyOneMusic;
        FileInfo[] path;
        DirectoryInfo mDirectory;

        void Awake()
        {
            mMusicName = new List<string>();
            mIndexMusic = 0;
            mNumberOfMusics = 0;
            //récupère les musiques de resources et les place dans des list définies de base
            //mMyMusicsRap = Resources.LoadAll<AudioClip>("Music/Rap");
            //mMyMusicsRock = Resources.LoadAll<AudioClip>("Music/Rock");
            //mMyMusicsElectro = Resources.LoadAll<AudioClip>("Music/Electro");
            //mMyMusics = Resources.LoadAll<AudioClip>("Music");

            //TEST LOAD ALL MUSIC WITH CLASS WWW 
            mURLAndroid = "file://";
            mDirectory = new DirectoryInfo("/storage/emulated/0/Music");
            path = mDirectory.GetFiles("*.mp3");

            Debug.Log("PATH AWAKE : " + path[0]);
            //www = new WWW[path.Length];
            //for (int i = 0; i < path.Length; ++i)
            //{
            //    www[i] = new WWW(mURLAndroid + path[i]);
            //    //Debug.Log("PATH LENGTH : " + path.Length + "WWW LENGTH : " + www.Length + " kikoo : " + www[i].url);
            //}
            mWWW = new WWW(mURLAndroid + path[mIndexMusic]);
            //mMyMusics = new AudioClip[path.Length];
            //StartCoroutine(getMusic(www));
            StartCoroutine(getOneMusic(mWWW));
            //FIN TEST

            //mElectroMusicName = new List<string>();
            //mRapMusicName = new List<string>();
            //mRockMusicName = new List<string>();


            //changer font après
            //mGothicFont = Resources.GetBuiltinResource(typeof(Font), "Fonts/GOTHIC.TTF") as Font; ;
            mFonts = Resources.LoadAll<Font>("Fonts");
            mGothicFont = mFonts[1];
        }

        IEnumerator getOneMusic(WWW www)
        {
            yield return www;
            if(www.error == null && www != null)
            {
                mMyOneMusic = www.GetAudioClip(false, true, AudioType.MPEG);
            }
            else
                Debug.Log("WWW ERROR : " + www.error);
            FillMusicName2(mMusicName, path[mIndexMusic]);
        }

        IEnumerator getMusic(WWW[] www)
        {
            yield return www;
            for (int i = 0; i < www.Length; ++i)
            {
                //Debug.Log("COROUTINE : " + www[i].url + " WWW.LENGTH : " + www.Length);
                if (www[i].error == null && www[i] != null)
                {
                    mMyMusics[i] = www[i].GetAudioClip(false, true, AudioType.MPEG);
                    mMusicName.Add(www[i].url);
                }
                else
                {
                    Debug.Log("WWW ERROR : " + www[i].error);
                }
            }
            FillMusicName(mMusicName, mMyMusics);
            Debug.Log("after getmusic");
            yield return new WaitForSeconds(1.0f);

        }

        void Start()
        {
            //FillMusicName(mElectroMusicName, mMyMusicsElectro);
            //FillMusicName(mRapMusicName, mMyMusicsRap);
            //FillMusicName(mRockMusicName, mMyMusicsRock);
            
            mIsRap = false;
            mIsElectro = false;
            mIsRock = false;
            mIsMusic = true;

            mIsPlay = false;
            mIsPaused = false;
            mIsNext = false;

            source.volume = 0.8f;
        }

        void Update()
        {
            //Debug.Log(mIndexMusic);
            source.volume = slider.value;
            //if (mIsRap)
            //{

            //}
            //else if (mIsRock)
            //{

            //}
            //else if (mIsElectro)
            //{

            //}
            //else 
            if (mIsMusic && mNumberOfMusics < path.Length)
            {
                Debug.Log("1");
                for (int i = 0; i < path.Length; ++i)
                {
                    mNumberOfMusics++;
                }
            }

            //ResetIndexMusic();
            if (source.isPlaying)
            {
                mSongSize = source.clip.length;
                mTimeElapsed += Time.deltaTime;
                float lValueX = mTimeElapsed / mSongSize;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);
                if (lValueX > 1)
                    NextMusic();
            }

            if (mIsPaused && source.isPlaying)
            {
                source.Pause();
            }
            else if (!mIsPaused)
            {
                source.UnPause();
            }

            if (source.isPlaying && mIsPlay && mIsNext && !mIsPaused)
            {
                Debug.Log("3");
                mIsNext = false;
                mIsPrevious = false;
                ResetNextIndexMusic();
                source.clip = PlayMusic();
                //mSongSize = mSource.clip.length;
                source.Play();
               
            }

            if (source.isPlaying && mIsPlay && mIsPrevious && !mIsPaused)
            {
                Debug.Log("4");
                mIsNext = false;
                mIsPrevious = false;
                source.clip = PlayMusic();
                //mSongSize = mSource.clip.length;
                source.Play();
                ResetPreviousIndexMusic();
            }

            if (!source.isPlaying && mIsPlay && !mIsPaused)
            {
                    
                mIsNext = false;
                mIsPrevious = false;
                source.clip = PlayMusic();
                source.Play();
            }
            

        }

        public void IsPlaying()
        {
            //Debug.Log("startcoroutine");
            //StartCoroutine(Delay());
            textMusic.text = mMusicName[mIndexMusic];
            Debug.Log("startcoroutine : " + textMusic.text);
            mIsPlay = true;
            mIsPaused = false;
        }

        public void IsPaused()
        {
            mIsPlay = false;
            mIsPaused = true;
        }

        public void NextMusic()
        {
            Debug.Log("INDEX MUSIQUE : " + mIndexMusic);
            mIsNext = !mIsNext;
            if (mIsNext)
            {
                //Debug.Log("INDEX MUSIC NEXT " + mIndexMusic);
                ++mIndexMusic;
                Debug.Log("INDEX MUSIQUE 2  : " + mIndexMusic);
                ResetNextIndexMusic();
                Debug.Log("INDEX MUSIQUE 3 : " + mIndexMusic);
                mWWW = new WWW(mURLAndroid + path[mIndexMusic]);
                StartCoroutine(getOneMusic(mWWW));
                textMusic.text = mMusicName[mIndexMusic];

                mTimeElapsed = 0;
                
                mSongSize = mMyOneMusic.length;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                mIsNext = false;
                //mSongSize = mSource.clip.length;
            }
        }

        public void StopMusic()
        {
            if (pause.activeSelf)
            {
                pause.SetActive(false);
                play.SetActive(true);
            }
            source.Stop();
            textMusic.text = " ";
            mIsPlay = false;
            mIsPaused = true;
            mIndexMusic = 0;
            mTimeElapsed = 0;
            progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        }

        public void PreviousMusic()
        {
            mIsPrevious = !mIsPrevious;

            if (mIsPrevious)
            {
                --mIndexMusic;
                ResetPreviousIndexMusic();
                textMusic.text = mMusicName[mIndexMusic];
                mTimeElapsed = 0;
                mSongSize = mMyMusics[mIndexMusic].length;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            }
        }

        private void FillMusicName(List<string> iMusicName, AudioClip[] iAudioCLip)
        {
            for (int i = 0; i < iAudioCLip.Length; ++i)
                iMusicName.Add(iAudioCLip[i].name.ToString());
        }

        private void FillMusicName2(List<string> iMusicName, FileInfo path)
        {
            iMusicName.Add(path.FullName);
            Debug.Log("FILL NAME : " + path.FullName);
        }

        private AudioClip PlayMusic()
        {

            //if (mIsRap && mIndexMusic < mMyMusicsRap.Length)
            //{
            //    return mMyMusicsRap[mIndexMusic];
            //}
            //else if (mIsElectro && mIndexMusic < mMyMusicsElectro.Length)
            //{
            //    return mMyMusicsElectro[mIndexMusic];
            //}
            //else if (mIsRock && mIndexMusic < mMyMusicsRock.Length)
            //{
            //    return mMyMusicsRock[mIndexMusic];
            //}
            //else 
            if (mIsMusic && mIndexMusic < path.Length)
            {
                mWWW = new WWW(mURLAndroid + path[mIndexMusic]);
                StartCoroutine(getOneMusic(mWWW));
                return mMyOneMusic;
            }
            else
            {
                Debug.Log("6 null ");
                return null;
            }
                
        }
        
        private void ResetNextIndexMusic()
        {

            if (mIndexMusic == path.Length)
            {
                //Debug.Log(mMyMusics.Length + " " + mMyMusics[0].name.ToString());
                mIndexMusic = 0;
                mWWW = new WWW(mURLAndroid + path[mIndexMusic]);
                StartCoroutine(getOneMusic(mWWW));
            }
                
        }

        private void ResetPreviousIndexMusic()
        {
            Debug.Log("8");
            if (mIndexMusic < 0)
                mIndexMusic = mMyMusics.Length - 1;
        }
    }
}