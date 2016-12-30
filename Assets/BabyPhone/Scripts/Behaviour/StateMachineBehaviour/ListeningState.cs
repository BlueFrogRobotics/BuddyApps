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
        private const int DETECTION_TIME = 5;

        private GameObject mListening;
        private GameObject mWindoAppOverBlack;

        //private Button mNotificationsButton;
        //private Text mNotificationAmount;

        private InputMicro mInputMicro;

        private bool mIsBabyCrying;
        private bool mIsBabyMoving;

        private float mSound;
        private float mMean;
        private float mMicroSensitivity;
        private int mContactIndice;
        private string mBabyName;

        private float mElapsedTime;
        public override void Init()
        {
            mWindoAppOverBlack = GetGameObject(2);
            mListening = GetGameObject(9);

            //mNotificationsButton = GetGameObject().GetComponent<Button>();
            //mNotificationAmount = GetGameObject().GetComponent<Text>();

            mInputMicro = mListening.GetComponent<InputMicro>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mListening.SetActive(true);
            mWindoAppOverBlack.SetActive(true);
 
            mMood.Set(MoodType.LISTENING);

            mContactIndice = (int)BabyPhoneData.Instance.Recever;
            mBabyName = BabyPhoneData.Instance.BabyName;
            mMicroSensitivity = ((BabyPhoneData.Instance.MicrophoneSensitivity)/100F);

            mMean = 0F;
            mIsBabyCrying = false;

            GetComponent<BabyPhoneMotionDetection>().enabled = true;
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mListening.SetActive(false);
            mWindoAppOverBlack.SetActive(false);

            iAnimator.SetInteger("ForwardState", 4);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mElapsedTime += Time.deltaTime;
            mIsBabyMoving = GetComponent<BabyPhoneMotionDetection>().IsMoving();
            if (mElapsedTime <= DETECTION_TIME)
            {
                
                mSound = mInputMicro.Loudness;
                mMean += mSound;
            }
            else
            {
                mMean = mMean / 5F;
                if (mMean >= mMicroSensitivity) //utiliser la sensibilité du micro
                    mIsBabyCrying = true;
                else
                    mIsBabyCrying = false;
                mMean = 0;
                mElapsedTime = 0;
            }

            if ((mIsBabyCrying) || (mIsBabyMoving))
            {
                StartCoroutine(SendMessage());
                iAnimator.SetTrigger("GoToBabyIsCrayingState");
            }
        }

        private IEnumerator SendMessage()
        {
            string lSentMessage = "";
            if (mIsBabyCrying)
                lSentMessage = mBabyName + " " + mDictionary.GetString("msgbbsnd") + " :( !";
            if(mIsBabyMoving)
                lSentMessage = mBabyName + " " + mDictionary.GetString("msgbbmvt") + " :( !";
            yield return new WaitForSeconds(1.5F);
            MailSender lSender = new MailSender("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL);
            Mail lEmail = new Mail("[BUDDY] ALERT from BABYPHONE", lSentMessage);
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
