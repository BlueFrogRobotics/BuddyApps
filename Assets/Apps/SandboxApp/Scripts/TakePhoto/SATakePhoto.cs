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
        [SerializeField]
        private int IndexOfRawImageWhereOverlay;

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
        private Mat MatOverlay;
        private Texture2D lTexture;
        private Texture2D lOverlayTexture;
        private Sprite lSpriteOverlay;
        private bool mIsLoadSpritedone;
        private RawImage mCopy;
        
        private GameObject mCopyDisplay;

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
            mMatSrc = new Mat();
            mIsLoadSpritedone = false;
        }
        

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0F;
            if (!Primitive.RGBCam.IsOpen)
                Primitive.RGBCam.Open(RGBCamResolution.W_640_H_480);
            if(!InstantPhoto && DisplayVideo)
                GetGameObject(IndexRawImageWhereDisplay).SetActive(true);
            //if (WantOverlay)
            //    GetGameObject(IndexOfRawImageWhereOverlay).SetActive(true);
            mRawImageWhereDisplay = GetGameObject(IndexRawImageWhereDisplay).GetComponent<RawImage>();
            //mCopy = GetGameObject(IndexOfRawImageWhereOverlay).GetComponent<RawImage>();
            if(WantOverlay)
            {
                mCopyDisplay = Instantiate(GetGameObject(IndexRawImageWhereDisplay));
                mCopyDisplay.transform.parent = GetGameObject(1).transform;

                mCopyDisplay.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                mCopyDisplay.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
                mCopyDisplay.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
                mCopy = mCopyDisplay.GetComponent<RawImage>();
            }

            // mCopy = Instantiate(mRawImageWhereDisplay);
            //mCopy.transform.parent = GetGameObject(1).transform;
            //mCopy.GetComponent<RectTransform>(). = ;
            //mCopy.GetComponent<RectTransform>().
            //mCopy.transform.SetSiblingIndex(1);
            //mCopy = Instantiate(mRawImageWhereDisplay);
            //mCopy.transform.parent = GetGameObject(1).transform;
            //mCopy.transform.SetSiblingIndex(0);
            //mRawImageWhereDisplay.enabled = false;
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
                mMatSrc = Primitive.RGBCam.FrameMat;
                Core.flip(mMatSrc, mMatDest, 1);
                if (InstantPhoto)
                {                    
                    if (mTimer > 1F)
                    {
                        Primitive.RGBCam.TakePhotograph(OnFinish, false);

                        GetGameObject(IndexRawImageWhereDisplay).SetActive(false);
                        Reset();
                    }
                    return;
                }

               

                if ((mTimer > 5.5F && mCountDownBuddy) || ((int)(mStarterCountdown - mTimer) < 1 && mCountDownBuddy))
                {
                    mTimer = 0F;
                    Primitive.RGBCam.TakePhotograph(OnFinish, false);
                    Reset();

                }

                if (IsCountdownSaid)
                {
                    //A voir en fonction de quand commence le countdown (peut etre que ça sera simple avec VOCON)
                    //mTTS.Say();
                }

                if (CountdownToDisplay == Countdown.CLASSIC  && mTimerForCountdown > 2F && !mCountDownDisplayed && !mCountDownBuddy) 
                {
                    if (GetGameObject(IndexTextOfCountdown).GetComponent<Text>() != null)
                    {
                        if (!mIsCountdownStarted)
                        {
                            mTimer = -0.5F;
                            mIsCountdownStarted = true;
                        }

                        GetGameObject(IndexTextOfCountdown).SetActive(true);
                        int test = (int)(mStarterCountdown - mTimer);
                        GetGameObject(IndexTextOfCountdown).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        GetGameObject(IndexTextOfCountdown).GetComponent<Text>().fontSize = 180;
                        GetGameObject(IndexTextOfCountdown).GetComponent<Text>().color = new Color32(ColorOfCountdown.r,ColorOfCountdown.g, ColorOfCountdown.b, 255);
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
                    if (!mCountDownDisplayed)
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

                if (DisplayVideo && !mIsPhotoTaken)
                {
                    if (!WantOverlay)
                    {
                        mRawImageWhereDisplay.texture = Utils.MatToTexture2D(mMatDest);
                    }
                    else
                    {
                        //if (IsFolderEmpty())
                        //{
                        //    Debug.Log("Your overlay doesn't exist, check if it is in the right folder (\"your app name\"\\Resources\\Overlay) or check if your extension is .png or .PNG");
                        //}
                        //else
                        //{
                        //l'overlay existe et on l'affiche
                        if(!mIsLoadSpritedone)
                        {
                            lSpriteOverlay = Resources.Load<Sprite>(NameOfOverlay);
                            mCopy.texture = lSpriteOverlay.texture;
                            mIsLoadSpritedone = true;
                        }
                        mRawImageWhereDisplay.texture = Utils.MatToTexture2D(mMatDest);
                        //}
                    }

                }
            }
            if(mIsPhotoTaken)
            {
                if (!mIsDisplayPhotoDone)
                {
                    mTimer = 0F;
                    mIsDisplayPhotoDone = true;
                    DisplayPhoto(mFilePath);
                }

                if(mTimer > 6F && mIsDisplayPhotoDone)
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
            mCountDownBuddy = true;
            Toaster.Hide();
        }

        private void OnFinish(Photograph iMyPhoto)
        {
            Sprite lFlipSprite;
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
                lFlipSprite = Sprite.Create(Utils.MatToTexture2D(DrawOverlayOnPicture(Utils.MatToTexture2D(mMatDest))), new UnityEngine.Rect(0, 0, iMyPhoto.Image.texture.width, iMyPhoto.Image.texture.height), new Vector2(0.5F, 0.5F));
            }
            else
            {
                lFlipSprite = Sprite.Create(iMyPhoto.Image.texture, new UnityEngine.Rect(0, 0, iMyPhoto.Image.texture.width, iMyPhoto.Image.texture.height), new Vector2(0.5F, 0.5F));
            }
        
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

            GetGameObject(IndexTextOfCountdown).SetActive(false);
            GetGameObject(IndexRawImageWhereDisplay).SetActive(false);
            //GetGameObject(IndexOfRawImageWhereOverlay).SetActive(false);
            Destroy(mCopyDisplay);
            Primitive.RGBCam.Close();
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

        private void DisplayPhoto(string iFile)
        {
            Sprite lsprite;
            lsprite = Utils.CreateSpriteFromFile(iFile);
            Toaster.Display<PictureToast>().With(lsprite);
        }

        private  Mat  DrawOverlayOnPicture(Texture2D iTexture)
        {
            lTexture = iTexture;
            lOverlayTexture = lSpriteOverlay.texture;

            var cols1 = lOverlayTexture.GetPixels();
            var cols2 = lTexture.GetPixels();

            // We scale the overlay and put it on top of the picture
            // it's ok if you don't get it ^^
            int x = 0;
            int y = 0;
            int i2 = 0;
            for (var i = 0; i < cols2.Length; ++i)
            {
                x = i % lTexture.width;
                y = i / lTexture.width;
                i2 = (x * lOverlayTexture.width / lTexture.width) + (y * lOverlayTexture.height / lTexture.height) * lOverlayTexture.width;
                if (cols1[i2].a != 0)
                {
                    cols2[i] = (1 - cols1[i2].a) * cols2[i] + cols1[i2].a * cols1[i2];
                }
            }

            lTexture.SetPixels(cols2);
            lTexture.Apply();
            Utils.Texture2DToMat(lTexture, mMatDest);
            return mMatDest;
        }
    }
}

