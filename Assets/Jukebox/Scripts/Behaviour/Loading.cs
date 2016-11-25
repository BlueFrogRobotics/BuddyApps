using UnityEngine;
using System.Collections;

namespace BuddyApp.Jukebox
{
    public class Loading : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private GameObject loadingScreen;

        [SerializeField]
        private GameObject startScreen;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(LoadingScreen());
        }

        private IEnumerator LoadingScreen()
        {
            animator.SetBool("Open_WLoading", true);
            yield return new WaitForSeconds(3F);
            startScreen.SetActive(true);

            animator.SetBool("Open_WLoading", false);
            animator.SetBool("Close_WLoading", true);
            yield return new WaitForSeconds(1F);
            loadingScreen.SetActive(false);
            animator.SetBool("Close_WLoading", false);
        }
    }
}