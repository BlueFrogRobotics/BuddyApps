using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class PlayCartoon : MonoBehaviour
    {
        //[SerializeField]
        //private GameObject animObject;

        private Animator cartoonAnimator;

        private bool mIsAnimationOn;
        private int mCartoonChoice;

        void OnAwake()
        {
            
        }
        void OnEnable()
        {
            cartoonAnimator = GetComponent<Animator>();

            // collect user configuration
            mIsAnimationOn = BabyPhoneData.Instance.IsAnimationOn;
            mCartoonChoice = (int)BabyPhoneData.Instance.AnimationToPlay;

            if (mIsAnimationOn)
            {
                Debug.Log("Animation on ?" + mIsAnimationOn);

                cartoonAnimator.SetBool("IsPlaying", true);
                if (mCartoonChoice == 0)
                    cartoonAnimator.SetTrigger("Hibou");
                else
                    cartoonAnimator.SetTrigger("Chrsitmas"); //penser à la corriger!
            }
        }

        void OnDisable()
        {
            cartoonAnimator.SetBool("IsPlaying", false);
        }

        //public void PlayAnimation()
        //{
        //    if (mIsAnimationOn)
        //    {
        //        Debug.Log("Animation on ?" + mIsAnimationOn);
        //        //animObject.SetActive(true);
        //        cartoonAnimator.SetBool("IsPlaying", true);
        //        if (mCartoonChoice == 0)
        //            cartoonAnimator.SetTrigger("Hibou");
        //        else
        //            cartoonAnimator.SetTrigger("Chrsitmas"); //penser à la corriger!
        //    }               
        //}

        //public void Return()
        //{
            
        //    //animObject.SetActive(false);
        //}

    }
}