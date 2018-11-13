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
                Buddy.Vocal.SayKey(STR_TWITTER_ERROR);
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
        
        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
        
        private void SendTweet()
        {
            Twitter.AccessTokenResponse accessToken = new Twitter.AccessTokenResponse {
                Token = mXMLData.Token,
                TokenSecret = mXMLData.TokenSecret
            };

            string strImageFullPath = PhotoManager.GetInstance().GetCurrentPhoto().GetPhotoFullpath();
            FileInfo f = new FileInfo(strImageFullPath);
            string strImageFileName = f.Name + f.Extension;

            string[] mTweetMsg = null;
            if (!string.IsNullOrEmpty(mXMLData.TwitterText)) {
                mTweetMsg = mXMLData.TwitterText.Split('/');
            }

            string strTweetMsg = (null == mTweetMsg)
                ? Buddy.Resources.GetRandomString(STR_TWEET_TEXT)
                : mTweetMsg[Random.Range(0, mTweetMsg.Length)];
            strTweetMsg += " " + mXMLData.TwitterHashtag;


            byte[] image = File.ReadAllBytes(strImageFullPath);
            ExtLog.W(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.NULL_VALUE, "Size of file to send : " + image.Length);

            StartCoroutine(Twitter.API.UploadMedia(strImageFileName, image, strTweetMsg, mXMLData.ConsumerKey, mXMLData.ConsumerSecret, accessToken,
                                                     new Twitter.PostTweetCallback(OnPostTweet)));
        }

        private void SendMail()
        {
            EMail mail = new EMail();
            mail.Addresses.Clear();
            mail.Addresses.Add(mXMLData.AdressMailReceiver);
            mail.Subject = string.IsNullOrEmpty(mXMLData.SubjectMail) ? Buddy.Resources.GetRandomString(STR_MAIL_SUBJECT) : mXMLData.SubjectMail;
            mail.Body = string.IsNullOrEmpty(mXMLData.BodyMail) ? Buddy.Resources.GetRandomString(STR_MAIL_TEXT) : mXMLData.BodyMail;
            mail.AddFile(PhotoManager.GetInstance().GetCurrentPhoto().GetPhotoFullpath());
            Buddy.WebServices.EMailSender.Send(mXMLData.AdressMailSender, mXMLData.PasswordMail, SMTP.GMAIL, mail, OnPostEmail);
        }

        void OnPostTweet(bool iSuccess)
        {
            if (!iSuccess) {
                Buddy.Vocal.SayKey(STR_TWITTER_ERROR);
            } else {
                Buddy.Vocal.SayKey(STR_SHARED);
            }

            Trigger("TRIGGER_PHOTO_SHARED");
        }

        void OnPostEmail(bool iSuccess)
        {
            if (!iSuccess) {
                Buddy.Vocal.SayKey(STR_TWITTER_ERROR);
            }
            else
            {
                if (Publish.MAIL == mXMLData.WhereToPublish)
                {
                    Buddy.Vocal.SayKey(STR_SHARED);
                    Trigger("TRIGGER_PHOTO_SHARED");
                }
            }
        }

    }
}