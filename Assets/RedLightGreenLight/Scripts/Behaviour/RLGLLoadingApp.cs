using UnityEngine;
using System.Collections;
using Buddy;

namespace BuddyApp.RedLightGreenLight
{
    public class RLGLLoadingApp : MonoBehaviour
    {
        [SerializeField]
        private GameObject BackGroundBlack;

        [SerializeField]
        private GameObject loadingScreen;

        [SerializeField]
        private GameObject startScreen;

        [SerializeField]
        private Animator animator;

        // Use this for initialization
        void Start()
        {
            
            BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
            StartCoroutine(LoadingScreen());
        }

        private IEnumerator LoadingScreen()
        {
            //animator.SetBool("Open_WLoading", true);
            animator.SetTrigger("Open_WLoading");
            yield return new WaitForSeconds(3F);
            //startScreen.SetActive(true);
            //animator.SetBool("Open_WLoading", false);
            //animator.SetBool("Close_WLoading", true);
            animator.SetTrigger("Close_WLoading");
            yield return new WaitForSeconds(1F);

            BackGroundBlack.GetComponent<Animator>().SetTrigger("Open");
            startScreen.GetComponent<Animator>().SetTrigger("Open_WMenu3");
            startScreen.GetComponent<RLGLMenu>().IsAnswerPlayYes = false;
            //loadingScreen.SetActive(false);
            //animator.SetBool("Close_WLoading", false);
        }
    }
}

