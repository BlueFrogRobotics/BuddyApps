using UnityEngine.UI;
using UnityEngine;
using System.Collections;

using BlueQuark;

namespace BuddyApp.MemoryGame
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class MemoryGameBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app quit happened
         */
        private MemoryGameData mAppData;

        private Animator mAnimator;

        private void Awake()
        {
            mAnimator = GetComponent<Animator>();
        }

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            mAppData = MemoryGameData.Instance;
            MemoryGameActivity.Init(mAnimator, this);
        }


    }
}