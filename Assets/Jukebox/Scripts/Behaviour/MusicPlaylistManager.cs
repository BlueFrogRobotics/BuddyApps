using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

        void Awake()
        {
            //récupère les musiques de resources et les place dans des list définies de base
            mMyMusicsRap = Resources.LoadAll<AudioClip>("Music/Rap");
            mMyMusicsRock = Resources.LoadAll<AudioClip>("Music/Rock");
            mMyMusicsElectro = Resources.LoadAll<AudioClip>("Music/Electro");
            mMyMusics = Resources.LoadAll<AudioClip>("Music");

            //LOAD ALL MUSIC WITH CLASS WWW 

            mElectroMusicName = new List<string>();
            mRapMusicName = new List<string>();
            mRockMusicName = new List<string>();
            mMusicName = new List<string>();

            mIndexMusic = 0;

            mNumberOfMusics = 0;

            //changer font après
            //mGothicFont = Resources.GetBuiltinResource(typeof(Font), "Fonts/GOTHIC.TTF") as Font; ;
            mFonts = Resources.LoadAll<Font>("Fonts");
            mGothicFont = mFonts[1];
        }

        void Start()
        {
            FillMusicName(mElectroMusicName, mMyMusicsElectro);
            FillMusicName(mRapMusicName, mMyMusicsRap);
            FillMusicName(mRockMusicName, mMyMusicsRock);
            FillMusicName(mMusicName, mMyMusics);

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
            if (mIsRap) {

            } else if (mIsRock) {

            } else if (mIsElectro) {

            } else if (mIsMusic && mNumberOfMusics < mMyMusics.Length) {
                for (int i = 0; i < mMyMusics.Length; ++i) {
                    mNumberOfMusics++;
                }
            }

            //ResetIndexMusic();
            if (source.isPlaying) {
                mSongSize = source.clip.length;
                mTimeElapsed += Time.deltaTime;
                float lValueX = mTimeElapsed / mSongSize;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(lValueX, 0);
                if (lValueX > 1)
                    NextMusic();
            }

            if (mIsPaused && source.isPlaying) {
                source.Pause();
            } else if (!mIsPaused) {
                source.UnPause();
            }

            if (source.isPlaying && mIsPlay && mIsNext && !mIsPaused) {
                mIsNext = false;
                mIsPrevious = false;
                source.clip = PlayMusic();
                //mSongSize = mSource.clip.length;
                source.Play();
                ResetNextIndexMusic();
            }

            if (source.isPlaying && mIsPlay && mIsPrevious && !mIsPaused) {
                mIsNext = false;
                mIsPrevious = false;
                source.clip = PlayMusic();
                //mSongSize = mSource.clip.length;
                source.Play();
                ResetPreviousIndexMusic();
            }

            if (!source.isPlaying && mIsPlay && !mIsPaused) {
                mIsNext = false;
                mIsPrevious = false;
                source.clip = PlayMusic();
                source.Play();
            }
        }

        public void IsPlaying()
        {
            textMusic.text = mMusicName[mIndexMusic];
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
            mIsNext = !mIsNext;
            if (mIsNext) {
                ++mIndexMusic;
                ResetNextIndexMusic();
                textMusic.text = mMusicName[mIndexMusic];
                mTimeElapsed = 0;
                mSongSize = mMyMusics[mIndexMusic].length;
                progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                //mSongSize = mSource.clip.length;
            }
        }

        public void StopMusic()
        {
            if (pause.activeSelf) {
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

            if (mIsPrevious) {
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

        private AudioClip PlayMusic()
        {
            if (mIsRap && mIndexMusic < mMyMusicsRap.Length) {
                return mMyMusicsRap[mIndexMusic];
            } else if (mIsElectro && mIndexMusic < mMyMusicsElectro.Length) {
                return mMyMusicsElectro[mIndexMusic];
            } else if (mIsRock && mIndexMusic < mMyMusicsRock.Length) {
                return mMyMusicsRock[mIndexMusic];
            } else if (mIsMusic && mIndexMusic < mMyMusics.Length) {
                return mMyMusics[mIndexMusic];
            } else
                return null;
        }

        private GameObject CreateText(Transform iCanvasTransform, float x, float y, string iTextToPrint, int iFontSize, Color iTextColor)
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
            text.font = mGothicFont;

            return lUITextGo;
        }

        private void ResetNextIndexMusic()
        {
            if (mIndexMusic == mMyMusics.Length)
                mIndexMusic = 0;
        }

        private void ResetPreviousIndexMusic()
        {
            if (mIndexMusic < 0)
                mIndexMusic = mMyMusics.Length - 1;
        }
    }
}