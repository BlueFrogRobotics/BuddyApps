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
        private BabyPhoneData mBabyPhoneData;

        private GameObject mFallingAssleep;
        private GameObject mWindoAppOverWhite;

        private Animator mFallingAssleepAnimator;

        private InputMicro mInputMicro;
        private int mCountNotification;
        private int mCount;
        private float mSound;
        private float mMean;
        private float mMicroSensitivty;
        private bool mIsBabyCrying;
        private int mContactIndice;
        private string mBabyName;

        public override void Init()
        {
            mBabyPhoneData = BabyPhoneData.Instance;

            mWindoAppOverWhite = GetGameObject(3);
            mFallingAssleep = GetGameObject(7);
            mFallingAssleepAnimator = mFallingAssleep.GetComponent<Animator>();

            mInputMicro = mFallingAssleep.GetComponent<InputMicro>();

            mCount = 0;
            mMean = 0F;

            mIsBabyCrying = false;
            mCountNotification = 0;

            mMicroSensitivty = mBabyPhoneData.MicrophoneSensitivity;
            mContactIndice = (int)mBabyPhoneData.Recever;
            mBabyName = mBabyPhoneData.BabyName;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindoAppOverWhite.SetActive(true);
            mFallingAssleep.SetActive(true);
            mFallingAssleepAnimator.SetTrigger("Open_WFallingAssleep");

            iAnimator.SetBool("DoPlayLullaby", true);
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
            if ((mIsBabyCrying))
            {
                mCountNotification = mCountNotification + 1;

                if (mCountNotification == 100)
                {
                    //GetGameObject(23).SetActive(true);
                    //mNotificationAmount.text = "1";
                    StartCoroutine(SendMessage());
                }
            }
            else
            {
                mSound = mInputMicro.Loudness;
                mMean += mSound;
                mCount = mCount + 1;

                if (mCount > mMicroSensitivty)
                {
                    mMean = mMean / mMicroSensitivty;
                    if (mMean >= 0.1F)
                        mIsBabyCrying = true;
                    else
                        mIsBabyCrying = false;
                    mMean = 0;
                    mCount = 0;
                }
            }

        }

        private IEnumerator SendMessage()
        {
            mRGBCam.Open();
            yield return new WaitForSeconds(1.5F);
            MailSender lSender = new MailSender("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL);
            Mail lEmail = new Mail("[BUDDY] ALERT from BABYPHONE", mBabyName + " seems to cry =(");
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
                default:
                    lMailContact = "buddy.bluefrog@gmail.com";
                    break;
            }
            return lMailContact;
        }
    }
}
