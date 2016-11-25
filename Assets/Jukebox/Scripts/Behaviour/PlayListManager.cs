using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public enum MusicType
    {
        RAP = 0,
        ELECTRO = 1,
        ROCK = 2,
        DEFAULT = RAP
    }

    public class PlayListManager : MonoBehaviour
    {
        /// <summary>
        /// UI
        /// </summary>
        [SerializeField]
        private GameObject canvas;

        [SerializeField]
        private Text typeFolder;

        [SerializeField]
        private Text numberMusicFolder;

        [SerializeField]
        private AudioSource source;

        private AudioClip[] mMyMusicsRock;
        private AudioClip[] mMyMusicsRap;
        private AudioClip[] mMyMusicsElectro;
        private AudioClip[] mMyMusics;
        private bool mIsRandom;

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

        private int mActualIndexMusic;

        void Awake()
        {
            //récupère les musiques de resources et les place dans des list définies de base
            mMyMusicsRap = Resources.LoadAll<AudioClip>("Music/Rap");
            mMyMusicsRock = Resources.LoadAll<AudioClip>("Music/Rock");
            mMyMusicsElectro = Resources.LoadAll<AudioClip>("Music/Electro");
            mMyMusics = Resources.LoadAll<AudioClip>("Music");

            mElectroMusicName = new List<string>();
            mRapMusicName = new List<string>();
            mRockMusicName = new List<string>();
            mMusicName = new List<string>();

            mActualIndexMusic = 0;
        }

        void Start()
        {
            mIsRandom = true;
            mIsMusicElectro = false;
            mIsMusicRock = false;
            mIsMusicRap = false;
            mVolume = 1f;
            source.volume = 0.1f;

            fillMusicName(mElectroMusicName, mMyMusicsElectro);
            fillMusicName(mRapMusicName, mMyMusicsRap);
            fillMusicName(mRockMusicName, mMyMusicsRock);
            fillMusicName(mMusicName, mMyMusics);
        }

        void Update()
        {
            if (mIsRandom) {
                if (mIsMusicRap) {

                } else if (mIsMusicElectro) {

                } else if (mIsMusicRock) {

                } else {

                }
            } else if (!mIsRandom) {
                if (mIsMusicRap) {
                    //PlayMusic(mMyMusicsRap);
                } else if (mIsMusicElectro) {
                    //PlayMusic(mMyMusicsElectro);
                } else if (mIsMusicRock) {
                    //PlayMusic(mMyMusicsRock);
                } else {
                    //PlayMusic(mMyMusics);
                }
            }
        }

        public static void ShuffleMusic<T>(T[] iArr)
        {
            for (int i = iArr.Length - 1; i > 0; --i) {
                int lRandom = Random.Range(0, i + 1);
                T lTmp = iArr[i];
                iArr[i] = iArr[lRandom];
                iArr[lRandom] = lTmp;
            }
        }

        public void UpVolume()
        {
            //use buddy api speaker
            source.volume += 0.05F;
        }

        public void DownVolume()
        {
            //use buddy api speaker
            source.volume -= 0.05F;
        }

        public void IsRandom()
        {
            mIsRandom = !mIsRandom;
        }

        public void NextMusic(AudioClip[] iAudioClip, AudioClip[] iAudioClipRandom)
        {
            source.Stop();
            if (mIsRandom)
                PlayRandomMusicArr(iAudioClip, iAudioClipRandom, mActualIndexMusic);
            else if (!mIsRandom)
                PlayMusic(iAudioClip, mActualIndexMusic);
        }

        private void fillMusicName(List<string> iMusicName, AudioClip[] iAudioCLip)
        {
            for (int i = 0; i < iAudioCLip.Length; ++i)
                iMusicName.Add(iAudioCLip[i].ToString());
        }

        private void PlayMusic(AudioClip[] IAudioClip, int iIndex)
        {
            if (!source.isPlaying)
                for (int i = 0; i < IAudioClip.Length - 1; ++i)
                    source.PlayOneShot(IAudioClip[Random.Range(0, IAudioClip.Length - 1)], mVolume);
        }

        private void PlayRandomMusicArr(AudioClip[] iAudioClip, AudioClip[] iAudioClipRandom, int iIndex)
        {
            if (!source.isPlaying)
                if (iIndex == 0) {
                    iAudioClipRandom = iAudioClip;
                    ShuffleMusic(iAudioClipRandom);
                }

            for (int i = iIndex; i < iAudioClipRandom.Length - 1; ++i) {
                source.PlayOneShot(iAudioClipRandom[i], this.mVolume);
                mActualIndexMusic = i;
            }
        }
    }
}