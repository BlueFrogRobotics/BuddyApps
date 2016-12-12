using UnityEngine;
using UnityEngine.UI;
using BuddyOS;
using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneParameters : AStateMachineBehaviour
    {
        //private GameObject mParameters;
        private Animator mBackgroundBlackAnimator;
        private Animator mParametresAnimator;

        //private Button mValidateButton;
        //private Button mQuitButton;

        private bool mValidateParameters;

        public override void Init()
        {
            //mParameters = GetGameObject(12);
            mBackgroundBlackAnimator = GetGameObject(2).GetComponent<Animator>();
            mParametresAnimator = GetGameObject(3).GetComponent<Animator>();
            //mValidateButton = GetGameObject(13).GetComponent<Button>();
            //mQuitButton = GetGameObject(14).GetComponent<Button>();

            //mValidateButton.onClick.AddListener(Validate);
            //mQuitButton.onClick.AddListener(Quit);

            mValidateParameters = false;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mParameters.SetActive(true);
            Debug.Log("toto");
            mBackgroundBlackAnimator.SetTrigger("Open_BG");
            mParametresAnimator.SetTrigger("Open_WParametres");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mParameters.SetActive(false);
            mBackgroundBlackAnimator.SetTrigger("Close_BG");
            mParametresAnimator.SetTrigger("Close_WParametres");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mValidateParameters == true)
                iAnimator.SetBool("DoHeadAdjust", true);

        }

        public void Validate()
        {
            mValidateParameters = true;
        }

        public void Quit()
        {
            BYOS.Instance.AppManager.Quit();
        }
    }
}
