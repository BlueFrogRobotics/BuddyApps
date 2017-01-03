using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;
using BuddyFeature.Web;
using System;

namespace BuddyApp.BabyPhone
{
    public class ListeningState : AStateMachineBehaviour
    {
        private GameObject mListening;
        private GameObject mWindoAppOverBlack;

        private bool mIsBabyCrying;
        private bool mIsBabyMoving;


        private int mCountNotifications;

        public override void Init()
        {
            mWindoAppOverBlack = GetGameObject(2);
            mListening = GetGameObject(9);

            //mNotificationsButton = GetGameObject().GetComponent<Button>();
            //mNotificationAmount = GetGameObject().GetComponent<Text>();

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mListening.SetActive(true);
            mWindoAppOverBlack.SetActive(true);

            mIsBabyCrying = false;

            mCountNotifications = iAnimator.GetInteger("NotificationsCounts");
            mMood.Set(MoodType.LISTENING);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mListening.SetActive(false);
            mWindoAppOverBlack.SetActive(false);
            mListening.GetComponent<MotionDetector>().enabled = false;
            iAnimator.SetInteger("ForwardState", 4);
            if (mRGBCam.IsOpen)
                mRGBCam.Close();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mIsBabyMoving = GetComponent<MotionDetector>().IsMoving();

            mIsBabyCrying = mListening.GetComponent<SoundDetector>().isNoisy;

            //if ((mIsBabyCrying) || (mIsBabyMoving))
            if (mIsBabyCrying)
            {
                StartCoroutine(SendMessage());
                iAnimator.SetInteger("NotificationsCounts", mCountNotifications + 1);
                iAnimator.SetTrigger("GoToBabyIsCrayingState");
            }
        }

        private IEnumerator SendMessage()
        {
            string lBabyName;
            int lContactIndice;
            string lSentMessage = "";

            lBabyName = BabyPhoneData.Instance.BabyName;
            lContactIndice = (int)BabyPhoneData.Instance.Recever;
            if (mIsBabyCrying)
                lSentMessage = lBabyName + " " + mDictionary.GetString("msgbbsnd") + " :( !";
            if (mIsBabyMoving)
                lSentMessage = lBabyName + " " + mDictionary.GetString("msgbbmvt") + " :( !";

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
                default:
                    lMailContact = "buddy.bluefrog@gmail.com";
                    break;
            }
            return lMailContact;
        }
    }
}
