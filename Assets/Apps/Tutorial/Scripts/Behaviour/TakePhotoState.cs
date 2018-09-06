using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using OpenCVUnity;

namespace BuddyApp.Tutorial
{
    /// <summary>
    /// In this state we take a photo and then we save the photo in a folder and we show you how you can access to this photo
    /// </summary>
    public class TakePhotoState : AStateMachineBehaviour
    {
        private Mat mMatSrc;
        private Texture2D mTextPhoto;
        private Sprite mSpritePhoto;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Vocal.SayKey("tpstateintro");
            if (Buddy.Sensors.HDCamera.IsBusy)
            {
                Buddy.Sensors.HDCamera.Close();
            }
            if (!Buddy.Sensors.HDCamera.IsOpen)
            {
                Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);
            }

            Buddy.Sensors.HDCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
           
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private void OnFrameCaptured(Mat iInput)
        {
            mMatSrc = iInput;
            mTextPhoto = Utils.MatToTexture2D(mMatSrc);
            mSpritePhoto = Sprite.Create(mTextPhoto, new UnityEngine.Rect(0, 0, mTextPhoto.width, mTextPhoto.height), new Vector2(0.5F, 0.5F));
            Core.flip(mMatSrc, mMatSrc, 1);
            if (!Buddy.Vocal.IsSpeaking && !Buddy.GUI.Toaster.IsBusy)
                Buddy.GUI.Toaster.Display<toast>().With(mSpritePhoto);
        }
    }
}

