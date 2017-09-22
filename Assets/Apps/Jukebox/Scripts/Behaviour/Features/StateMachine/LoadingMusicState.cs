using UnityEngine;
using System.Collections;
using Buddy;
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

        /// <summary>
        /// SECONDE VERSION JUKEBOX
        /// </summary>
        /// 


        private bool mIsPlaylist;
        public bool IsPlaylist { get { return mIsPlaylist; } set { mIsPlaylist = value; } }

        private List<int> mIndexToPlay;

        public override void Start()
        {
            mIndexMusic = 0;
            mIsPlaylist = false;
            
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(!mIsPlaylist)
                mIndexToPlay = iAnimator.GetBehaviour<InitState>().IndexToPlay;
            mMusicPlay = GetGameObject(0).GetComponent<AudioSource>();
            if(mMusicPlay.isPlaying)
                mMusicPlay.Stop();
            mIsLoadDone = false;
            mURLAndroid = "file://";
            mDirectory = new DirectoryInfo("/storage/emulated/0/Music");
            mPath = mDirectory.GetFiles("*.mp3");
            mPathLength = mPath.Length;
            
            mWWW = new WWW(mURLAndroid + mPath[mIndexToPlay[mIndexMusic]]);
            StartCoroutine(GetMusic(mWWW));
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mIsLoadDone)
            {
                iAnimator.SetTrigger("Play");
                mIsLoadDone = false;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
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

