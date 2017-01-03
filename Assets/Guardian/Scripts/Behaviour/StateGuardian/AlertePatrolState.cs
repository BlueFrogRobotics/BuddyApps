using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Globalization;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class AlertePatrolState : AStateGuardian
    {

        private float mTimer = 5.0f;

        private TextToSpeech mTTS;
        private bool mHasAlerted;
        private bool mHasSentNotification = false;
        private BuddyFeature.Web.MailSender mMailSender = null;
        private RGBCam mWebcam;
        private int mCountPhoto = 0;
        private Mood mMood;
        private DetectionManager mDetectorManager;

        private GameObject mHaloPrefab;
        private GameObject mBackgroundPrefab;
        private Animator mBackgroundAnimator;
        private Animator mHaloAnimator;
        private Image[] mHaloImages;
        private Image mIcoMessage;
        private Text mMessage;
        private Sprite[] mListSpriteNotif;
        private GameObject mCounterTime;
        private GameObject mObjectButtonAskPassword;
        //private Dropdown mDropListContact;

        private Animator mAnimator;
        private Button mButtonPassword;
        private string mMailAdress = "";
        private Dictionary mDictionary;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetWindowAppOverBuddyColor(1);
            InitLink();
            mHasSentNotification = false;
            mBackgroundPrefab.GetComponent<Canvas>().enabled = true;
            //animator.SetBool("HasAlerted", false);
            mTTS = BYOS.Instance.TextToSpeech;
            mMood = BYOS.Instance.Mood;
            mWebcam = BYOS.Instance.RGBCam;
            mDictionary = BYOS.Instance.Dictionary;
            mTimer = 3.0f;
            //mTTS = new BuddyFeature.Vocal.TextToSpeech();
            mHasAlerted = false;
            if (mMailSender == null)
                mMailSender = new BuddyFeature.Web.MailSender("notif.buddy@gmail.com", "autruchemagiquebuddy", BuddyFeature.Web.SMTP.GMAIL);
                //mMailSender = new BuddyFeature.Web.MailSender("buddy@bluefrogrobotics.com", "buddySend1Email", BuddyFeature.Web.SMTP.BFR);
            mIcoMessage.enabled = true;
            mCounterTime.SetActive(false);
            for (int i = 0; i < mHaloImages.Length; i++)
                mHaloImages[i].color = new Color(1F, 0f, 0f, 1F);
            int lAlerte = animator.GetInteger("Alerte");
            if (lAlerte > 0 && lAlerte <= mListSpriteNotif.Length)
                mIcoMessage.sprite = mListSpriteNotif[lAlerte - 1];
            mMessage.text = "JE LANCE LA SURVEILLANCE DANS";
            mAnimator = animator;
            mButtonPassword = mObjectButtonAskPassword.GetComponentInChildren<Button>();
            mButtonPassword.onClick.AddListener(AskPassword);
            SetMailAdress();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer -= Time.deltaTime;
            //mFaceManager.Speak(mTTS.IsSpeaking());
            //mDetectorManager.SoundDetector.CanSave = mMailSender.CanSend;
            //Debug.Log("peut send: " + mMailSender.CanSend);
            if (!mHasAlerted)
            {
                mHasAlerted = true;
                SayAlertType(animator);
                mHaloPrefab.SetActive(true);
                //mBackgroundPrefab.SetActive(true);
                mHaloAnimator.SetTrigger("Open_WTimer");
                mBackgroundAnimator.SetTrigger("Open_BG");
                mObjectButtonAskPassword.SetActive(true);
            }

            else if (!mHasSentNotification && mTimer<0.0f)
            {
                if (HasSavedProof())
                {
                //if (mMailSender.CanSend)
                    SendNotification();

                mHasSentNotification = true;
                
                    Debug.Log("proof saved");
                mTimer = 3.0f;
                }
            }

            else if (mTimer < 0.0f && mHasSentNotification)
            {
                animator.SetBool("HasAlerted", false);
                mHaloAnimator.SetTrigger("Close_WTimer");
                mObjectButtonAskPassword.SetActive(false);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("ChangeState", false);
            animator.SetBool("HasAlerted", false);
            animator.SetInteger("Alerte", 0);
            //mFaceManager.SetExpression(MoodType.NEUTRAL);
            mMood.Set(MoodType.NEUTRAL);
            mButtonPassword.onClick.RemoveAllListeners();
        }

        private void AskPassword()
        {
            mAnimator.SetBool("AskPassword", true);
            mAnimator.SetBool("HasAlerted", false);
            mHaloAnimator.SetTrigger("Close_WTimer");
            mObjectButtonAskPassword.SetActive(false);
            Debug.Log("ask password");
        }

        private bool HasSavedProof()
        {
            bool lHasSaved = false;
            int lAlerte = mAnimator.GetInteger("Alerte");
            switch (lAlerte)
            {
                case (int)DetectionManager.Alert.SOUND:
                    lHasSaved = mDetectorManager.SoundDetector.SoundSaved;
                    break;
                default:
                    lHasSaved = true;
                    break;
            }
            return lHasSaved;
        }

        private void SendNotification()
        {
            CultureInfo lCulture;
            if (BYOS.Instance.LanguageManager.CurrentLang==Language.FRA)
                lCulture = new CultureInfo("fr-FR");
            else
                lCulture = new CultureInfo("en-US");

            DateTime localDate = DateTime.Now;

            int lAlerte = mAnimator.GetInteger("Alerte");
            switch (lAlerte)
            {
                case (int)DetectionManager.Alert.SOUND:
                    if (mMailSender.CanSend && mMailAdress != "")
                    {
                        //mDetectorManager.mSoundDetector.SaveWav("noise");

                        string lTextMail;
                        if (BYOS.Instance.LanguageManager.CurrentLang == Language.FRA)
                            lTextMail = "On est le " + localDate.ToString(lCulture) + ". Je viens de détecter du bruit";
                        else
                            lTextMail = "It's " + localDate.ToString(lCulture) + ". I have detected some noise";

                        BuddyFeature.Web.Mail lMail = new BuddyFeature.Web.Mail("noise alert", lTextMail);
                        lMail.AddTo(mMailAdress);
                        if (mWebcam != null && !mWebcam.IsOpen)
                        {
                            mWebcam.Open();
                        }
                        lMail.AddFile("noise.wav");
                        mCountPhoto++;
                        mMailSender.Send(lMail);
                        mDetectorManager.SoundDetector.CanSave = false;
                    }
                    break;
                case (int)DetectionManager.Alert.FIRE:
                    //mDetectedIssues
                    //mMessage.text = "ATTENTION DEPART DE FEU POTENTIEL!";
                    mAnimator.SetBool("ChangeState", false);
                    Debug.Log(localDate.ToString(lCulture));
                    if (mMailSender.CanSend && mMailAdress != "")
                    {
                        string lTextMail;
                        if (BYOS.Instance.LanguageManager.CurrentLang == Language.FRA)
                            lTextMail = "On est le " + localDate.ToString(lCulture) + ". Je viens de détecter une forte chaleur";
                        else
                            lTextMail = "It's " + localDate.ToString(lCulture) + ". I have detected fire";
                        BuddyFeature.Web.Mail lMail = new BuddyFeature.Web.Mail("fire alert", lTextMail);
                        lMail.AddTo(mMailAdress);
                        if (mWebcam != null && !mWebcam.IsOpen)
                        {
                            mWebcam.Open();
                        }
                        if (mWebcam != null && mWebcam.IsOpen && mWebcam.FrameTexture2D != null)
                        {
                            lMail.AddTexture2D(mWebcam.FrameTexture2D, "photocam.png");//+mCountPhoto+".png");
                            mCountPhoto++;
                            mMailSender.Send(lMail);
                        }
                    }
                    break;
                case (int)DetectionManager.Alert.MOVEMENT:
                    //mMessage.text = "ATTENTION INTRUSION POTENTIELLE!";
                    mAnimator.SetBool("ChangeState", false);

                    Debug.Log(localDate.ToString(lCulture));
                    if (mMailSender.CanSend && mMailAdress != "")
                    {
                        Debug.Log("can send");
                        string lTextMail;
                        if (BYOS.Instance.LanguageManager.CurrentLang == Language.FRA)
                            lTextMail = "On est le " + localDate.ToString(lCulture) + ". Je viens de détecter un mouvement";
                        else
                            lTextMail = "It's " + localDate.ToString(lCulture) + ". I have detected some movement";
                        BuddyFeature.Web.Mail lMail = new BuddyFeature.Web.Mail("movement alert", lTextMail);
                        lMail.AddTo(mMailAdress);
                        if (mWebcam != null && !mWebcam.IsOpen)
                        {
                            mWebcam.Open();
                        }
                        mDetectorManager.MovementDetector.Save("monitoring.avi");
                        lMail.AddFile("monitoring.avi");
                        //lMail.AddTexture2D(mWebcam.FrameTexture2D, "photocam.png"); //+ mCountPhoto + ".png");
                        //mCountPhoto++;
                        mMailSender.Send(lMail);
                    }
                    else
                        Debug.Log("peut pas send");
                    break;
                case (int)DetectionManager.Alert.KIDNAPPING:
                    //mMessage.text = "ATTENTION VOL POTENTIEL!";
                    break;
                default:
                    break;
            }
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        private void SayAlertType(Animator animator)
        {
            //mFaceManager.SetExpression(MoodType.SCARED);
            mMood.Set(MoodType.SCARED);
            int lAlerte = animator.GetInteger("Alerte");

            switch (lAlerte)
            {
                case (int)DetectionManager.Alert.SOUND:
                    mTTS.Say(mDictionary.GetString("alertSoundOral"));//("Bruit détecté");
                    mMessage.text = mDictionary.GetString("alertSoundText");//"ATTENTION SON DETECTE!";

                    break;
                case (int)DetectionManager.Alert.FIRE:
                    mMessage.text = mDictionary.GetString("alertFireText");//"ATTENTION DEPART DE FEU POTENTIEL!";
                    mTTS.Say(mDictionary.GetString("alertFireOral"));//("Feu détecté");

                    break;
                case (int)DetectionManager.Alert.MOVEMENT:
                    mMessage.text = mDictionary.GetString("alertMovementText");// "ATTENTION INTRUSION POTENTIELLE!";
                    mTTS.Say(mDictionary.GetString("alertMovementOral"));//("Mouvement détecté");

                    break;
                case (int)DetectionManager.Alert.KIDNAPPING:
                    mMessage.text = mDictionary.GetString("alertKidnapText");// "ATTENTION VOL POTENTIEL!";
                    mTTS.Say(mDictionary.GetString("alertKidnapOral"));//("On me kidnappe");
                    break;
                default:
                    break;
            }

        }

        //private void SetMailAdress()
        //{
        //    switch (mDropListContact.captionText.text)
        //    {
        //        case "RODOLPHE HASSELVANDER":
        //            mMailAdress = "rh@bluefrogrobotics.com";
        //            Debug.Log("rodolphe");
        //            break;
        //        case "WALID ABDERRAHMANI":
        //            mMailAdress = "tigrejounin@gmail.com";
        //            Debug.Log("walid");
        //            break;
        //        case "JEAN MICHEL MOURIER":
        //            mMailAdress = "jmm@bluefrogrobotics.com";
        //            Debug.Log("jean michel");
        //            break;
        //        default:
        //            mMailAdress = "";
        //            Debug.Log("personne");
        //            break;
        //    }
        //}

        private void SetMailAdress()
        {
            switch (GuardianData.Instance.Recever)
            {
                case GuardianData.Contact.RODOLPHE:
                    mMailAdress = "rh@bluefrogrobotics.com";
                    Debug.Log("rodolphe");
                    break;
                case GuardianData.Contact.WALID:
                    mMailAdress = "tigrejounin@gmail.com";
                    Debug.Log("walid");
                    break;
                case GuardianData.Contact.J2M:
                    mMailAdress = "jmm@bluefrogrobotics.com";
                    Debug.Log("jean michel");
                    break;
                case GuardianData.Contact.MAUD:
                    mMailAdress = "mv@bluefrogrobotics.com";
                    Debug.Log("maud");
                    break;
                case GuardianData.Contact.FRANCK:
                    mMailAdress = "fd@bluefrogrobotics.com";
                    Debug.Log("franck");
                    break;
                case GuardianData.Contact.BENOIT:
                    mMailAdress = "bp@bluefrogrobotics.com";
                    Debug.Log("benoit");
                    break;
                case GuardianData.Contact.MARC:
                    mMailAdress = "mg@bluefrogrobotics.com";
                    Debug.Log("marc");
                    break;
                default:
                    mMailAdress = "";
                    Debug.Log("personne");
                    break;
            }
        }

        private void InitLink()
        {
            mDetectorManager = StateManager.DetectorManager;
            mBackgroundAnimator = StateManager.BackgroundAnimator;
            mHaloPrefab = StateManager.HaloPrefab;
            mBackgroundPrefab = StateManager.BackgroundPrefab;
            mHaloAnimator = StateManager.HaloAnimator;
            mHaloImages = StateManager.HaloImages;
            mIcoMessage = StateManager.IcoMessage;
            mMessage = StateManager.MessageText;
            mListSpriteNotif = StateManager.ListSpriteNotif;
            mCounterTime = StateManager.CounterTime;
            mObjectButtonAskPassword = StateManager.ObjectButtonAskPassword;
            //mDropListContact = StateManager.DropListContact;
        }
    }
}