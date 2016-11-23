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
            yield return new WaitForSeconds(3f);
            startScreen.SetActive(true);

            animator.SetBool("Close_WLoading", true);

            yield return new WaitForSeconds(1f);
            loadingScreen.SetActive(false);
        }
    }
}