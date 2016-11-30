using UnityEngine;
using System.Collections;

namespace BuddyApp.Alarm
{
    public class LoadingMenuAlarm : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private GameObject loadingScreen;

        [SerializeField]
        private GameObject startScreen;

        [SerializeField]
        private GameObject globalIA;

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
            globalIA.SetActive(true);

            animator.SetBool("Open_WLoading", false);
            animator.SetBool("Close_WLoading", true);

            yield return new WaitForSeconds(1F);
            loadingScreen.SetActive(false);
        }
    }
}