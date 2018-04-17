using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using UnityEngine.UI;
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
        [SerializeField]
        private bool DisplayVideo;
        [SerializeField]
        private GameObject RawImageWhereDisplay;
        [SerializeField]
        private bool DisplayPhotoTaken;
        [SerializeField]
        private bool WantOverlay;
        [SerializeField]
        private string TriggerToNextState;
        private Shared.test test;

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
            RawImageWhereDisplay.gameObject.SetActive(true);
            mPhotoTaken = false;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log(test.TestLol);
            Mat mMatSrc = Primitive.RGBCam.FrameMat;
            Core.flip(mMatSrc, mMatDest, 1);

            if (DisplayVideo)
            {
                RawImageWhereDisplay.GetComponent<RawImage>().texture = Utils.MatToTexture2D(mMatDest);
                if (!mIsDisplay)
                {
                    DisplayToast();
                    mIsDisplay = true;
                }
            }
            
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        private void DisplayToast()
        {
            
        }

    }
}

