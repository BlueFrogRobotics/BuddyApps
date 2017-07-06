using UnityEngine;
using Buddy;
using System.Collections;

namespace BuddyApp.Recipe
{
    public class RecipeLoadingApp : MonoBehaviour
    {

        [SerializeField]
        private GameObject loadingScreen;

        [SerializeField]
        private GameObject startApp;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Animator animatorApp;

        void Start()
        {
            BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
            //BYOS.Instance.VocalManager.EnableDefaultErrorHandling = false;
            StartCoroutine(LoadingScreen());
        }

        private IEnumerator LoadingScreen()
        {
            animator.SetBool("Open_WLoading", true);
            yield return new WaitForSeconds(3F);
            animator.SetBool("Open_WLoading", false);
            animator.SetBool("Close_WLoading", true);
            yield return new WaitForSeconds(1F);
            animator.SetBool("Close_WLoading", false);
            loadingScreen.SetActive(false);
            //startApp.SetActive(true);
            animatorApp.SetTrigger("ActivateApp");
        }
    }
}