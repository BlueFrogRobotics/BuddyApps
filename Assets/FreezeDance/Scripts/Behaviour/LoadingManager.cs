using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace BuddyApp.FreezeDance
{

    public class LoadingManager : MonoBehaviour
    {
        [SerializeField]
        private Animator loadingAnimator;

        [SerializeField]
        private GameObject backgroundTricks;

        [SerializeField]
        private GameObject loadingScreen;

        [SerializeField]
        private GameObject startScreen;

        [SerializeField]
        private GameObject victoryAnim;

        [SerializeField]
        private GameObject defeatAnim;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(LoadingScreenFunc());
        }

        IEnumerator LoadingScreenFunc()
        {
            yield return new WaitForSeconds(3f);
            startScreen.SetActive(true);
            backgroundTricks.SetActive(true);
            loadingAnimator.SetBool("HasLoaded", true);
            yield return new WaitForSeconds(1f);
            loadingScreen.SetActive(false);
        }
    }
}