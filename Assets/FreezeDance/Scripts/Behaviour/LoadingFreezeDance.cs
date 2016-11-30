using UnityEngine;
using System.Collections;

namespace BuddyApp.FreezeDance
{
    public class LoadingFreezeDance : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private GameObject loadScreen;

        [SerializeField]
        private GameObject startScreen;

        [SerializeField]
        private GameObject aiFreezeDance;

        [SerializeField]
        private GameObject canvasQuit;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(LoadingScreen());
        }

        private IEnumerator LoadingScreen()
        {
            startScreen.SetActive(true);
            animator.SetBool("Open_WLoading", true);
            yield return new WaitForSeconds(3F);
            aiFreezeDance.SetActive(true);
            canvasQuit.SetActive(true);

            animator.SetBool("Open_WLoading", false);
            animator.SetBool("Close_WLoading", true);
            yield return new WaitForSeconds(1F);
            animator.SetBool("Close_WLoading", false);
            loadScreen.SetActive(false);
        }
    }
}