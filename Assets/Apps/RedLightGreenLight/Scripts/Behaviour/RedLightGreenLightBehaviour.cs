using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.RedLightGreenLight
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class RedLightGreenLightBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private RedLightGreenLightData mAppData;
        private Animator mAnimator; 
        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			RedLightGreenLightActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = RedLightGreenLightData.Instance;

            mAnimator = GetComponent<Animator>();
        }

        public void OnClickedButtonTowin()
        {
            mAnimator.SetBool("IsWon", true);
        }
    }
}