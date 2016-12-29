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

        private bool mIsAnimOn;

        void OnEnable()
        {
        }

        void Update()
        {
            mIsAnimOn = BabyPhoneData.Instance.IsAnimationOn;
        }

        public void PlayAnimation()
        {
            if (mIsAnimOn)
                animationViewer.SetActive(true);
        }

        public void Return()
        {
            animationViewer.SetActive(false);
        }

    }
}