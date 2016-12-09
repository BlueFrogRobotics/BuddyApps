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
        [HideInInspector]
        public bool mIsSentenceDone;

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

        public void OnClickedButtonTowin()
        {
            mAnimator.SetBool("IsWon", true);
        }

        public void OnClickedButtonReplay()
        {
            mReplay = true;
        }

        public void ClickButtonYesRule()
        {
            mAnimator.GetBehaviour<RulesState>().IsAnswerRuleYes = true;
            //mCanvasRules.SetActive(false);
        }

        public void CLickButtonNoRule()
        {
            mAnimator.GetBehaviour<RulesState>().IsAnswerRuleNo = true;
            //mCanvasRules.SetActive(false);
        }

        public void ClickButtonYesStart()
        {
            mAnimator.GetBehaviour<StartState>().IsAnswerYes = true;
        }

        public void CLickButtonNoStart()
        {
            mAnimator.GetBehaviour<StartState>().IsAnswerNo = true;
        }

        public void ClickButtonYesReplay()
        {
            mAnimator.GetBehaviour<ReplayState>().IsAnswerYes = true;
        }

        public void ClickButtonNoReplay()
        {
        }
    }
}
