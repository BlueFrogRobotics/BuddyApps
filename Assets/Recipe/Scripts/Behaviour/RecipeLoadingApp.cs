﻿using UnityEngine;
using BuddyOS;
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

        void Start()
        {
            BYOS.Instance.VocalActivation.enabled = false;
            StartCoroutine(LoadingScreen());
        }

        private IEnumerator LoadingScreen()
        {
            animator.SetBool("Open_WLoading", true);
            yield return new WaitForSeconds(3F);
            animator.SetBool("Open_WLoading", false);
            animator.SetBool("Close_WLoading", true);
            yield return new WaitForSeconds(1F);
            loadingScreen.SetActive(false);
            animator.SetBool("Close_WLoading", false);
            startApp.SetActive(true);
        }
    }
}