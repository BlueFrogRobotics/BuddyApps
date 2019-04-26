using UnityEngine;
using BlueQuark;
using System.IO;
using System.Collections;

namespace BuddyApp.Shared
{
    public enum PublishShared : int
    {
        TWITTER,
        MAIL,
        BOTH
    }; 

    [SerializeField]
    public sealed class XMLDataShared
    {
        public PublishShared WhereToPublish { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string ConsumerKey { get; set; } 
        public string ConsumerSecret { get; set; }
        public bool UseKey { get; set; }
        public string TwitterText { get; set; }
        public string TwitterHashtag { get; set; }
        public string AdressMailReceiver { get; set; }
        public string AdressMailSender { get; set; }
        public string PasswordMail { get; set; }
        public string SubjectMail { get; set; }
        public string BodyMail { get; set; }
    }

    /// <summary>
    /// You have to use the public static function SetPhotograph on the state before this Shared state to show the photo and share it on twitter/mail
    /// </summary>
    public sealed class SharedSend : ASharedSMB
    {
        //First sentence said by the robot
        [SerializeField]
        private bool AskQuestion;
        [SerializeField]
        private string QuestionKey;

        //name of the config file for twitter/mail parameters
        [SerializeField]
        private string FilenameOfConfig;

        //Display a picture at the same time when Buddy is talking
        [SerializeField]
        private bool DisplayPhotoToShare;

        [SerializeField]
        private string UtteranceWantShare;
        [SerializeField]
        private string UtteranceDontWantShare;

        [SerializeField]
        private string TriggerWhenShared;
        [SerializeField]
        private string TriggerWhenNotShared;

        [SerializeField]
        private bool QuitAppAfterNumberOfListen;
        [SerializeField]
        private int NumberOfListeningBeforeQuit;

        //Part of the config.xml
        private string mToken;
        private string mTokenSecret;
        private string mConsumerKey;
        private string mConsumerSecret;
        private string mAllHashtag;
        private PublishShared mWhereToPublish;
        private string[] mTweetMsg;
        private EMail mMail;
        private bool mUseKey;

        private static Photograph mPhotographToShare;
        private Texture2D mTexture;

        private int mNumberOfListen;

        private XMLDataShared mXMLData = null;

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mNumberOfListen = 0;
            mMail = new EMail();
            mTexture = new Texture2D(1, 1);

            if (QuitAppAfterNumberOfListen && NumberOfListeningBeforeQuit == 0)
            {
                NumberOfListeningBeforeQuit = 1;
            }

            if (File.Exists(Buddy.Resources.GetRawFullPath("Shared/" + FilenameOfConfig)))
            {
                mXMLData = new XMLDataShared();
                mXMLData = Utils.UnserializeXML<XMLDataShared>(Buddy.Resources.GetRawFullPath("Shared/" + FilenameOfConfig));

                if (mXMLData == null)
                    Debug.Log("null xml data");
                mToken = mXMLData.Token;
                mTokenSecret = mXMLData.TokenSecret;
                mConsumerKey = mXMLData.ConsumerKey;
                mConsumerSecret = mXMLData.ConsumerSecret;
                mUseKey = mXMLData.UseKey;
                mAllHashtag = mXMLData.TwitterHashtag;
                mTweetMsg = mXMLData.TwitterText.Split('/');
                mWhereToPublish = mXMLData.WhereToPublish;
            }
            else
            {
                ExtLog.E(ExtLogModule.APP, GetType(),
                           LogStatus.FAILURE, LogInfo.ACCESSING,
                           "Couldn't access to the file Config.xml, please verify if Config.xml exists in the folder Assets\"\"Apps\"\"YourApp\"\"Resources\"\"Raw\"\"Share");
            }


            //Buddy asks a question at the begining
            if (AskQuestion && !string.IsNullOrEmpty(QuestionKey))
            {
                if (!SharedVocalFunctions.ContainsSpecialChar(QuestionKey) && !SharedVocalFunctions.ContainsWhiteSpace(QuestionKey))
                {
                    Buddy.Vocal.SayKeyAndListen(QuestionKey.ToLower(), null, (iInput) => { OnEndListen(iInput); }, null, SpeechRecognitionMode.FREESPEECH_ONLY);
                }
                else if (SharedVocalFunctions.ContainsSpecialChar(QuestionKey) || SharedVocalFunctions.ContainsWhiteSpace(QuestionKey))
                {
                    Buddy.Vocal.SayAndListen(QuestionKey, null, (iInput) => { OnEndListen(iInput); });
                }
            }
            
            //check if the path to the picture is not null and display it
            if (DisplayPhotoToShare && !string.IsNullOrEmpty(mPhotographToShare.FullPath))
            {
                Buddy.GUI.Toaster.Display<PictureToast>().With(mPhotographToShare.FullPath);
            }
            else if (DisplayPhotoToShare && string.IsNullOrEmpty(mPhotographToShare.FullPath))
            {
                ExtLog.E(ExtLogModule.APP, GetType(),
                           LogStatus.FAILURE, LogInfo.NULL_VALUE,
                           "You want to display a picture but your path is empty or not valid.");
            }

            FButton LRightButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            LRightButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_share"));
            LRightButton.OnClick.Add(() => { OnButtonShare(); });
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Vocal.Stop();
            Buddy.GUI.Footer.Hide();
            Buddy.GUI.Toaster.Hide();
        } 

        public static void SetPhotograph(Photograph iPhotograph)
        {
            mPhotographToShare = iPhotograph;
        }

        private void OnButtonShare()
        {
            Share();
        }

        private void SendTweet(string iMsg)
        {
            if (mXMLData != null)
            {
                Twitter.AccessTokenResponseShared accessToken = new Twitter.AccessTokenResponseShared
                {
                    Token = mToken,
                    TokenSecret = mTokenSecret
                };
                StartCoroutine(Twitter.APIShared.UploadMedia(LoadTexture(mTexture), iMsg, mConsumerKey, mConsumerSecret, accessToken,
                                                         new Twitter.PostTweetCallbackShared(OnPostTweet)));
            }
            
        }

        private void SendMail()
        {
            if (mXMLData != null)
            {
                string lAdress = mXMLData.AdressMailSender;
                string lPasswordMail = mXMLData.PasswordMail;
                mMail.AddTexture2D(LoadTexture(mTexture), mPhotographToShare.FullPath);
                mMail.Addresses.Add(mXMLData.AdressMailReceiver);
                mMail.Subject = mXMLData.SubjectMail;
                mMail.Body = mXMLData.BodyMail;
                Buddy.WebServices.EMailSender.Send(lAdress, lPasswordMail, SMTP.GMAIL, mMail, iInput => OnMailSent(iInput));
            }
            
        }

        private void OnPostTweet(bool success)
        {
            Debug.Log("OnPostTweet - " + (success ? "succeeded." : "failed."));
        }

        private void OnMailSent(bool iInput)
        {
            Debug.Log("Mail Sent");
        }

        private Texture2D LoadTexture(Texture2D iText)
        {
            byte[] bytes = new byte[File.ReadAllBytes(mPhotographToShare.FullPath).Length];
            bytes = File.ReadAllBytes(mPhotographToShare.FullPath);
            iText.LoadImage(bytes);
            return iText;
        }

        private void OnEndListen(SpeechInput iInput)
        {
            if (iInput.IsInterrupted)
                return;
            if (object.Equals(Buddy.Vocal.LastHeardInput.Utterance, UtteranceWantShare))
            {
                Share();
                Trigger(TriggerWhenShared);
            }
            else if (object.Equals(Buddy.Vocal.LastHeardInput.Utterance, UtteranceDontWantShare))
            {
                Trigger(TriggerWhenNotShared);
            }
            else
            {
                if (mNumberOfListen < NumberOfListeningBeforeQuit)
                {
                    mNumberOfListen++;
                    Buddy.Vocal.Listen(
                        iInputRec => { OnEndListen(iInputRec); }
                        );
                }
                else
                {
                    QuitApp();
                }
            }
        }

        private void Share()
        {
            if (!Buddy.Vocal.IsSpeaking)
            {
                mMail.Addresses.Clear();
                string lMessage;
                lMessage = mTweetMsg[Random.Range(0, mTweetMsg.Length)];
                lMessage += " " + mAllHashtag;
                
                if (mWhereToPublish == PublishShared.MAIL)
                {
                    SendMail();
                }
                else if (mWhereToPublish == PublishShared.TWITTER)
                {
                    SendTweet(lMessage);
                }
                else
                {
                    SendTweet(lMessage);
                    SendMail();
                }
            }
        }
    }
}

