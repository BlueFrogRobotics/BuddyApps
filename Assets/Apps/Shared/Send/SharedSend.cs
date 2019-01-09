using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Shared
{
    public enum Publish : int
    {
        TWITTER,
        MAIL,
        BOTH
    };

    //[SerializeField]
    public sealed class XMLData
    {
        public Publish WhereToPublish { get; set; }
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
        private string PicturePath;

        [SerializeField]
        private bool OnlyVocal;


        public override void Start()
        {

          
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //Buddy asks a question at the begining
            if (AskQuestion && !string.IsNullOrEmpty(QuestionKey))
            {
                if (SharedVocalFunctions.ContainsSpecialChar(QuestionKey) || SharedVocalFunctions.ContainsWhiteSpace(QuestionKey))
                {
                    Buddy.Vocal.SayKey(QuestionKey.ToLower());
                }
                else
                {
                    Buddy.Vocal.Say(Buddy.Resources.GetRandomString(QuestionKey));
                }
            }

            //check if the path to the picture is not null and display it
            if(DisplayPhotoToShare && !string.IsNullOrEmpty(PicturePath))
            {
                Buddy.GUI.Toaster.Display<PictureToast>().With(Buddy.Resources.Get<Sprite>(PicturePath));
            }
            else if(string.IsNullOrEmpty(PicturePath))
            {
                ExtLog.E(ExtLogModule.APP, GetType(),
                           LogStatus.FAILURE, LogInfo.NULL_VALUE,
                           "You want to display a picture but your path is empty or not valid.");
            }

            //Check if the dev wants only vocal or if he wants to display the footer with a button share
            if(!OnlyVocal)
            {
                FButton LRightButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
                LRightButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_share"));
                LRightButton.OnClick.Add(() => { OnButtonShare(); });
            }
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        private void OnButtonShare()
        {
            Debug.Log("Share");
        }
    }
}

