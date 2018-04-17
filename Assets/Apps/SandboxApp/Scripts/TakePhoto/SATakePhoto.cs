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
        [Tooltip("You need to create the rawimage in the canvas of your scene, then you put it in \"your app name\"stateMachineManager. And you put the index of your rawimage here : ")]
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
        private string TriggerToNextState;

        private bool mIsDisplay;
        private bool mPhotoTaken;
        private Mat mMatSrc;
        private Mat mMatDest;
        private RawImage mRawImageWhereDisplay;
        

        public override void Start()
        {
            Primitive.RGBCam.Resolution = RGBCamResolution.W_640_H_480;
            mMatDest = new Mat();
            mIsDisplay = false;
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
            Mat mMatSrc = Primitive.RGBCam.FrameMat;
            Core.flip(mMatSrc, mMatDest, 1);

            if (DisplayVideo)
            {
                if(!WantOverlay)
                {
                    mRawImageWhereDisplay.texture = Utils.MatToTexture2D(mMatDest);
                }
                else
                {
                    if(IsFolderEmpty())
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
            if(!File.Exists(lPath + "/" + NameOfOverlay + ".png") || !File.Exists(lPath + "/" + NameOfOverlay + ".PNG"))
            {
                return true;
            }
            return false;
        }

    }
}

