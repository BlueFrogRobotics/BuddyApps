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
        private const int DETECTION_TIME = 30;
        private GameObject mFallingAssleep;
        private GameObject mWindoAppOverWhite;

        private Animator mFallingAssleepAnimator;

        private InputMicro mInputMicro;

        private float mSound;
        private float mMean;
        private float mMicroSensitivty;
        private bool mIsBabyCrying;
        private int mContactIndice;
        private string mBabyName;
        private float mElapsedTime;
        private bool mDidISend;

        public override void Init()
        {
            mWindoAppOverWhite = GetGameObject(3);
            mFallingAssleep = GetGameObject(7);
            mFallingAssleepAnimator = mFallingAssleep.GetComponent<Animator>();

            mInputMicro = mFallingAssleep.GetComponent<InputMicro>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindoAppOverWhite.SetActive(true);
            mFallingAssleep.SetActive(true);
            mFallingAssleepAnimator.SetTrigger("Open_WFallingAssleep");

            iAnimator.SetBool("DoPlayLullaby", true);

            mMean = 0F;
            mIsBabyCrying = false;
            mDidISend = false;

            mMicroSensitivty = BabyPhoneData.Instance.MicrophoneSensitivity;
            mContactIndice = (int)BabyPhoneData.Instance.Recever;
            mBabyName = BabyPhoneData.Instance.BabyName;
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mFallingAssleep.SetActive(false);
            mFallingAssleepAnimator.SetTrigger("Close_WFallingAssleep");

            mWindoAppOverWhite.SetActive(false);

            iAnimator.SetBool("DoPlayLullaby", false);
            iAnimator.SetInteger("ForwardState", 3);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mElapsedTime += Time.deltaTime;

            if (mElapsedTime <= DETECTION_TIME)
            {
                mSound = mInputMicro.Loudness;
                mMean += mSound;
            }
            else
            {
                mMean = mMean / 5F;
                if (mMean >= 0.1F)
                    mIsBabyCrying = true;
                else
                    mIsBabyCrying = false;
                mMean = 0;
                mElapsedTime = 0;
            }

            if ((mIsBabyCrying) && (!mDidISend))
            {
                StartCoroutine(SendMessage());
                mDidISend = true;
                iAnimator.SetTrigger("GoToBabyIsCrayingState");
            }
    }

        private IEnumerator SendMessage()
        {
            mRGBCam.Open();
            yield return new WaitForSeconds(1.5F);
            MailSender lSender = new MailSender("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL);
            Mail lEmail = new Mail("[BUDDY] ALERT from BABYPHONE", mBabyName + " " + mDictionary.GetString("msgbb")+" :( !");
            lEmail.Addresses.Add(GetMailContact(mContactIndice));
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
