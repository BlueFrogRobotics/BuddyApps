using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Xml;
using OpenCVUnity;

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

        [SerializeField]
        private string Title;
        [SerializeField]
        private string BuddySays;
        [SerializeField]
        private bool IsSoundForButton;
        [SerializeField]
        private FXSound FxSound;

        [Header("Put your mouse on the index of the RawImage and follow the tooltip")]
        [SerializeField]
        private bool DisplayVideo;
        [Tooltip("You need to create the rawimage in the canvas of your scene, then you link it in \"your app name\"stateMachineManager. And you put the index of your rawimage int the stateMachineManager here : ")]
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

        [Space]
        [Header("Countdown Parameters : ")]
        [Tooltip("If you want to have a visible countdown, you have to create a text in the canvas of your scene. Then you link it in \"your app name\"stateMachineManager. And you put the index of your text in the stateMachineManager here ")]
        [SerializeField]
        private bool IsCountdownVisible;
        [SerializeField]
        private int IndexTextOfCountdown;
        [SerializeField]
        private bool IsCountdownSaid;
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
            GetGameObject(IndexRawImageWhereDisplay).SetActive(true);
            mRawImageWhereDisplay = GetGameObject(IndexRawImageWhereDisplay).GetComponent<RawImage>();

            mPhotoTaken = false;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(InstantPhoto)
            {
                Primitive.RGBCam.TakePhotograph(OnFinish, false);
                GetGameObject(IndexRawImageWhereDisplay).SetActive(false);
            }
            mTimerForCountdown += Time.deltaTime;
            mTimer += Time.deltaTime;

            Mat mMatSrc = Primitive.RGBCam.FrameMat;
            Core.flip(mMatSrc, mMatDest, 1);

            if (mTimer > 5.5F || (int)(mStarterCountdown - mTimer) < 1)
            {
                Primitive.RGBCam.TakePhotograph(OnFinish, false);

            }

            if (IsCountdownSaid)
            {
                //mTTS.Say();
            }

            if (IsCountdownVisible && mTimerForCountdown > 2F)
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
                    GetGameObject(IndexTextOfCountdown).GetComponent<Text>().text = test.ToString();
                }
                else
                    Debug.Log("You put the wrong index of your UI Object text in the StateMachineBehaviour");
            }

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

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        private void DisplayToast()
        {

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

        private void OnFinish(Photograph iMyPhoto)
        {
            Primitive.RGBCam.Close();
            GetGameObject(IndexTextOfCountdown).SetActive(false);
            GetGameObject(IndexRawImageWhereDisplay).SetActive(false);
            Trigger(TriggerToNextState);
        }
    }
}

