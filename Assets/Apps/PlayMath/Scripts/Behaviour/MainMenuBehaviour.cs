﻿using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
    public class MainMenuBehaviour : AnimationSyncBehaviour {
        [SerializeField]
		private Animator mPlayMathAnimator;

        private Text mTitleTop;
        private Text mTablesButton;
        private Text mSettingsButton;
        private Text mPlayButton;
        private Text mScoresButton;
        private Text mCertifButton;

        private bool mTriggerOnce;

		public void Start() {
            // Retrieve every child text component
            mTitleTop = this.gameObject.transform.Find("Top_UI/Title_Top").GetComponent<Text>();
            mTablesButton = this.gameObject.transform.Find("Middle_UI/Button_Tables/Text").GetComponent<Text>();
            mSettingsButton = this.gameObject.transform.Find("Middle_UI/Button_Settings/Text").GetComponent<Text>();
            mPlayButton = this.gameObject.transform.Find("Middle_UI/Button_Play/Text").GetComponent<Text>();
            mScoresButton = this.gameObject.transform.Find("Middle_UI/Button_Scores/Text").GetComponent<Text>();
            mCertifButton = this.gameObject.transform.Find("Middle_UI/Button_Certificate/Text").GetComponent<Text>();
		}

        public void InitState()
        {
            TranslateUI();
            mTriggerOnce = true;
        }

        public void TranslateUI(){
            mTitleTop.text = BYOS.Instance.Dictionary.GetString("menutitle").ToUpper();
            mTablesButton.text = BYOS.Instance.Dictionary.GetString("tablelabel").ToUpper();
            mSettingsButton.text = BYOS.Instance.Dictionary.GetString("settingslabel").ToUpper();
            mPlayButton.text = BYOS.Instance.Dictionary.GetString("playlabel").ToUpper();
            mScoresButton.text = BYOS.Instance.Dictionary.GetString("leaderboardlabel").ToUpper();
            mCertifButton.text = BYOS.Instance.Dictionary.GetString("certificationlabel").ToUpper();
        }

		public void OnClickSettings() {
            if (mTriggerOnce)
            {
                mTriggerOnce = false;
                mPlayMathAnimator.SetTrigger("GameSettings");
            }
		}

		//public void OnClickBestScores() {
		//	mPlayMathAnimator.SetTrigger("BestScore");
		//}

		public void OnClickPlay() {
            if (mTriggerOnce)
            {
                User.Instance.GameParameters.Table = 0;
                User.Instance.GameParameters.Sequence = 4;
                mTriggerOnce = false;
                mPlayMathAnimator.SetTrigger("Play");
            }
		}

        public void OnClickTables() {
            //if (mTriggerOnce)
            //{
                User.Instance.GameParameters.Sequence = 10;
                mTriggerOnce = false;
                //mPlayMathAnimator.SetTrigger("SelectTable"); 
            //}
        }
	}
}

