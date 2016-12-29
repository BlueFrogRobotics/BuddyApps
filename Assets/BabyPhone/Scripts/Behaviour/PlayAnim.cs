using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class PlayAnim : MonoBehaviour
    {
        [SerializeField]
        private GameObject animationViewer;

        void OnEnable()
        {
        }

        void Update()
        {

        }
        public void PlayAnimation()
        {
            animationViewer.SetActive(true);
        }

        public void Return()
        {
            animationViewer.SetActive(false);
        }

    }
}