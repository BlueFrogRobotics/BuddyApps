using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.RLGL
{
    [RequireComponent(typeof(StateMachineAppLinker))]
    [RequireComponent(typeof(FreezeDance.MotionGame))]
    public class RLGLBehaviour : MonoBehaviour
    {
        private TextToSpeech mTTS;
        private Animator mAnimator;
        [HideInInspector]
        public bool mReplay;

        [SerializeField]
        private GameObject Background;

        [SerializeField]
        private GameObject WindowQuestion;

        private int mIndex;
        public int Index { get { return mIndex; } set { mIndex = value; } }

        private bool mIsClicked;
        public bool IsClicked { get { return mIsClicked; } set { mIsClicked = value; } }

        //[SerializeField]
        //private GameObject mCanvasRules;

        // Use this for initialization
        void Start()
        {
            mAnimator = GetComponent<Animator>();

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnClickedButtonYes()
        {
            mIsClicked = true;
            if(mIndex == 0)
            {
                mAnimator.GetBehaviour<StartState>().IsAnswerYes = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                //WindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
                Debug.Log("ON CLICK BUTTON YES START");
            }
            else if (mIndex == 1)
            {
                mAnimator.GetBehaviour<RulesState>().IsAnswerRuleYes = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                //WindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
                Debug.Log("ON CLICK BUTTON YES RULES");
            }
            else if (mIndex == 2)
            {
                mAnimator.GetBehaviour<ReplayState>().IsAnswerYes = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                //WindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
                Debug.Log("ON CLICK BUTTON YES REPLAY");
            }
        }

        public void OnClickedButtonNo()
        {
            mIsClicked = true;
            if (mIndex == 0)
            {
                mAnimator.GetBehaviour<StartState>().IsAnswerNo = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                //WindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");

                Debug.Log("ON CLICK BUTTON NO START");
            }
            else if (mIndex == 1)
            {
                mAnimator.GetBehaviour<RulesState>().IsAnswerRuleNo = true;
                Background.GetComponent<Animator>().SetTrigger("Close_BG");
                //WindowQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
                Debug.Log("ON CLICK BUTTON NO RULES");
            }
            else if (mIndex == 2)
            {
                BYOS.Instance.AppManager.Quit();
                //mAnimator.GetBehaviour<ReplayState>().IsAnswerNo = false;
            }
        }

        public void OnCLickTuto()
        {
            mIsClicked = true;
        }

        public void OnCLickMenu()
        {
            mIsClicked = true;
        }

        public void OnClickedButtonTowin()
        {
            mAnimator.SetBool("IsWon", true);
        }

        public void OnClickedButtonReplay()
        {
            mReplay = true;
        }

        //public void ClickButtonYesRule()
        //{
        //    mAnimator.GetBehaviour<RulesState>().IsAnswerRuleYes = true;
        //}

        //public void CLickButtonNoRule()
        //{
        //    mAnimator.GetBehaviour<RulesState>().IsAnswerRuleNo = true;
        //}

        //public void ClickButtonYesStart()
        //{
        //    mAnimator.GetBehaviour<StartState>().IsAnswerYes = true;
        //}

        //public void CLickButtonNoStart()
        //{
        //    mAnimator.GetBehaviour<StartState>().IsAnswerNo = true;
        //}

        //public void ClickButtonYesReplay()
        //{
        //    mAnimator.GetBehaviour<ReplayState>().IsAnswerYes = true;
        //}

        //public void ClickButtonNoReplay()
        //{
        //}
    }
}
