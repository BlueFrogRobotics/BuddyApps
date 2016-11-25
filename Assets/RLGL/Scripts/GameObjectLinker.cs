using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class GameObjectLinker : MonoBehaviour
    {
        private TextToSpeech mTTS;

        [SerializeField]
        private GameObject mMotionDetector;
        public GameObject MotionDetector{ get { return mMotionDetector; } set { mMotionDetector = value; } }

        [SerializeField]
        private GameObject mCanvasUI;
        public GameObject CanvasUI{ get { return mCanvasUI; } set  {mCanvasUI = value; } }


        //[SerializeField]
        //private GameObject mGameplay;
        //public GameObject Gameplay { get { return mGameplay; } set { mGameplay = value; } }

        [SerializeField]
        private GameObject mWindowQuestion;
        public GameObject WindowQuestion { get { return mWindowQuestion; } set { mWindowQuestion = value; } }

        private Animator mAnimator;

        [HideInInspector]
        public bool mReplay;
        [HideInInspector]
        public bool mIsSentenceDone;

        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mAnimator = GetComponent<Animator>();
            mReplay = false;
            mIsSentenceDone = false;
        }

        void Update()
        {
            if(mReplay && !mIsSentenceDone)
            {
                mTTS.Say("Wesh poto on va rejouer");
                mIsSentenceDone = true;
            }
            if (mTTS.HasFinishedTalking() && mIsSentenceDone)
            {
                mAnimator.GetBehaviour<ReplayState>().IsAnswerYes = true;
                //mAnimator.SetBool("IsReplayDone", true);
                mReplay = false;
                mIsSentenceDone = false;
            }

        }

        public void OnClickedButtonTowin()
        {
            //Debug.Log("GOLINKER BUTTON TO WIN");
            mAnimator.SetBool("IsWon", true);
        } 

        public void OnClickedButtonReplay()
        {
            mReplay = true;
        }
    }


}
