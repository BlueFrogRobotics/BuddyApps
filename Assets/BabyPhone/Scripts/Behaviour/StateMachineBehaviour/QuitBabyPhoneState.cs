using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class QuitBabyPhoneState : AStateMachineBehaviour
    {
        private GameObject mWindowYesNoQuestion;
        private Animator mBackgroundBlackAnimator;
        private Animator mYesNoQuestionWindow;

        public override void Init()
        {
            mWindowYesNoQuestion = GetGameObject(10);
            mBackgroundBlackAnimator = GetGameObject(1).GetComponent<Animator>();
            mYesNoQuestionWindow = mWindowYesNoQuestion.GetComponent<Animator>();

            //mReturnButton = GetGameObject(13).GetComponent<Button>();
            //mQuitButton = GetGameObject(14).GetComponent<Button>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindowYesNoQuestion.SetActive(true);
            mBackgroundBlackAnimator.SetTrigger("Open_BG");
            mYesNoQuestionWindow.SetTrigger("Open_WQuestion");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindowYesNoQuestion.SetActive(false);
            mBackgroundBlackAnimator.SetTrigger("Close_BG");
            mYesNoQuestionWindow.SetTrigger("Close_WQuestion");
            iAnimator.SetInteger("ForwardState", -1);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

    }
}
