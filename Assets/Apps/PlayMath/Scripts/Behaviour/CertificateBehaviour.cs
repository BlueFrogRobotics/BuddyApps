using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.PlayMath{
	public class CertificateBehaviour : MonoBehaviour {

        [SerializeField]
		private Animator mPlayMathAnimator;
        [SerializeField]
        private Text mTitleTop;
        [SerializeField]
        private Text mTextCertificate;
        [SerializeField]
        private Text mTextWhat;
        [SerializeField]
        private Text mTextApp;
        [SerializeField]
        private Text mTextScore;
        [SerializeField]
        private Score mScore;
        [SerializeField]
        private Certificate mCertificate;
        [SerializeField]
        private RawImage mUserPic;

   		void Start() {
        }

        public void SetCertificate() {
            mCertificate.GameParams = User.Instance.GameParameters;
            mCertificate.TimeStamp = DateTime.Now;

            mTitleTop.text = "Congratulations ! ";

            mTextWhat.text = mCertificate.GameParams.Operands.ToString() 
                + " at level " + mCertificate.GameParams.Difficulty;
            
            mTextApp.text = "Buddy Play Math on " + mCertificate.TimeStamp.ToString("MM/dd/yyyy HH:mm");

            mTextScore.text = "Perfect ! You have " 
                + mScore.CorrectAnswers + "/" + mCertificate.GameParams.Sequence + " !";

            mUserPic.texture = mCertificate.UserPic;
		}

		public void OnClickGoToMenu() {
			mPlayMathAnimator.SetTrigger("BackToMenu");
		}

		public void OnClickPlay() {
			mPlayMathAnimator.SetTrigger("Play");
		}
	}
}
