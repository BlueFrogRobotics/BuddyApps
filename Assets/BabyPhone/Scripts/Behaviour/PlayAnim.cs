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
        private Animator cartoonAnimator;

        [SerializeField]
        private GameObject animObject;

        private bool mIsAnimOn;
        private int mCartoonChoice;

        void OnEnable()
        {
        }

        void Update()
        {
            mIsAnimOn = BabyPhoneData.Instance.IsAnimationOn;
            mCartoonChoice = (int) BabyPhoneData.Instance.AnimationToPlay;
        }

        public void PlayAnimation()
        {
            if (mIsAnimOn)
            {
                animObject.SetActive(true);
                cartoonAnimator.SetBool("IsPlaying", true);
                if (mCartoonChoice == 0)
                    cartoonAnimator.SetTrigger("Hibou");
                else
                    cartoonAnimator.SetTrigger("Chrsitmas"); //penser à la corriger!
            }
                
        }

        public void Return()
        {
            cartoonAnimator.SetBool("IsPlaying", false);
            animObject.SetActive(false);
        }

    }
}