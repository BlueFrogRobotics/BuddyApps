using UnityEngine;
using System.Collections;

namespace BuddyApp.RLGL
{
    public class RLGLLoading : MonoBehaviour
    {
        //public Animator animator;
        [SerializeField]
        private GameObject mLoadingScreen;
        [SerializeField]
        private GameObject mStartScreen;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(LoadingScreen());
        }

        IEnumerator LoadingScreen()
        {
            yield return new WaitForSeconds(3f);
            mStartScreen.SetActive(true);

            //animator.SetBool();
            yield return new WaitForSeconds(1f);
            mLoadingScreen.SetActive(false);
        }
    }
}
