using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Xml;
using OpenCVUnity;
using Buddy.UI;

namespace BuddyApp.SandboxApp
{
    public class SATakePhoto : AStateMachineBehaviour
    {
        //TODO : - toast redo photo (creer buttoninfo pour les deux boutons)
        // - take photo (display de la video ou non)
        // - options : overlay pris dans le dossier resources de l'app
        // - vocal pour demander si la photo est bonne ou pas
        // - gérer le cas où le mec met oui pour loverlay mais qu'il n'y a pas d'image dans le dossier resources de l'application
        // - peut etre enregistrer le nom de la photo dans une classe a part pour qu'on puisse la retrouver dans une state après

        //SHARED : Linker d'objet UI pour les utiliser dans shared

        //Ce quil reste a faire pour la v1 : Overlay (a voir) + buddy qui dit le décompte

        //[SerializeField]
        //private string Title;
        //[SerializeField]
        //private string BuddySays;
        //[SerializeField]
        //private bool IsSoundForButton;
        //[SerializeField]
        //private FXSound FxSound;
        [SerializeField]
        private string NameOfPhotoTaken;

        [Header("Put your mouse on the index of the RawImage and follow the tooltip")]
        [SerializeField]
        private bool DisplayVideo;
        [Tooltip("You need to create the rawimage in the canvas of your scene, then you link it in \"your app name\"stateMachineManager of the AIBehaviour on your scene. And you put the index of your rawimage int the stateMachineManager here : ")]
        [SerializeField]
        private int IndexRawImageWhereDisplay;

        [Space]
        [SerializeField]
        private bool DisplayPhotoTaken;
        [Tooltip("If you check this you need to put all your overlays in the folder Overlay in \"your app name\"\\Resources\\Overlay")]
        [SerializeField]
        private bool WantOverlay;
        [SerializeField]
        private string NameOfOverlay;

        [Space]
        [SerializeField]
        private string TriggerToNextState;

        
       
        private enum Countdown
        {
            BUDDY_COUNTDOWN,
            CLASSIC,
            NONE,
        };

        [Space]
        [Header("Countdown Parameters : ")]
        [SerializeField]
        private Countdown CountdownToDisplay;
        [Tooltip("If you want to have a visible countdown, you have to create a text in the canvas of your scene. Then you link it in \"your app name\"stateMachineManager of the AIBehaviour on your scene. And you put the index of your text in the stateMachineManager here ")]
        //[SerializeField]
        //private bool IsCountdownVisible;
        [SerializeField]
        private Color32 ColorOfCountdown;
        [SerializeField]
        private int IndexTextOfCountdown;
        [SerializeField]
        private bool IsCountdownSaid;

        [Space]
        [Header("If you want to take an instant photo without countdown : ")]
        [SerializeField]
        private bool InstantPhoto;

        private bool mIsDisplay;
        private bool mPhotoTaken;
        private Mat mMatSrc;
        private Mat mMatDest;
        private RawImage mRawImageWhereDisplay;
        private TextToSpeech mTTS;
        private bool mCountdownSaid;
        private float mTimer;
        private float mTimerForCountdown;
        private float mStarterCountdown;
        private bool mIsCountdownStarted;
        private bool mIsDone;
        private Sprite mPhotoSprite;
        private bool mIsPhotoTaken;
        private string mFilePath = "";
        private bool mIsDisplayPhotoDone;
        private bool mCountDownDisplayed;
        private bool mCountDownBuddy;

        public override void Start()
        {
            Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Header.DisplayParametersButton = false; 
            mTTS = Interaction.TextToSpeech;
            Primitive.RGBCam.Resolution = RGBCamResolution.W_640_H_480;
            mMatDest = new Mat();
            mIsDisplay = false;
            mTimerForCountdown = 0F;
            mStarterCountdown = 5F;
            mCountdownSaid = false;
            mIsCountdownStarted = false;
            
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!Primitive.RGBCam.IsOpen)
                Primitive.RGBCam.Open(RGBCamResolution.W_640_H_480);
            if(!InstantPhoto && DisplayVideo)
                GetGameObject(IndexRawImageWhereDisplay).SetActive(true);
            mRawImageWhereDisplay = GetGameObject(IndexRawImageWhereDisplay).GetComponent<RawImage>();
            mIsDone = false;
            mIsDisplayPhotoDone = false;
            mPhotoTaken = false;
            mCountDownBuddy = false;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimerForCountdown += Time.deltaTime;
            mTimer += Time.deltaTime;
            if (!mIsDone)
            {
                Debug.Log("1 "  + mIsDone);
                if (InstantPhoto)
                {
                    Mat mMatSrc = Primitive.RGBCam.FrameMat;
                    Core.flip(mMatSrc, mMatDest, 1);
                    if (mTimer > 1F)
                    {
                        Primitive.RGBCam.TakePhotograph(OnFinish, false);

                        GetGameObject(IndexRawImageWhereDisplay).SetActive(false);
                        Reset();
                    }
                    return;
                }

                Debug.Log("2");

                if ((mTimer > 5.5F && mCountDownBuddy) || ((int)(mStarterCountdown - mTimer) < 1 && mCountDownBuddy))
                {
                    mTimer = 0F;
                    Primitive.RGBCam.TakePhotograph(OnFinish, false);
                    Reset();

                }
                Debug.Log("4");

                if (IsCountdownSaid)
                {
                    //mTTS.Say();
                }

                if (CountdownToDisplay == Countdown.CLASSIC  && mTimerForCountdown > 2F && !mCountDownDisplayed && !mCountDownBuddy) 
                {
                    if (GetGameObject(IndexTextOfCountdown).GetComponent<Text>() != null)
                    {
                        Debug.Log("6");

                        if (!mIsCountdownStarted)
                        {
                            mTimer = -0.5F;
                            mIsCountdownStarted = true;
                        }
                        Debug.Log("7");

                        GetGameObject(IndexTextOfCountdown).SetActive(true);
                        int test = (int)(mStarterCountdown - mTimer);
                        GetGameObject(IndexTextOfCountdown).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        GetGameObject(IndexTextOfCountdown).GetComponent<Text>().fontSize = 180;
                        GetGameObject(IndexTextOfCountdown).GetComponent<Text>().color = new Color32(ColorOfCountdown.r,ColorOfCountdown.g, ColorOfCountdown.b, 255);
                        Debug.Log("MTIMER : " + mTimer + " TEST : " + test);
                        GetGameObject(IndexTextOfCountdown).GetComponent<Text>().text = test.ToString();
                        if((int)(mStarterCountdown - mTimer) <= 0)
                        {
                            mCountDownDisplayed = true;
                            mCountDownBuddy = true;
                        }
                        
                    }
                    else
                        Debug.Log("You put the wrong index of your UI Object text in the StateMachineBehaviour");
                }
                else if(CountdownToDisplay == Countdown.BUDDY_COUNTDOWN && mTimerForCountdown > 2F)
                {
                    if(!mCountDownDisplayed)
                    {
                        Toaster.Display<CountdownToast>().With(5, OnCountDownFinished);
                        mCountDownDisplayed = true;
                    }
                }
                else if(CountdownToDisplay == Countdown.NONE && mTimerForCountdown > 2F)
                {
                    if ((int)(mStarterCountdown - mTimer) <= 0)
                    {
                        mCountDownDisplayed = true;
                        mCountDownBuddy = true;
                    }

                }
                Debug.Log("8");

                if (DisplayVideo)
                {
                    if (!WantOverlay)
                    {
                        mRawImageWhereDisplay.texture = Utils.MatToTexture2D(mMatDest);
                    }
                    else
                    {
                        if (IsFolderEmpty())
                        {
                            Debug.Log("Your overlay doesn't exist, check if it is in the right folder (\"your app name\"\\Resources\\Overlay) or check if your extension is .png or .PNG");
                        }
                        else
                        {
                            //l'overlay existe et on l'affiche

                        }
                    }

                }
            }
            if(mIsPhotoTaken)
            {
                if (!mIsDisplayPhotoDone)
                {
                    mIsDisplayPhotoDone = true;
                    DisplayPhoto(mFilePath);
                }

                if(mTimer > 6F)
                {
                    if (!string.IsNullOrEmpty(TriggerToNextState))
                        Trigger(TriggerToNextState);
                    else
                        Debug.Log("You forgot to write the trigger to activate in the animator");
                }
            }
          

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Toaster.Hide();
        }

        private bool IsFolderEmpty()
        {
            string lPath = BYOS.Instance.Resources.GetPathToRaw("Overlay");
            if (!File.Exists(lPath + "/" + NameOfOverlay + ".png") || !File.Exists(lPath + "/" + NameOfOverlay + ".PNG"))
            {
                return true;
            }
            return false;
        }

        private void OnCountDownFinished()
        {
            Debug.Log("ON COUNTDOWN FINISHED LOL");
            mCountDownBuddy = true;
            Toaster.Hide();
        }

        private void OnFinish(Photograph iMyPhoto)
        {
            Debug.Log("ONFINISH");
            GetGameObject(IndexTextOfCountdown).SetActive(false);
            GetGameObject(IndexRawImageWhereDisplay).SetActive(false);
            Primitive.RGBCam.Close();
            string lFileName;
            // save file 
            if (string.IsNullOrEmpty(NameOfPhotoTaken))
            {
                lFileName = "Buddy_" + System.DateTime.Now.Day + "day" +
                System.DateTime.Now.Month + "month" + System.DateTime.Now.Hour + "h" +
                System.DateTime.Now.Minute + "min" + System.DateTime.Now.Second + "sec.png";
            }
            else
                lFileName = NameOfPhotoTaken + ".png";
            
            if (File.Exists(Resources.GetPathToRaw(lFileName)))
                File.Delete(Resources.GetPathToRaw(lFileName));
            mFilePath = Resources.GetPathToRaw(lFileName);
            if(WantOverlay)
            {
                //A faire plus tard avec le takephoto de greg pour mettre l'overlay sur la photo
            }
            else
                mPhotoSprite = iMyPhoto.Image;
            //peut add la fonction flip de limage ou pas
            Sprite lFlipSprite;
            lFlipSprite = Sprite.Create(FlipTexture(iMyPhoto.Image.texture), new UnityEngine.Rect(0, 0, iMyPhoto.Image.texture.width, iMyPhoto.Image.texture.height), new Vector2(0.5F, 0.5F));
            Utils.SaveSpriteToFile(lFlipSprite, mFilePath);

            if(DisplayPhotoTaken)
            {
                mIsPhotoTaken = true;
            }
            else
                if (!string.IsNullOrEmpty(TriggerToNextState))
                    Trigger(TriggerToNextState);
                else
                    Debug.Log("You forgot to write the trigger to activate in the animator");



        }

        private void Reset()
        {
            mIsDone = true;
            DisplayVideo = false;
            //IsCountdownVisible = false;
            InstantPhoto = false;
        }

        Texture2D FlipTexture(Texture2D iOriginal)
        {
            Texture2D lFlipped = new Texture2D(iOriginal.width, iOriginal.height);

            int xN = iOriginal.width;
            int yN = iOriginal.height;


            for (int i = 0; i < xN; i++)
            {
                for (int j = 0; j < yN; j++)
                {
                    lFlipped.SetPixel(xN - i - 1, j, iOriginal.GetPixel(i, j));
                }
            }
            lFlipped.Apply();

            return lFlipped;
        }

        private void DisplayPhoto(string lol)
        {
            Sprite lsprite;
            lsprite = Utils.CreateSpriteFromFile(lol);
            Toaster.Display<PictureToast>().With(lsprite);
        }
    }
}

