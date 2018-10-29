using BlueQuark;

using UnityEngine;

using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public class SharePhotoState : AStateMachineBehaviour
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
        
        private Texture2D mTexture;

        private XMLData mXMLData;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mXMLData = new XMLData();
            mMail = new EMail();
            mTexture = new Texture2D(1, 1);
            mXMLData = Utils.UnserializeXML<XMLData>(Buddy.Resources.GetRawFullPath("Share/config.xml"));

            mToken = mXMLData.Token;
            mTokenSecret = mXMLData.TokenSecret;
            mConsumerKey = mXMLData.ConsumerKey;
            mConsumerSecret = mXMLData.ConsumerSecret;
            mUseKey = mXMLData.UseKey;
            mAllHashtag = mXMLData.TwitterHashtag;
            mTweetMsg = mXMLData.TwitterText.Split('/');
            mWhereToPublish = mXMLData.WhereToPublish;

            mMail.Addresses.Clear();
            string lMessage;
            lMessage = mTweetMsg[Random.Range(0, mTweetMsg.Length)];
            lMessage += " " + mAllHashtag;

            //Buddy.GUI.Notifier.Display<SimpleNotification>().With(mAllHashtag, Buddy.Resources.Get<Sprite>("Ico_Twitter"));

            switch(mWhereToPublish)
            {
                case Publish.MAIL:
                    SendMail();
                    break;

                case Publish.TWITTER:
                    SendTweet(lMessage);
                    break;

                case Publish.BOTH:
                    SendMail();
                    SendTweet(lMessage);
                    break;
            }

            //Buddy.Vocal.SayKey("tweetpublished", true);
            //Buddy.Vocal.Say(mAllHashtag, true);

            Trigger("TRIGGER_PHOTO_SHARED");
        }



        private void SendTweet(string iMsg)
        {
            Twitter.AccessTokenResponse accessToken = new Twitter.AccessTokenResponse {
                Token = mToken,
                TokenSecret = mTokenSecret
            };

            StartCoroutine(Twitter.API.UploadMedia(LoadTexture(mTexture), iMsg, mConsumerKey, mConsumerSecret, accessToken,
                                                     new Twitter.PostTweetCallback(this.OnPostTweet)));
        }

        private void SendMail()
        {
            string lAdress = mXMLData.AdressMailSender;
            string lPasswordMail = mXMLData.PasswordMail;
            mMail.AddTexture2D(LoadTexture(mTexture), PhotoManager.GetInstance().GetCurrentPhoto().GetFullPath());
            mMail.Addresses.Add(mXMLData.AdressMailReceiver);
            mMail.Subject = mXMLData.SubjectMail;
            mMail.Body = mXMLData.BodyMail;
            Buddy.WebServices.EMailSender.Send(lAdress, lPasswordMail, SMTP.GMAIL, mMail);
        }

        void OnPostTweet(bool success)
        {
            Debug.Log("OnPostTweet - " + (success ? "succeeded." : "failed."));
        }

        private Texture2D LoadTexture(Texture2D iText)
        {
            byte[] bytes = new byte[File.ReadAllBytes(PhotoManager.GetInstance().GetCurrentPhoto().GetFullPath()).Length];
            bytes = File.ReadAllBytes(PhotoManager.GetInstance().GetCurrentPhoto().GetFullPath());
            iText.LoadImage(bytes);
            return iText;
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}