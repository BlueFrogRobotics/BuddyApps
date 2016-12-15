using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class QuitBabyPhone : AStateMachineBehaviour
    {
        private GameObject mWindoAppOverWhite;
        private Animator mBackgroundBlackAnimator;
        private Animator mYesNoQuestionWindow;

        private Button mReturnButton;
        private Button mQuitButton;

        public override void Init()
        {
            mWindoAppOverWhite = GetGameObject(12);
            mBackgroundBlackAnimator = GetGameObject(2).GetComponent<Animator>();
            mYesNoQuestionWindow = GetGameObject(13).GetComponent<Animator>();

            mReturnButton = GetGameObject(13).GetComponent<Button>();
            mQuitButton = GetGameObject(14).GetComponent<Button>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindoAppOverWhite.SetActive(true);
            mBackgroundBlackAnimator.SetTrigger("Open_BG");
            mYesNoQuestionWindow.SetTrigger("Open_WQuestion");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindoAppOverWhite.SetActive(false);
            mBackgroundBlackAnimator.SetTrigger("Close_BG");
            mYesNoQuestionWindow.SetTrigger("Close_WQuestion");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public void Validate()
        {

        }

        public void Quit()
        {
            BYOS.Instance.AppManager.Quit();
        }
    }
}
