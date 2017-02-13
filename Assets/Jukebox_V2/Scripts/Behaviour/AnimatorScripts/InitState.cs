using UnityEngine;
using System.Collections;
using BuddyOS.App;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

namespace BuddyApp.Jukebox
{
    public class InitState : AStateMachineBehaviour
    {

        private List<int> mIndexToPlay;
        public List<int> IndexToPlay { get { return mIndexToPlay; } set { mIndexToPlay = value; } }
        private int mPathLength;

        private FileInfo[] mPath;
        private DirectoryInfo mDirectory;
        private string mURLAndroid;

        private bool mIsCoroutineDone;

        public override void Init()
        {
            mIndexToPlay = new List<int>();
            mIsCoroutineDone = false;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mURLAndroid = "file://";
            mDirectory = new DirectoryInfo("/storage/emulated/0/Music");
            mPath = mDirectory.GetFiles("*.mp3");
            mPathLength = mPath.Length;
            StartCoroutine(FirstFill());
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mIsCoroutineDone)
                iAnimator.SetTrigger("InitDone");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        IEnumerator FirstFill()
        {
            for (int i = 0; i < mPathLength; ++i)
            {
                Debug.Log(" Remplissage indextoplay");
                mIndexToPlay.Add(i);
            }
            yield return mIndexToPlay;
            mIsCoroutineDone = true;
        }
    }
}

	 
