using BlueQuark;

using UnityEngine;

using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public class SharePhotoState : AStateMachineBehaviour
    {
        private readonly string STR_TWEET_TEXT = "tweet";
        private readonly string STR_MAIL_SUBJECT = "emailsubject";
        private readonly string STR_MAIL_TEXT = "emailtext";
        private readonly string STR_TWITTER_ERROR = "errorconnexion";
        private readonly string STR_SHARED = "pictureshared";

        private XMLData mXMLData;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On State Enter...");

            mXMLData = Utils.UnserializeXML<XMLData>(Buddy.Resources.GetRawFullPath("Share/config.xml"));
            
            if (null == mXMLData)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.NULL_VALUE, "Failed to read configuration file.");
                Buddy.Vocal.SayKey(STR_TWITTER_ERROR, false);
                Trigger("TRIGGER_PHOTO_SHARED");
                return;
            }
            
            switch (mXMLData.WhereToPublish)
            {
                case Publish.MAIL:
                    SendMail();
                    return;

                case Publish.TWITTER:
                    SendTweet();
                    return;

                case Publish.BOTH:
                    SendMail();
                    SendTweet();
                    return;
            }
            
            Trigger("TRIGGER_PHOTO_SHARED");
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");
        }

        private void SendTweet()
        {
            Twitter.AccessTokenResponse accessToken = new Twitter.AccessTokenResponse {
                Token = mXMLData.Token,
                TokenSecret = mXMLData.TokenSecret
            };

            string strImageFullPath = PhotoManager.GetInstance().GetCurrentPhoto().GetPhotoFullpath();
            FileInfo lFileInfo = new FileInfo(strImageFullPath);
            string strImageFileName = lFileInfo.Name + lFileInfo.Extension;

            string[] mTweetMsg = null;
            if (!string.IsNullOrEmpty(mXMLData.TwitterText)) {
                mTweetMsg = mXMLData.TwitterText.Split('/');
            }

            string lStrTweetMsg = (null == mTweetMsg)
                ? Buddy.Resources.GetRandomString(STR_TWEET_TEXT)
                : mTweetMsg[Random.Range(0, mTweetMsg.Length)];
            lStrTweetMsg += " " + mXMLData.TwitterHashtag;


            byte[] lImage = File.ReadAllBytes(strImageFullPath);
            ExtLog.W(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.NULL_VALUE, "Size of file to send : " + lImage.Length);

            StartCoroutine(Twitter.API.UploadMedia(strImageFileName, lImage, lStrTweetMsg, mXMLData.ConsumerKey, mXMLData.ConsumerSecret, accessToken,
                                                     new Twitter.PostTweetCallback(OnPostTweet)));
        }

        private void SendMail()
        {
            EMail lMail = new EMail();
            lMail.Addresses.Clear();
            lMail.Addresses.Add(mXMLData.AdressMailReceiver);
            lMail.Subject = string.IsNullOrEmpty(mXMLData.SubjectMail) ? Buddy.Resources.GetRandomString(STR_MAIL_SUBJECT) : mXMLData.SubjectMail;
            lMail.Body = string.IsNullOrEmpty(mXMLData.BodyMail) ? Buddy.Resources.GetRandomString(STR_MAIL_TEXT) : mXMLData.BodyMail;
            lMail.AddFile(PhotoManager.GetInstance().GetCurrentPhoto().GetPhotoFullpath());
            Buddy.WebServices.EMailSender.Send(mXMLData.AdressMailSender, mXMLData.PasswordMail, SMTP.GMAIL, lMail, OnPostEmail);
        }

        private void OnPostTweet(bool iBSuccess)
        {
            if (!iBSuccess)
            {
                Buddy.Vocal.SayKey(STR_TWITTER_ERROR, false);
            }
            else
            {
                Buddy.Vocal.SayKey(STR_SHARED, false);
            }

            Trigger("TRIGGER_PHOTO_SHARED");
        }

        private void OnPostEmail(bool iBSuccess)
        {
            if (!iBSuccess) {
                Buddy.Vocal.SayKey(STR_TWITTER_ERROR, false);
            }
            else
            {
                Buddy.Vocal.SayKey(STR_SHARED, false);
            }

            if (Publish.MAIL == mXMLData.WhereToPublish)
            {
                Trigger("TRIGGER_PHOTO_SHARED");
            }
        }
    }
}