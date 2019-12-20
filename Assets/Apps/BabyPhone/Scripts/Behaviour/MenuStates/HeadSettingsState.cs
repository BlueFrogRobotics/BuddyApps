using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using OpenCVUnity;

namespace BuddyApp.BabyPhone
{
    public sealed class HeadSettingsState : AStateMachineBehaviour
    {
        private Texture2D mCamView;

        private GameObject mCustomUi;
        private Animator mCustomUiAnim;

        private GameObject mCallerVideoRawImg;
        private UnityEngine.UI.RawImage mCam;

        public override void Start()
        {
            // Get the custom UI prefab
            mCustomUi = GetGameObject(0);
            // Get the video feedback raw img game object 
            mCallerVideoRawImg = GetGameObject(1);

            // Get the animator of the UI prefab
            mCustomUiAnim = mCustomUi.GetComponent<Animator>();
            // Get the RawImage type in game object            
            mCam = mCallerVideoRawImg.GetComponent<UnityEngine.UI.RawImage>();
        }

        // This coroutine is waiting for the Toaster to be hide to show the custom Ui prefab
        public IEnumerator DisplayCustomUi()
        {
            yield return new WaitUntil(() => !Buddy.GUI.Toaster.IsBusy);
            // Show custom prefab UI
            mCustomUiAnim.SetTrigger("Open_WCall");
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            FButton validateButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            validateButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));
            validateButton.SetBackgroundColor(Utils.BUDDY_COLOR);
            validateButton.SetIconColor(Color.white);

            validateButton.OnClick.Add(() => { Trigger("NextStep"); });
            // Hide the default parameter button.
            //Buddy.GUI.Header.DisplayParametersButton(false);
            // Initialize texture.
            mCamView = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Set the RGB Camera
            Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320X240_30FPS_RGB);
            // The matrix is send to OnNewFrame.
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));

            Buddy.Vocal.SayKey("headorientationmessage");
            // Show Ui prefab as soon as possible
            StartCoroutine(DisplayCustomUi());
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            mCustomUiAnim.SetTrigger("Close_WCall");
            StopCoroutine(DisplayCustomUi());
            Buddy.Sensors.RGBCamera.Close();
            Buddy.GUI.Footer.Hide();
        }

        //  -----CALLBACK------  //

        // On each frame captured by the camera this function is called, with the matrix of pixel.
        private void OnFrameCaptured(RGBCameraFrame iInput)
        {
            // Always clone the input matrix to avoid working with the matrix when the C++ part wants to modify it. It will crash.
            Mat lMatSrc = iInput.Mat.clone();
            // Flip to avoid mirror effect.
            Core.flip(lMatSrc, lMatSrc, 1);

            // Use matrice format, to scale the texture.
            mCamView = Utils.ScaleTexture2DFromMat(lMatSrc, mCamView);
            // Use matrice to fill the texture.
            Utils.MatToTexture2D(lMatSrc, mCamView);
            mCam.texture = mCamView;
        }
    }
}
