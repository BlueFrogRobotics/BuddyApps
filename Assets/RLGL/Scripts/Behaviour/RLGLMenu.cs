using UnityEngine;
using System.Collections;
using BuddyOS;
using System;

namespace BuddyApp.RLGL
{
    public class RLGLMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject Listener;

        private TextToSpeech mTTS;
        private bool mIsQuestionDone;

        private bool mIsAnswerPlayYes;
        public bool IsAnswerPlayYes { get { return mIsAnswerPlayYes; } set { mIsAnswerPlayYes = value; } }

        // Use this for initialization
        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            
        }

        // Update is called once per frame
        void Update()
        {
            if(!mIsQuestionDone && mTTS.HasFinishedTalking())
            {
                mTTS.Say("Hey, what do you want to do?");
                mIsQuestionDone = true;
            }
            StartCoroutine(AskingStartGame(5));
        }

        private IEnumerator AskingStartGame(int iIndex)
        {
            yield return new WaitForSeconds(3.0F);
            Listener.GetComponent<RLGLListener>().STTRequest(iIndex);
            while (!mIsAnswerPlayYes)
            {
                yield return null;
            }
        }
    }

}