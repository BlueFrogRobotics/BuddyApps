using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using BuddyOS;

public class BabyPhone : MonoBehaviour
{
    /// <summary>
    /// TEST OPEN CAMERA
    /// </summary>
    private RGBCam mRGBCam;





    [SerializeField]
    Motors mMotors;

    //[SerializeField]
    //Motion mMotion;

    [SerializeField]
    private List<Canvas> mSceneCanvas = new List<Canvas>();

    [SerializeField]
    private List<GameObject> GameObjectToDisable = new List<GameObject>();

    [SerializeField]
    private GameObject mNotifications;

    [SerializeField]
    private Text mNumberOfNotifications;

    [SerializeField]
    private Text mStartListningCount;

    private Face mFace;

    [SerializeField]
    private MicInput mMicroInput;

    [SerializeField]
    private SendMail mSendMail;

    [SerializeField]
    AudioSource mSpeaker;

    private int mCountNotification;
    private TextToSpeech mTTS;

    //private RGBCam mCam;

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
        mTTS = BYOS.Instance.TextToSpeech;
        mFace = BYOS.Instance.Face;

        mRGBCam = BYOS.Instance.RGBCam;
        
    }
    void Start()
    {
        //StartCoroutine(SendMessage());
        //mSendMail.Send();
        //mCam.Open();

        mCount = 0;
        mMean = 0;
        mTime = 0;

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

        if ((mTime >= 4f) && (!mStartCaringBaby) && (!mSayTuto))
        {
            mTTS.Say("Place-moi à environ un maitre du lit de bébé, en orientant mon visage vers lui.");
            mSayTuto = true;
        }

        if ((mTime >= 4f) && (mTime <= 10f) && (!mStartCaringBaby))
        {
            //mSceneCanvas[0].GetComponent<Canvas>().enabled = true;
            //mSceneCanvas[1].GetComponent<Canvas>().enabled = true;

            GameObjectToDisable[0].SetActive(true);
            GameObjectToDisable[1].SetActive(true);

        }

        if ((mTime > 10f) && (!mSayYesOrNo))
        {
            mTTS.Say("Veux-tu commencer la surveillance de bébé?");
            mSayYesOrNo = true;
        }

        if ((mTime > 10f) && (!mStartCaringBaby))
        {
            //mSceneCanvas[1].GetComponent<Canvas>().enabled = false;
            //mSceneCanvas[2].GetComponent<Canvas>().enabled = true;

            GameObjectToDisable[1].SetActive(false);
            GameObjectToDisable[2].SetActive(true);
        }

        if (mStartCaringBaby)
        {
            mTime += Time.deltaTime;
            //mSceneCanvas[2].GetComponent<Canvas>().enabled = false;
            //mSceneCanvas[3].GetComponent<Canvas>().enabled = true;
            GameObjectToDisable[2].SetActive(false);
            GameObjectToDisable[3].SetActive(true);
            if (mTime >= 2f)
                mStartListningCount.text = "4";

            if (mTime >= 4f)
                mStartListningCount.text = "3";

            if (mTime >= 6f)
                mStartListningCount.text = "2";

            if (mTime >= 8f)
                mStartListningCount.text = "1";

            if (mTime >= 10f)
                mStartListningCount.text = "0";

            if (mTime >= 10.5f)
            {
                //mSceneCanvas[3].GetComponent<Canvas>().enabled = false;
                //mSceneCanvas[2].GetComponent<Canvas>().enabled = false;
                GameObjectToDisable[3].SetActive(false);
                GameObjectToDisable[2].SetActive(false);
                BuddyLisen();
                mIsBuddyListening = true;
            }
        }

        if (mIsBuddyListening)
        {
            mSound = mMicroInput.MicLoundness;
            mMean += mSound;
            mCount = mCount + 1;
            
            if (mCount > 50f)
            {
                mMean = mMean / 50f;
                //Debug.Log(mMean);
                if (mMean >= 0.1f)
                    mIsBabyCrying = true;
                else
                    mIsBabyCrying = false;
                mMean = 0;
                mCount = 0;
            }
        }
        
        //Debug.Log(mMean);
        if ((mIsBabyCrying) && (mIsBuddyListening))
        {
            StartCoroutine(SetSadMood());
            //mNotifications.GetComponent<GameObject>().SetActive(true);
            
            mCountNotification = mCountNotification + 1;
            if (mCountNotification == 100)
            {
                
                mNotifications.SetActive(true);
                mNumberOfNotifications.text = "1";
                Debug.Log("TIME TO SEND");
                StartCoroutine(SendMessage());
                mMove = true;

            }
           
            if ((mTime >= 300) && (mIsBuddyListening) && (!mIsBabyCrying))
            {
                if (mSpeaker.isPlaying)
                {
                    mSpeaker.Stop();
                }
            }

            //if (mCountNotification == 150)
            //    mNumberOfNotifications.text = "2";
        }

        

        //if (mMove)
        //{
        //    if (Mathf.Round(mTime) % 3 == 0.0f)
        //    {
        //        if (mRight)
        //        {
        //            mMotors.Wheels.TurnAbsoluteAngle(-20, 120, 1);
        //            mRight = false;
        //        }
        //        else
        //        {
        //            mMotors.Wheels.TurnAbsoluteAngle(20, 120, 1);
        //            mRight = true;
        //        }
        //    }
        //}

    }

    /// <summary>
    /// cette fonction est appelée quand on appuie sur le bouton "commencer à garder bébé"
    /// </summary>
    public void StartCaringBaby()
    {
        mStartCaringBaby = true;
        mTime = 0;
    }

    private void BuddyLisen()
    {
        StartCoroutine(SetListenMood());
        // lance aussi la berceuse 4
        if (!mSpeaker.isPlaying)
        {
            mSpeaker.Play();
        }

    }

    IEnumerator SetSadMood()
    {
        yield return new WaitForSeconds(0.5f);
        mFace.SetMood(FaceMood.SAD);
    }

    IEnumerator SetListenMood()
    {
        yield return new WaitForSeconds(0.5f);
        mFace.SetMood(FaceMood.LISTEN);
    }

    IEnumerator SendMessage()
    {
        //mCam.Open();
        //yield return new WaitForSeconds(2.0f);
        mRGBCam.Open();
        yield return new WaitForSeconds(0.5f);
        mSendMail.Send();
        yield return new WaitForSeconds(0.5f);
        mRGBCam.Close();

    }
    // brouillon
    //------------------------------------------------------------------------//
    //BuddyLisen();
    // mFace.Speak(mTTS.IsSpeaking());
    //if (!IsBabyCraying)
    //{
    //    mFace.Listen();
    //}
    //else
    //{
    //    mFace.Sad();
    //}

    //    Debug.Log(mMean);
    //    mMean = 0;
    //    mCount = 0;
    //}

    //mSpeaker.isPlaying();
    //------------------------------------------------------------------------//
}
