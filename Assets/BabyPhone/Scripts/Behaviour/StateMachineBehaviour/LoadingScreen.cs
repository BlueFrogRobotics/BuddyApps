using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class LoadingScreen : AStateMachineBehaviour
    {

        private Animator mLoadingAnimator;

        public override void Init()
        {
            mLoadingAnimator = GetGameObject(0).GetComponent<Animator>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            StartCoroutine(LoadingScreenFunc());
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("StartApp", true);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        IEnumerator LoadingScreenFunc()
        {
            mLoadingAnimator.SetTrigger("Open_WLoading");
            yield return new WaitForSeconds(3f);
            mLoadingAnimator.SetTrigger("Close_WLoading");
            yield return new WaitForSeconds(1f);
        }

        //IEnumerator WaintEndOfLoading()
        //{
        //    yield return new WaitForSeconds(5f);
        //}
    }
}
