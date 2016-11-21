using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using BuddyOS;
using BuddyFeature.Web;

namespace BuddyApp.BabyPhone
{
    [RequireComponent(typeof(InputMicro))]
    [RequireComponent(typeof(AudioSource))]
    public class BabyPhone : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> canvas = new List<GameObject>();

        [SerializeField]
        private GameObject notifications;

        [SerializeField]
        private Text notificationAmount;

        [SerializeField]
        private Text startListningCount;

        private TextToSpeech mTTS;
        private RGBCam mRGBCam;
        private Face mFace;
        private InputMicro mMicro;
        private AudioSource mSpeaker;

        private int mCountNotification;

        private bool mLeft;
        private bool mMove;
        private bool mSayTuto;
        private bool mSayYesOrNo;
        private bool mRight;

        /// <summary>
        /// passe à true quand le buddy entend un bruit fort 
        /// </summary>
        private bool mIsBabyCrying;

        /// <summary>
        /// passe à true quand on buddy est entrain d'écouter le son ambiant
        /// </summary>
        private bool mIsBuddyListening;

        /// <summary>
        /// 
        /// </summary>
        private float mSound;

        /// <summary>
        /// compteur qui défini le nombre de frame pour faire la moyenne des bruit
        /// </summary>
        private int mCount;

        /// <summary>
        /// Calcule la moyenne du bruit ambiant sur 10 frame 
        /// </summary>
        private float mMean;

        /// <summary>
        ///  compteur du temps
        /// </summary>
        private float mTime;

        /// <summary>
        /// s'active quand on appuie sur le bouton oui 
        /// </summary>
        private bool mStartCaringBaby;

        void Awake()
        {
            mMicro = GetComponent<InputMicro>();
            mSpeaker = GetComponent<AudioSource>();
            mTTS = BYOS.Instance.TextToSpeech;
            mFace = BYOS.Instance.Face;
            mRGBCam = BYOS.Instance.RGBCam;
        }

        void Start()
        {
            mCount = 0;
            mMean = 0F;
            mTime = 0F;

            mStartCaringBaby = false;
            mIsBabyCrying = false;
            mIsBuddyListening = false;
            mCountNotification = 0;
            mSayTuto = false;
            mSayYesOrNo = false;
            mRight = false;

            mMove = false;
            mTTS = new TextToSpeech();
        }

        void Update()
        {

            mTime += Time.deltaTime;

            if ((mTime >= 4F) && (!mStartCaringBaby) && (!mSayTuto)) {
                mTTS.Say("Place-moi à environ un maitre du lit du bébé, en orientant mon visage vers lui.");
                mSayTuto = true;
            }

            if ((mTime >= 4F) && (mTime <= 10F) && (!mStartCaringBaby)) {
                canvas[0].SetActive(true);
                canvas[1].SetActive(true);
            }

            if ((mTime > 10F) && (!mSayYesOrNo)) {
                mTTS.Say("Veux-tu commencer la surveillance de bébé?");
                mSayYesOrNo = true;
            }

            if ((mTime > 10F) && (!mStartCaringBaby)) {
                canvas[1].SetActive(false);
                canvas[2].SetActive(true);
            }

            if (mStartCaringBaby) {
                mTime += Time.deltaTime;
                canvas[2].SetActive(false);
                canvas[3].SetActive(true);
                if (mTime >= 2F)
                    startListningCount.text = "4";

                if (mTime >= 4F)
                    startListningCount.text = "3";

                if (mTime >= 6F)
                    startListningCount.text = "2";

                if (mTime >= 8F)
                    startListningCount.text = "1";

                if (mTime >= 10F)
                    startListningCount.text = "0";

                if (mTime >= 10.5F) {
                    canvas[3].SetActive(false);
                    canvas[2].SetActive(false);
                    BuddyListen();
                    mIsBuddyListening = true;
                }
            }

            if (mIsBuddyListening) {
                mSound = mMicro.Loudness;
                mMean += mSound;
                mCount = mCount + 1;

                if (mCount > 50F) {
                    mMean = mMean / 50F;
                    if (mMean >= 0.1f)
                        mIsBabyCrying = true;
                    else
                        mIsBabyCrying = false;
                    mMean = 0;
                    mCount = 0;
                }
            }

            if ((mIsBabyCrying) && (mIsBuddyListening)) {
                mRGBCam.Open();
                StartCoroutine(SetSadMood());

                mCountNotification = mCountNotification + 1;
                if (mCountNotification == 100) {
                    notifications.SetActive(true);
                    notificationAmount.text = "1";
                    StartCoroutine(SendMessage());
                    mMove = true;
                }

                if ((mTime >= 300) && (mIsBuddyListening) && (!mIsBabyCrying)) {
                    if (mSpeaker.isPlaying) {
                        mSpeaker.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// cette fonction est appelée quand on appuie sur le bouton "commencer à garder bébé"
        /// </summary>
        public void StartCaringBaby()
        {
            mStartCaringBaby = true;
            mTime = 0;
        }

        private void BuddyListen()
        {
            StartCoroutine(SetListenMood());
            if (!mSpeaker.isPlaying) {
                mSpeaker.Play();
            }
        }

        private IEnumerator SetSadMood()
        {
            yield return new WaitForSeconds(0.5F);
            mFace.SetMood(FaceMood.SAD);
        }

        private IEnumerator SetListenMood()
        {
            yield return new WaitForSeconds(0.5F);
            mFace.SetMood(FaceMood.LISTEN);
        }

        private IEnumerator SendMessage()
        {
            mRGBCam.Open();
            yield return new WaitForSeconds(0.5F);
            MailSender lSender = new MailSender("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL);
            Mail lEmail = new Mail("BUDDY ALERT - BABYPHONE", "Your baby seems to cry =(");
            lEmail.AddTexture2D(mRGBCam.FrameTexture2D, "image.png");
            lSender.Send(lEmail);

            //mSendMail.Send();
            yield return new WaitForSeconds(0.5F);
            mRGBCam.Close();
        }
    }
}