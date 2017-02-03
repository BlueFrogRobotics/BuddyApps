using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System.IO;
using System.Collections.Generic;

namespace BuddyApp.Jukebox
{
    public class LoadingMusicState : AStateMachineBehaviour
    {
        private AudioSource mMusicPlay;

        private int mPathLength;
        public int PathLength { get { return mPathLength; } set { mPathLength = value; } }

        private FileInfo[] mPath;
        private DirectoryInfo mDirectory;
        private string mURLAndroid;
        private WWW mWWW;

        private int mIndexMusic;
        public int IndexMusic { get { return mIndexMusic; } set { mIndexMusic = value; } }

        private string mMusicName;
        public string MusicName { get { return mMusicName; } set { mMusicName = value; } }

        private AudioClip mOneMusic;
        public AudioClip OneMusic { get { return mOneMusic; } set { mOneMusic = value; } }

        private bool mIsLoadDone;
        private AudioSource msource;

        public override void Init()
        {
            mIndexMusic = 0;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMusicPlay = GetGameObject(0).GetComponent<AudioSource>();
            if(mMusicPlay.isPlaying)
                mMusicPlay.Stop();
            mIsLoadDone = false;
            mURLAndroid = "file://";
            mDirectory = new DirectoryInfo("/storage/emulated/0/Music");
            mPath = mDirectory.GetFiles("*.mp3");
            mPathLength = mPath.Length;
            mWWW = new WWW(mURLAndroid + mPath[mIndexMusic]);
            StartCoroutine(GetMusic(mWWW));
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mIsLoadDone)
            {
                iAnimator.SetTrigger("Play");
                mIsLoadDone = false;
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        IEnumerator GetMusic(WWW iWWW)
        {
            yield return iWWW;
            if (iWWW.error == null && iWWW != null)
            {
                mOneMusic = iWWW.GetAudioClip(false, true, AudioType.MPEG);
            }
            else
                Debug.Log("WWW ERROR : " + iWWW.error);
            mMusicName = mPath[mIndexMusic].FullName;
            mMusicName = mMusicName.Remove(0, 26);
            mIsLoadDone = true;
        }

    }
}

