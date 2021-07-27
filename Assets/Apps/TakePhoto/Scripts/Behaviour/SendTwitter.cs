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
                Debug.Log("[TAKEPHOTO APP] TakePhoto SendTwitter::Start Config file doesn't exist " + configFileName);
                configFileName = Buddy.Resources.GetRawFullPath("Twitter/config.xml");
                Debug.Log( "[TAKEPHOTO APP] set to twitter/config...");
                if (!File.Exists(configFileName))
                {
                    Debug.Log("[TAKEPHOTO APP] TakePhoto SendTwitter::Start Config file doesn't exist " + configFileName + " ");
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
            try
            { 
                Debug.Log("[TAKEPHOTO APP] send Twitter On State Enter...");
                bool success = false;
                if (mXMLData != null) {
                    //ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] xml file exists");
                    mMail.Addresses.Clear();
                    string lMessage = "";
                    if (mTweetMsg != null)
                    {
                        //ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] tweet msg exists");
                        lMessage = mTweetMsg[Random.Range(0, mTweetMsg.Length)];
                    }
                    //ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] where to publish " + mWhereToPublish.ToString());
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
                //if failed to share the picture
                if (!success)
                {
                    Buddy.Vocal.SayKey("errorconnexion");
                    Trigger("AskPhotoAgain");
                }
                // else picture share with success
                else
                {
                    Buddy.Vocal.Say("Super ! C'est partagé!");
                    Trigger("AskPhotoAgain");
                }
            }
                catch (System.Exception ex)
            {
                Debug.Log("[TAKEPHOTO APP] Caught exception " + ex.ToString());
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
            try
            {
                if (mXMLData != null
                    && !string.IsNullOrEmpty(mToken)
                    && !string.IsNullOrEmpty(mTokenSecret)
                    && !string.IsNullOrEmpty(mConsumerKey)
                    && !string.IsNullOrEmpty(mConsumerSecret))
                {
                    Twitter.AccessTokenResponse accessToken = new Twitter.AccessTokenResponse
                    {
                        Token = mToken,
                        TokenSecret = mTokenSecret
                    };

                    StartCoroutine(Twitter.API.UploadMedia(LoadTexture(mTexture), iMsg, mConsumerKey, mConsumerSecret, accessToken,
                                                             new Twitter.PostTweetCallback(this.OnPostTweet)));

                    return true;
                }
                // 28/04/2021 : Twitter is optional, always return true
                return true;
                // return false;
            }
            catch (System.Exception ex)
            {
                Debug.Log("[TAKEPHOTO APP] Caught exception " + ex.ToString());
                return false;
            }
        }

		private bool SendMail()
		{
            
            // sender address
            string mail_sender = string.Empty;
            // password
            string mail_password = string.Empty;
            // receiver address
            string mail_receiver = string.Empty;
            // mail subject
            string mail_subject = string.Empty;
            // mail body
            string mail_body = string.Empty;
            // SMTP service
            SMTP mail_smtp = 0;

            // init
            mMail.ClearFileAttachment();

            try
            {
                // if mail adress saved in the settings
                if(!string.IsNullOrEmpty(TakePhotoData.Instance.mailtoshare))
                {
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] found user mail in settings");
                    // assign receiver address
                    mail_receiver = TakePhotoData.Instance.mailtoshare;
                    mMail.Addresses.Add(mail_receiver);

                    // if xml config file present
                    if (mXMLData != null)
                    {
                        ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] mail in settings AND xml");

                        // check sender adress
                        if (!string.IsNullOrEmpty(mXMLData.AdressMailSender))
                        {
                            mail_sender = mXMLData.AdressMailSender;
                            // check password
                            if (!string.IsNullOrEmpty(mXMLData.PasswordMail))
                            {
                                mail_password = mXMLData.PasswordMail;
                                mail_smtp = SMTP.GMAIL;
                            }
                            else // if password NOK, take the default address to send the mail
                            {
                                mail_sender = "demo@buddytherobot.com";
                                mail_password = "DemoBuddy";
                                mail_smtp = SMTP.BFR;
                            }
                        }
                        else // if address NOK, take the default address to send the mail
                        {
                            mail_sender = "demo@buddytherobot.com";
                            mail_password = "DemoBuddy";
                            mail_smtp = SMTP.BFR;
                        }

                        // check mail subject
                        if (!string.IsNullOrEmpty(mXMLData.AdressMailSender))
                        {
                            mMail.Subject = mXMLData.SubjectMail;
                        }
                        else
                        {
                            mMail.Subject = Buddy.Resources.GetString("emailsubject");
                        }

                        // check mail body
                        if (!string.IsNullOrEmpty(mXMLData.BodyMail))
                        {
                            mMail.Body = mXMLData.BodyMail;
                        }
                        else
                        {
                            mMail.Body = Buddy.Resources.GetString("emailbody");
                        }

                        // add the picture in attachement
                        mMail.AddTexture2D(LoadTexture(mTexture), TakePhotoData.Instance.PhotoPath);
                    }
                    // saved user mail address, but no xml config file
                    else
                    {
                        ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] found user mail in settings BUT NO xml");

                        // assign receiver address
                        mail_receiver = TakePhotoData.Instance.mailtoshare;
                        mMail.Addresses.Add(mail_receiver);
                        // use the default send address
                        mail_sender = "demo@buddytherobot.com";
                        mail_password = "DemoBuddy";
                        mail_smtp = SMTP.BFR;
                        // mail subject and body
                        mMail.Subject = Buddy.Resources.GetString("emailsubject");
                        mMail.Body = Buddy.Resources.GetString("emailbody");
                        // add the picture in attachement
                        mMail.AddTexture2D(LoadTexture(mTexture), TakePhotoData.Instance.PhotoPath);

                        }
                    }
                //else if no mail address saved in the settings
                else
                {
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] Found NO user mail in settings");

                    // if xml file present and params OK
                    if (mXMLData != null
                        && !string.IsNullOrEmpty(mXMLData.AdressMailSender)
                        && !string.IsNullOrEmpty(mXMLData.PasswordMail)
                        && !string.IsNullOrEmpty(mXMLData.AdressMailReceiver))
                    {
                        ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] Found NO user mail in settings but xml OK");
                        // assign receiver address
                        mail_receiver = mXMLData.AdressMailReceiver;
                        mMail.Addresses.Add(mail_receiver);
                        // use the default send address
                        mail_sender = mXMLData.AdressMailSender;
                        mail_password = mXMLData.PasswordMail;
                        mail_smtp = SMTP.BFR;
                        // check mail subject
                        if (!string.IsNullOrEmpty(mXMLData.AdressMailSender))
                            mMail.Subject = mXMLData.SubjectMail;
                        else
                            mMail.Subject = Buddy.Resources.GetString("emailsubject");
                        // check mail body
                        if (!string.IsNullOrEmpty(mXMLData.BodyMail))
                            mMail.Body = mXMLData.BodyMail;
                        else
                            mMail.Body = Buddy.Resources.GetString("emailbody");
                        // add the picture in attachement
                        mMail.AddTexture2D(LoadTexture(mTexture), TakePhotoData.Instance.PhotoPath);

                    }
                    // else no user mail address saved in settings, nor xml config file
                    else
                    {
                        ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] Found NO user mail in settings NOR xml");

                        // fail to send email
                        return false;
                    }
                }

                // send email
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] sending mail now " 
                    + " from " + mail_sender
                    + " to " + mail_receiver);

                /*ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "[TAKEPHOTO APP] params "
                   + " PWD " + mail_password
                   + " SMTP " + mail_smtp.ToString() 
                   + " subject " + mMail.Subject
                   + " Phto " + TakePhotoData.Instance.PhotoPath);*/

                Buddy.WebServices.EMailSender.Send(mail_sender, mail_password, mail_smtp, mMail, iInput => OnMailSent(iInput));
                return true;

            }
            catch (System.Exception ex)
            {
                Debug.Log("[TAKEPHOTO APP] Caught exception " + ex.ToString());
                return false;
            }
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