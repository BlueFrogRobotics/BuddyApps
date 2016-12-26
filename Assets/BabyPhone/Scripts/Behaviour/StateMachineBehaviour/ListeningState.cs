using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;
using BuddyFeature.Web;
using System;

namespace BuddyApp.BabyPhone
{
    //écoute les eventuels bruits, et gère l'écran de vielle, si détecte un bruit, il envois un message
    public class ListeningState : AStateMachineBehaviour
    {
        private BabyPhoneData mBabyPhoneData;

        private GameObject mListening;
        private GameObject mWindoAppOverBlack;

        //private Button mNotificationsButton;
        //private Text mNotificationAmount;

        private InputMicro mMicro;

        private bool mIsBabyCrying;

        private int mCountNotification;
        private int mCount;
        private float mSound;
        private float mMean;
        private float mMicroSensitivty;
        private int mContactIndice;
        private string mBabyName;

        public override void Init()
        {
            mBabyPhoneData = BabyPhoneData.Instance;

            mWindoAppOverBlack = GetGameObject(2);
            mListening = GetGameObject(9);

            //mNotificationsButton = GetGameObject(23).GetComponent<Button>();
            //mNotificationAmount = GetGameObject(24).GetComponent<Text>();

            mMicro = mListening.GetComponent<InputMicro>();
            mContactIndice = (int) mBabyPhoneData.Recever;
            mBabyName = mBabyPhoneData.BabyName;
            mMicroSensitivty = mBabyPhoneData.MicrophoneSensitivity;
            mCount = 0;
            mMean = 0F;

            mIsBabyCrying = false;
            mCountNotification = 0;

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mListening.SetActive(true);
            mWindoAppOverBlack.SetActive(true);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mListening.SetActive(false);
            mWindoAppOverBlack.SetActive(false);

            iAnimator.SetInteger("ForwardState", 4);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if ((mIsBabyCrying))
            {
                StartCoroutine(SetSadMood());

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
                StartCoroutine(SetListenMood());

                mSound = mMicro.Loudness;
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

        private IEnumerator SetSadMood()
        {
            yield return new WaitForSeconds(0.5F);
            mFace.SetExpression(MoodType.SAD);
        }

        private void BuddyListen()
        {
            StartCoroutine(SetListenMood());

        }

        private IEnumerator SetListenMood()
        {
            yield return new WaitForSeconds(0.5F);
            mFace.SetExpression(MoodType.LISTENING);
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
