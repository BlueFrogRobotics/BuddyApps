using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
    public class CertificateBehaviour : AnimationSyncBehaviour {

        [SerializeField]
		private Animator mPlayMathAnimator;
        [SerializeField]
        private Score mScore;
        [SerializeField]
        private Certificate mCertificate;
        [SerializeField]
        private RawImage mUserPic;

        private Text mTitleTop;
        private Text mTextCertificate;
        private Text mTextWhat;
        private Text mTextApp;
        private Text mTextScore;
        private Text mShare;

        private bool mTriggerOnce;

   		void Start() {
            mTitleTop = this.gameObject.transform.Find("Top_UI/Title_Top").GetComponent<Text>();

            mTextCertificate = this.gameObject.transform.Find(
                "Middle_UI/Background_Mask/Certificate/Texts/Text_Certificate").GetComponent<Text>();
            mTextWhat = this.gameObject.transform.Find(
                "Middle_UI/Background_Mask/Certificate/Texts/Text_What").GetComponent<Text>();
            mTextApp = this.gameObject.transform.Find(
                "Middle_UI/Background_Mask/Certificate/Texts/Text_App").GetComponent<Text>();
            mTextScore = this.gameObject.transform.Find(
                "Middle_UI/Background_Mask/Certificate/Texts/Text_Score").GetComponent<Text>();
            
            mShare = this.gameObject.transform.Find("Bottom_UI/Button_Share/Text").GetComponent<Text>();
        }

        public void InitState()
        {
            TranslateUI();
            SetCertificate();
            mTriggerOnce = true;
        }

        private void SetCertificate() {
            mCertificate.GameParams = User.Instance.GameParameters;
            mCertificate.TimeStamp = DateTime.Now;

            if (mCertificate.GameParams.Table > 0)
            {
                mTextWhat.text = String.Format(BYOS.Instance.Dictionary.GetString("certificatetablelabel"),
                    mCertificate.GameParams.Table);
            }
            else
            {
                mTextWhat.text = String.Format(BYOS.Instance.Dictionary.GetString("certificatewhatlabel"),
                    mCertificate.GameParams.Operands.ToString(),
                    mCertificate.GameParams.Difficulty);
            }
            
            mTextApp.text =  String.Format( BYOS.Instance.Dictionary.GetString("certificateapplabel"),
                                            mCertificate.TimeStamp.ToString("MM/dd/yyyy HH:mm"));

            mTextScore.text = String.Format(BYOS.Instance.Dictionary.GetString("certificationscorelabel"),
                                            mScore.CorrectAnswers,
                                            mCertificate.GameParams.Sequence);

            mUserPic.texture = mCertificate.UserPic;

            User.Instance.Certificates.NewCertificate(mCertificate);
            User.SaveUser();

            BYOS.Instance.Interaction.TextToSpeech.SayKey("perfectscore");
		}

		public void OnClickGoToMenu() {
            if (mTriggerOnce)
            {
                mTriggerOnce = false;
                mPlayMathAnimator.SetTrigger("BackToMenu");
            }
		}

		public void OnClickPlay() {
            if (mTriggerOnce)
            {
                mTriggerOnce = false;
                mPlayMathAnimator.SetTrigger("Play");
            }
		}

        public void TranslateUI() {
            mTitleTop.text = BYOS.Instance.Dictionary.GetString("certificatetitle");
            mShare.text = BYOS.Instance.Dictionary.GetString("sharelabel").ToUpper();
        }
	}
}
