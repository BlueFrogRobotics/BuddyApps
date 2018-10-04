using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using BlueQuark;

namespace BuddyApp.TakePhoto
{
    public sealed class SendTwitter : AStateMachineBehaviour
	{
		private bool mNeedListen;
		private bool mFirst;

		bool mPressedYes;
		bool mPressedNo;

		private string mLastSpeech;
		private short mErrorCount;

        //robotHuehuehue
        //private const string mToken = "3107499058-DtOkSKQVm9aXk7g8DsT9ZyNKeixWCdQ5bnkuB5y";
        //private const string mTokenSecret = "tszMyp6cFjeBb9k9raT7fxuHTCsw0g70eiMhJOmZYeJAG";
        //private const string mConsumerKey = "HbjgvAlxXb4F9vPcDHKtxOC6t";
        //private const string mConsumerSecret = "PQQrjxJcTs40QA9h5Rwr8rpQuoMp1J6gexgfjNXfJS8wTlC1Ey";

        //buddyrobotEvent
        //private const string mToken = "815872173902598144-hkmlCGIoFySurjBNltb0fRR1LTJCiRM";
        //private const string mTokenSecret = "cE7Ncw8GkRLXzU3Xy0LEAm0u2hXoWSkJOp45rYdKlMkUM";
        //private const string mConsumerKey = "kHvJ5gVnhex7zKcGbB1456Wrz";
        //private const string mConsumerSecret = "BZnxvSyYgo7fRoBYtI8WWfKH9G8lfhVeJo3KdfTUW7ZdekkTdI";

        private string mToken;
        private string mTokenSecret; 
        private string mConsumerKey;
        private string mConsumerSecret;
        private string mAllHashtag;
        private Publish mWhereToPublish;
        private string[] mTweetMsg;
        private EMail mMail;
        

        private int mRandom;
        private Texture2D mTexture;
        private TakePhotoBehaviour mTakePhotoBH;

        private XMLData mTwitterData;

        public override void Start()
        {
            mRandom = 0;
            mTwitterData = new XMLData();
            mMail = new EMail();
            mTexture = new Texture2D(1, 1);
            mTwitterData = Utils.UnserializeXML<XMLData>(Buddy.Resources.GetRawFullPath("Twitter/config.xml"));
            Debug.Log("KIKOO : " + mTwitterData.WhereToPublish);

            mToken = mTwitterData.Token;
            mTokenSecret = mTwitterData.TokenSecret;
            mConsumerKey = mTwitterData.ConsumerKey;
            mConsumerSecret = mTwitterData.ConsumerSecret;

            mAllHashtag = mTwitterData.TwitterHashtag;
            mTweetMsg = mTwitterData.TwitterText.Split('/');

        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMail.Addresses.Clear();
            string lMessage;
            lMessage = mTweetMsg[UnityEngine.Random.Range(0, mTweetMsg.Length)];
            lMessage += " " + mAllHashtag;
            //Debug.Log("TOKEN : " + mToken + " TOKEN SECRET : " + mTokenSecret + " CONSUMER KEY : " + mConsumerKey + " CONSUMER SECRET : " + mConsumerSecret + " HASHTAG : " + mAllHashtag + " TEXT : " + lTweetMsg);
            //Buddy.GUI.Notifier.Display<SimpleNotification>().With(mAllHashtag, Buddy.Resources.Get<Sprite>("Ico_Twitter"));
            Debug.Log("AVANT MAIL");
            SendMail();
            Debug.Log("APRES MAIL");
            // SendTweet(lTweetMsg);
            //Buddy.Vocal.SayKey("tweetpublished", true);
            //Buddy.Vocal.Say(mAllHashtag, true);

            //Trigger("AskPhotoAgain");
        }

        

		private void SendTweet(string iMsg)
		{
            Twitter.AccessTokenResponse accessToken = new Twitter.AccessTokenResponse
            {
                Token = mToken,
                TokenSecret = mTokenSecret
            };

            StartCoroutine(Twitter.API.UploadMedia(LoadTexture(mTexture), iMsg, mConsumerKey, mConsumerSecret, accessToken,
                                                     new Twitter.PostTweetCallback(this.OnPostTweet)));
        }

        private void SendMail()
        {
            string lAdress = mTwitterData.AdressMailSender;
            string lPasswordMail = mTwitterData.PasswordMail;
            ////mMail.AddTexture2D(LoadTexture(mTexture) , mTakePhotoBH.PhotoPath);
            //mMail.Addresses.Add(mTwitterData.AdressMailReceiver);
            //mMail.Subject = mTwitterData.SubjectMail;
            //mMail.Body = mTwitterData.BodyMail;
            EMail lMail = new EMail(mTwitterData.SubjectMail, mTwitterData.BodyMail, mTwitterData.AdressMailReceiver);
            Debug.Log("sender : " + mTwitterData.AdressMailSender + " pass : " + mTwitterData.PasswordMail + " subject : " + mTwitterData.SubjectMail + " body : " + mTwitterData.BodyMail + " receiver : " + mTwitterData.AdressMailReceiver);
            Buddy.WebServices.EMailSender.Send(lAdress, lPasswordMail, SMTP.GMAIL, lMail);
        }

        void OnPostTweet(bool success)
		{
			Debug.Log("OnPostTweet - " + (success ? "succeeded." : "failed."));
		}

        private Texture2D LoadTexture(Texture2D iText)
        {
            //byte[] bytes = new byte[File.ReadAllBytes(mTakePhotoBH.PhotoPath).Length];
            byte[] bytes = File.ReadAllBytes(mTakePhotoBH.PhotoPath);
            iText.LoadImage(bytes);
            return iText;
        }

        /// <summary>
        /// Function that will be called when the email has beent sent
        /// </summary>
        private void OnMailSent()
        {
            Debug.Log("EMAIL SENT");
            mMail = null;

        }


    }
}