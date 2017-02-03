using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;
using BuddyFeature.Web;
using System;

namespace BuddyApp.BabyPhone
{
    public class FallingAssleepState : AStateMachineBehaviour
    {
        private GameObject mFallingAssleep;
        private GameObject mWindoAppOverWhite;
        private GameObject mCartoonObject;
        private GameObject mNotifications;

        private Animator mBackgroundBlackAnimator;

        private Text mNotificationText;
        private SoundDetect mSoundDetector;

        private Animator mFallingAssleepAnimator;
        private Animator mCartoonAnimator;

        private bool mIsBabyCrying;
        private bool isAnimationOn;
        private bool mIsSoundDetectionOn;
        private bool mIsMotionDetection;
        private bool mDidISend;

        private int  mNotificationsCount;
        private int mCartoonChoice;

        public override void Init()
        {
            mWindoAppOverWhite = GetGameObject(3);
            mFallingAssleep = GetGameObject(7);
            mCartoonObject = GetGameObject(12);
            mNotifications = GetGameObject(15); //white
            mNotificationText = GetGameObject(16).GetComponent<Text>();
            mFallingAssleepAnimator = mFallingAssleep.GetComponent<Animator>();
            mBackgroundBlackAnimator = GetGameObject(1).GetComponent<Animator>();
            mCartoonAnimator = mCartoonObject.GetComponent<Animator>();
            mSoundDetector = GetGameObject(17).GetComponent<SoundDetect>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindoAppOverWhite.SetActive(true);
            mFallingAssleep.SetActive(true);

            mFallingAssleepAnimator.SetTrigger("Open_WFallingAssleep");

            mBackgroundBlackAnimator.SetTrigger("Close_BG");

            mFallingAssleep.GetComponent<SoundDetect>().Init();

            iAnimator.SetBool("DoPlayLullaby", true);

            mMood.Set(MoodType.HAPPY);
            mIsBabyCrying = false;
            mDidISend = false;

            if (!mSoundDetector.IsInit)
                mSoundDetector.Init();

            mIsSoundDetectionOn = BabyPhoneData.Instance.IsSoundDetectionOn;
            isAnimationOn = BabyPhoneData.Instance.IsAnimationOn;
            mCartoonChoice = (int)BabyPhoneData.Instance.AnimationToPlay;

            if (isAnimationOn)
            {
                mCartoonObject.SetActive(true);
                mCartoonAnimator.SetBool("IsPlaying", true);
                if (mCartoonChoice == 0)
                    mCartoonAnimator.SetTrigger("Hibou");
                else
                    mCartoonAnimator.SetTrigger("Chrsitmas"); // penser a corriger 
            }

            mNotificationsCount = iAnimator.GetInteger("NotificationsCounts");
            if (mNotificationsCount > 0)
            {
                //Debug.Log("cout notifications : " + lCountNotifications);
                mNotifications.SetActive(true);
                mNotificationText.text = mNotificationsCount.ToString();
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mFallingAssleep.SetActive(false);
            mFallingAssleepAnimator.SetTrigger("Close_WFallingAssleep");
            mFallingAssleep.GetComponent<SoundDetect>().Stop();
            mWindoAppOverWhite.SetActive(false);

            iAnimator.SetBool("DoPlayLullaby", false);
            iAnimator.SetInteger("ForwardState", 3);

            mCartoonObject.SetActive(false);
            mCartoonAnimator.SetBool("IsPlaying", false);
            mRGBCam.Close();

            mNotifications.SetActive(false);

            mSoundDetector.Stop();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //if(mIsMotionDetection)
            //  mIsBabyMoving = GetComponent<MotionDetector>().IsMoving();

            if (mIsSoundDetectionOn)
                mIsBabyCrying = mSoundDetector.IsASoundDetected;

            if ((mIsBabyCrying) && (!mDidISend))
            {
                StartCoroutine(SadFace());
                StartCoroutine(SendMessage());
                iAnimator.SetInteger("NotificationsCounts", mNotificationsCount + 1 );
                mDidISend = true;

                //update notification count
                mNotificationsCount = iAnimator.GetInteger("NotificationsCounts");
                if (mNotificationsCount >= 1)
                {
                    //Debug.Log("cout notifications : " + lCountNotifications);
                    mNotifications.SetActive(true);
                    mNotificationText.text = mNotificationsCount.ToString();
                }
            }  
    }

        private IEnumerator SendMessage()
        {
            string lBabyName;
            int lContactIndice;

            lBabyName = BabyPhoneData.Instance.BabyName;
            lContactIndice = (int)BabyPhoneData.Instance.Recever;

            string lSentMessage = lBabyName + " " + mDictionary.GetString("msgbbsnd") + " :( !";

            mRGBCam.Open();
            yield return new WaitForSeconds(1.5F);

            MailSender lSender = new MailSender("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL);
            Mail lEmail = new Mail("[BUDDY] ALERT from BABYPHONE", lSentMessage);
            lEmail.Addresses.Add(GetMailContact(lContactIndice));
            lEmail.AddTexture2D(mRGBCam.FrameTexture2D, "image.png");
            lSender.Send(lEmail);

            yield return new WaitForSeconds(1.5F);
            mRGBCam.Close();
        }
        private String GetMailContact(int iContact)
        {
            string lMailContact;
            switch (iContact)
            {
                case 0:
                    lMailContact = "buddy.bluefrog@gmail.com";
                    break;
                case 1:
                    lMailContact = "rh@bluefrogrobotics.com";
                    break;
                case 2:
                    lMailContact = "jmm@bluefrogrobotics.com";
                    break;
                case 3:
                    lMailContact = "mv@bluefrogrobotics.com";
                    break;
                case 4:
                    lMailContact = "bp@bluefrogrobotics.com";
                    break;
                case 5:
                    lMailContact = "mg@bluefrogrobotics.com";
                    break;
                case 6:
                    lMailContact = "karama.guimbal@gmail.com";
                    break;
                case 7:
                    lMailContact = "fdv@bluefrogrobotics.com";
                    break;
                default:
                    lMailContact = "buddy.bluefrog@gmail.com";
                    break;
            }
            return lMailContact;
        }
        private IEnumerator SadFace()
        {
            mMood.Set(MoodType.SAD);
            yield return new WaitForSeconds(3F);
        }
    }
}
