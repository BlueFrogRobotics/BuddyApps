using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class LoadingScreenState : AStateMachineBehaviour
    {
        private const int LOADING_TIME = 3;

        private Animator mLoadingAnimator;
        private GameObject mWindoAppOverBlack;
        private float mTime;

        public override void Init()
        {
            mLoadingAnimator = GetGameObject(0).GetComponent<Animator>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTime = 0;
            mRGBCam.Close();
            StartCoroutine(LoadingScreenFunc());
            BYOS.Instance.VocalManager.EnableTrigger = false;
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTime += Time.deltaTime;
            if (mTime >= LOADING_TIME)
                iAnimator.SetTrigger("StartApp");
        }

        IEnumerator LoadingScreenFunc()
        {
            mLoadingAnimator.SetTrigger("Open_WLoading");
            yield return new WaitForSeconds(3f);
            mLoadingAnimator.SetTrigger("Close_WLoading");
            yield return new WaitForSeconds(1f);
        }
    }
}
