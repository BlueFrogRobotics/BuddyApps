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
            yield return new WaitForSeconds(3f);
            startScreen.SetActive(true);
            aiFreezeDance.SetActive(true);
            canvasQuit.SetActive(true);

            animator.SetBool("Close_WLoading", true);

            yield return new WaitForSeconds(1f);
            loadScreen.SetActive(false);
        }
    }
}