using UnityEngine;
using System.Collections;
using Buddy;
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

        private Button mButtonDance;

        public override void Start()
        {

            BYOS.Instance.Header.DisplayParametersButton = false;
            mButtonDance = GetGameObject(6).GetComponent<Button>();
            mButtonDance.onClick.Invoke();
            mIndexToPlay = new List<int>();
            mIsCoroutineDone = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mURLAndroid = "file://";
            mDirectory = new DirectoryInfo("/storage/emulated/0/Music");
            mPath = mDirectory.GetFiles("*.mp3");
            mPathLength = mPath.Length;
            StartCoroutine(FirstFill());
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mIsCoroutineDone)
                iAnimator.SetTrigger("InitDone");
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
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

	 
