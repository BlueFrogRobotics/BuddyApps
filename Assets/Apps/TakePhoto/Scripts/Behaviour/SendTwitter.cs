using UnityEngine;
using System.IO;
using BlueQuark;

namespace BuddyApp.TakePhoto
{
	public sealed class SendTwitter : AStateMachineBehaviour
	{
		private string mToken;
		private string mTokenSecret;
		private string mConsumerKey;
		private string mConsumerSecret;
		private string mAllHashtag;
		private Publish mWhereToPublish;
		private string[] mTweetMsg;
		private EMail mMail;
		private bool mUseKey;

		private int mRandom;
		private Texture2D mTexture;
		private TakePhotoData mTakePhotoBH;

		private XMLData mXMLData;

		private bool mMailOrTweetSent;
		private bool mIsVocalSaid;

        private bool mSuccess;

		public override void Start()
		{
			mMailOrTweetSent = false;
			mIsVocalSaid = false;
			mRandom = 0;
			mXMLData = null;
			mMail = new EMail();
			mTexture = new Texture2D(1, 1);
            mTweetMsg = null;
            mSuccess = false;

            string configFileName = Buddy.Platform.Application.PersistentDataPath + "Shared/config.xml";
            if (!File.Exists(configFileName))
            {
                // Persistent file doesn't exist, try local configuration
                Debug.Log("TakePhoto SendTwitter::Start Config file doesn't exist " + configFileName);
                configFileName = Buddy.Resources.GetRawFullPath("Twitter/config.xml");
                if (!File.Exists(configFileName))
                {
                    Debug.Log("TakePhoto SendTwitter::Start Config file doesn't exist " + configFileName + " ");
                    return;
                }
            }


            mXMLData = Utils.UnserializeXML<XMLData>(configFileName);

            if (mXMLData != null)
            {
                mToken = mXMLData.Token;
                mTokenSecret = mXMLData.TokenSecret;
                mConsumerKey = mXMLData.ConsumerKey;
                mConsumerSecret = mXMLData.ConsumerSecret;
                mUseKey = mXMLData.UseKey;
                mAllHashtag = mXMLData.TwitterHashtag;
                if (!string.IsNullOrEmpty(mXMLData.TwitterText))
                    mTweetMsg = mXMLData.TwitterText.Split('/');
                mWhereToPublish = mXMLData.WhereToPublish;
            }
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            bool success = false;
			if (mXMLData != null) {
				mMail.Addresses.Clear();
				string lMessage = "";
                if (mTweetMsg != null)
				    lMessage = mTweetMsg[Random.Range(0, mTweetMsg.Length)];
				lMessage += " " + mAllHashtag;
				if (mWhereToPublish == Publish.MAIL) {
                    success = SendMail();
				} else if (mWhereToPublish == Publish.TWITTER) {
                    success = SendTweet(lMessage);
				} else {
                    success = SendMail();
                    success |= SendTweet(lMessage);
				}
			}
            if (!success)
            {
                Buddy.Vocal.SayKey("errorconnexion");
                Trigger("AskPhotoAgain");
            }
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mMailOrTweetSent && !mIsVocalSaid)
            {
                mIsVocalSaid = true;
                string keyMsg = mSuccess ? "pictureshared" : "errorconnexion";
                Buddy.Vocal.SayKey(keyMsg, (iOutput) =>
                {
                    Trigger("AskPhotoAgain");
                });
               
            }
		}

		private bool SendTweet(string iMsg)
		{
			if (mXMLData != null
                && !string.IsNullOrEmpty(mToken)
                && !string.IsNullOrEmpty(mTokenSecret)
                && !string.IsNullOrEmpty(mConsumerKey)
                && !string.IsNullOrEmpty(mConsumerSecret))
            {
				Twitter.AccessTokenResponse accessToken = new Twitter.AccessTokenResponse {
					Token = mToken,
					TokenSecret = mTokenSecret
				};

				StartCoroutine(Twitter.API.UploadMedia(LoadTexture(mTexture), iMsg, mConsumerKey, mConsumerSecret, accessToken,
														 new Twitter.PostTweetCallback(this.OnPostTweet)));

                return true;
			}
            return false;
		}

		private bool SendMail()
		{
			if (mXMLData != null
                && !string.IsNullOrEmpty(mXMLData.AdressMailSender)
                && !string.IsNullOrEmpty(mXMLData.PasswordMail)
                && !string.IsNullOrEmpty(mXMLData.AdressMailReceiver))
            { 
                string lAdress = mXMLData.AdressMailSender;
				string lPasswordMail = mXMLData.PasswordMail;
				mMail.AddTexture2D(LoadTexture(mTexture), TakePhotoData.Instance.PhotoPath);
				mMail.Addresses.Add(mXMLData.AdressMailReceiver);
				mMail.Subject = mXMLData.SubjectMail;
				mMail.Body = mXMLData.BodyMail;
				Buddy.WebServices.EMailSender.Send(lAdress, lPasswordMail, SMTP.GMAIL, mMail, iInput => OnMailSent(iInput));
                return true;
            }
            return false;
        }

		private void OnMailSent(bool success)
        {
            Debug.Log("OnMailSent - " + (success ? "succeeded." : "failed."));
            mSuccess = success;
            mMailOrTweetSent = true;
        }

		private void OnPostTweet(bool success)
		{
			Debug.Log("OnPostTweet - " + (success ? "succeeded." : "failed."));
            mSuccess = success;
            mMailOrTweetSent = true;
		}

		private Texture2D LoadTexture(Texture2D iText)
		{
            byte[] bytes = new byte[File.ReadAllBytes(TakePhotoData.Instance.PhotoPath).Length];
            bytes = File.ReadAllBytes(TakePhotoData.Instance.PhotoPath);
            iText.LoadImage(bytes);
			return iText;
		}
	}
}