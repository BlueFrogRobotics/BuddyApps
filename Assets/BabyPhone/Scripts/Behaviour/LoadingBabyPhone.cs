using UnityEngine;
using System.Collections;

namespace BuddyApp.BabyPhone
{
    public class LoadingBabyPhone : MonoBehaviour
    {
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

            yield return new WaitForSeconds(1f);
            loadingScreen.SetActive(false);
        }
    }
}