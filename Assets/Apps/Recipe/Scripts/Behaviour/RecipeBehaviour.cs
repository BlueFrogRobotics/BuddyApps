using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class RecipeBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private RecipeData mAppData;

        private Animator mAnimatorRecipe;
        private float mValueHeadYes;
        private float mValueHeadNo;
        private const float HEAD_YES_ANGLE_INCREMENT = 5F;
        private const float HEAD_NO_ANGLE_INCREMENT = 10F;
        private bool mUserPlaceHead;
        public bool mWantManualPositionning { get; set; }

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			RecipeActivity.Init(null);

            /*
			* Init your app data
			*/
            mWantManualPositionning = false;
            mUserPlaceHead = false;
            mAppData = RecipeData.Instance;

            mAnimatorRecipe = GetComponent<Animator>();

            mValueHeadYes = Buddy.Actuators.Head.Yes.Angle;
            mValueHeadNo = Buddy.Actuators.Head.No.Angle;
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters();
           
            StartCoroutine(RetrieveCredentialsAsync());
        }

        /// <summary>
        /// Generic function to move the head
        /// </summary>
        /// <param name="iButtonClicked"> 0 : move to the top, 1 : move to the right, 2 : move to the bottom, 3 : move to the left </param>
        public void MoveHead(int iButtonClicked)
        {
            RecipeUtils.DebugColor("BUTTON MOVE HEAD : ", "red");
            mWantManualPositionning = true;
            Buddy.Navigation.Stop();
            if(!mUserPlaceHead)
            {
                mUserPlaceHead = true;
                Buddy.Vocal.SayKey("recipeusermovetherobot");
            }

            

            //if (!Buddy.Actuators.Head.IsBusy)
            //{
            //mettre une value yes et no pour éviter la double opération
                switch (iButtonClicked)
                {
                    case 0:
                        RecipeUtils.DebugColor("TOP : " + mValueHeadYes, "red");
                        mValueHeadYes += HEAD_YES_ANGLE_INCREMENT;
                        if ((mValueHeadYes) > 37)
                            mValueHeadYes = 37;
                        else
                            Buddy.Actuators.Head.Yes.SetPosition(mValueHeadYes);
                        break;
                    case 1:
                        RecipeUtils.DebugColor("RIGHT : " + mValueHeadNo, "red");
                        mValueHeadNo -= HEAD_NO_ANGLE_INCREMENT;
                        if ((mValueHeadNo) < -100)
                            mValueHeadNo = -100;
                        else
                            Buddy.Actuators.Head.No.SetPosition(mValueHeadNo);
                        break;
                    case 2:
                        RecipeUtils.DebugColor("BOTTOM : " + mValueHeadYes, "red");
                        mValueHeadYes -= HEAD_YES_ANGLE_INCREMENT;
                        if ((mValueHeadYes) > -10)
                            mValueHeadYes = -10;
                        else
                            Buddy.Actuators.Head.Yes.SetPosition(mValueHeadYes);
                        break;
                    case 3:
                        RecipeUtils.DebugColor("LEFT : " + mValueHeadNo, "red");
                        mValueHeadNo += HEAD_NO_ANGLE_INCREMENT;
                        if ((mValueHeadNo) > 100)
                            mValueHeadNo = 100;
                        else
                            Buddy.Actuators.Head.No.SetPosition(mValueHeadNo);
                        break;
                    default:
                        break;
                }
            //}

        }

        public void Validate()
        {
            mAnimatorRecipe.SetTrigger("HEAD_POSITION_DONE");
        }

        private IEnumerator RetrieveCredentialsAsync()
        {
            //OLD cred : http://bfr-dev.azurewebsites.net/dev/BuddyDev-mplqc5fk128f1.txt
            using (WWW lQuery = new WWW("http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt"))
            {
                yield return lQuery;
                Buddy.Vocal.DefaultInputParameters.Credentials = lQuery.text;
                RecipeData.Instance.mGoogleCredentials = lQuery.text;
                Debug.Log("<color=red>CREDENTIAL : </color>" + lQuery.text + " lol : " + RecipeData.Instance.mGoogleCredentials);
            }
        }


    }
}