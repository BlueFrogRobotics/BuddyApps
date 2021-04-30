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
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] Sharing On State Enter...");

            mXMLData = Utils.UnserializeXML<XMLData>(Buddy.Platform.Application.PersistentDataPath + "Shared/config.xml");
            
            if (null == mXMLData)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.NULL_VALUE, "[GALLERY APP] Failed to read configuration file.");
                // if user mail saved in settings
                if(string.IsNullOrEmpty(GalleryData.Instance.mailshare))
                {
                    Buddy.Vocal.SayKey(STR_TWITTER_ERROR, false);
                }
                else
                {
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] Sharing On State Enter...");
                    SendMail();
                }
                
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
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] Start sending mail...");

            EMail lMail = new EMail();
            lMail.Addresses.Clear();
            // if no user mail from settings
            if(string.IsNullOrEmpty(GalleryData.Instance.mailshare))
            {
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] mail adress from xml");
                lMail.Addresses.Add(mXMLData.AdressMailReceiver);
            }
            else
            {
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] mail adress from settings");
                lMail.Addresses.Add(GalleryData.Instance.mailshare);
            }

            // checking xml file
            if (mXMLData == null)
            {
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] mail subject xml null");
                lMail.Subject = Buddy.Resources.GetRandomString(STR_MAIL_SUBJECT);
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] mail body xml null");
                lMail.Body = Buddy.Resources.GetRandomString(STR_MAIL_TEXT);
            }
            else
            {
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] mail subject");
                lMail.Subject = string.IsNullOrEmpty(mXMLData.SubjectMail) ? Buddy.Resources.GetRandomString(STR_MAIL_SUBJECT) : mXMLData.SubjectMail;
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] mail body");
                lMail.Body = string.IsNullOrEmpty(mXMLData.BodyMail) ? Buddy.Resources.GetRandomString(STR_MAIL_TEXT) : mXMLData.BodyMail;
            }
            
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] Adding file" + PhotoManager.GetInstance().GetCurrentPhoto().GetPhotoFullpath());
            lMail.AddFile(PhotoManager.GetInstance().GetCurrentPhoto().GetPhotoFullpath());

            // if the robot has not a configured mail address
            if (mXMLData != null)
            {
                if (string.IsNullOrEmpty(mXMLData.AdressMailSender) || string.IsNullOrEmpty(mXMLData.PasswordMail))
                {
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] sending mail from demo@buddytherobot.com");
                    Buddy.WebServices.EMailSender.Send("demo@buddytherobot.com", "DemoBuddy", SMTP.BFR, lMail, OnPostEmail);
                }
                else
                {
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] sending mail from " + mXMLData.AdressMailSender);
                    Buddy.WebServices.EMailSender.Send(mXMLData.AdressMailSender, mXMLData.PasswordMail, SMTP.GMAIL, lMail, OnPostEmail);
                }
            }
            else
            {
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[GALLERY APP] sending mail from demo@buddytherobot.com with null xml");
                Buddy.WebServices.EMailSender.Send("demo@buddytherobot.com", "DemoBuddy", SMTP.BFR, lMail, OnPostEmail);
            }

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