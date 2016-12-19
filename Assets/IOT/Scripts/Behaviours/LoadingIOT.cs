using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class LoadingIOT : MonoBehaviour
    {
        [SerializeField]
        private GameObject loadingScreen;

        //[SerializeField]
        //private GameObject startScreen;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Animator iAAnimator;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(LoadingScreen());
        }

        private IEnumerator LoadingScreen()
        {
            //startScreen.SetActive(true);
            animator.SetBool("Open_WLoading", true);
            yield return new WaitForSeconds(3F);

            animator.SetBool("Open_WLoading", false);
            animator.SetBool("Close_WLoading", true);
            yield return new WaitForSeconds(1F);
            animator.SetBool("Close_WLoading", false);
            loadingScreen.SetActive(false);
            iAAnimator.SetTrigger("Next");
        }
    }
}
