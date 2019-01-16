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

		public override void Start()
		{
			mMailOrTweetSent = false;
			mIsVocalSaid = false;
			mRandom = 0;
			mXMLData = null;
			mMail = new EMail();
			mTexture = new Texture2D(1, 1);

			if (File.Exists(Buddy.Resources.GetRawFullPath("Twitter/config.xml"))) {

				mXMLData = new XMLData();
				mXMLData = Utils.UnserializeXML<XMLData>(Buddy.Resources.GetRawFullPath("Twitter/config.xml"));

				mToken = mXMLData.Token;
				mTokenSecret = mXMLData.TokenSecret;
				mConsumerKey = mXMLData.ConsumerKey;
				mConsumerSecret = mXMLData.ConsumerSecret;
				mUseKey = mXMLData.UseKey;
				mAllHashtag = mXMLData.TwitterHashtag;
				mTweetMsg = mXMLData.TwitterText.Split('/');
				mWhereToPublish = mXMLData.WhereToPublish;

			}
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mXMLData != null) {
				mMail.Addresses.Clear();
				string lMessage;
				lMessage = mTweetMsg[Random.Range(0, mTweetMsg.Length)];
				lMessage += " " + mAllHashtag;
				if (mWhereToPublish == Publish.MAIL) {
					SendMail();
				} else if (mWhereToPublish == Publish.TWITTER) {
					SendTweet(lMessage);
				} else {
					SendMail();
					SendTweet(lMessage);
				}
			}
			Trigger("AskPhotoAgain");
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mMailOrTweetSent && !mIsVocalSaid) {
				mIsVocalSaid = true;
				Buddy.Vocal.SayKey("pictureshared");
			}
		}

		private void SendTweet(string iMsg)
		{

			if (mXMLData != null) {
				Twitter.AccessTokenResponse accessToken = new Twitter.AccessTokenResponse {
					Token = mToken,
					TokenSecret = mTokenSecret
				};

				StartCoroutine(Twitter.API.UploadMedia(LoadTexture(mTexture), iMsg, mConsumerKey, mConsumerSecret, accessToken,
														 new Twitter.PostTweetCallback(this.OnPostTweet)));
			}

			// TODO inform if no tweeter
		}

		private void SendMail()
		{
			if (mXMLData != null) {
				string lAdress = mXMLData.AdressMailSender;
				string lPasswordMail = mXMLData.PasswordMail;
				mMail.AddTexture2D(LoadTexture(mTexture), TakePhotoData.Instance.PhotoPath);
				mMail.Addresses.Add(mXMLData.AdressMailReceiver);
				mMail.Subject = mXMLData.SubjectMail;
				mMail.Body = mXMLData.BodyMail;
				Buddy.WebServices.EMailSender.Send(lAdress, lPasswordMail, SMTP.GMAIL, mMail, iInput => OnMailSent(iInput));
			}

			// TODO inform if no mail
		}

		private void OnMailSent(bool iInput)
		{
			if (iInput) {
				mMailOrTweetSent = true;
			}
		}

		private void OnPostTweet(bool success)
		{
			Debug.Log("OnPostTweet - " + (success ? "succeeded." : "failed."));
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