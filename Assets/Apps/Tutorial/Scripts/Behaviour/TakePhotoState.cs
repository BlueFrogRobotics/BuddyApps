using OpenCVUnity;

using BlueQuark;

using UnityEngine;
using System;

namespace BuddyApp.Tutorial
{
	/// <summary>
	/// In this state we take a photo and then we save the photo in a folder and we show you how you can access to this photo
	/// </summary>
	public sealed class TakePhotoState : AStateMachineBehaviour
	{

		private Mat mMatSrc;
		private Texture2D mCameraTexture;
        private Photograph mPhoto;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// Buddy give random sentence from the dico as instruction
			Buddy.Vocal.SayKey("tpstateintro");

			// This following sentence will be spoken by the robot just after the first one
			Buddy.Vocal.SayKey("tpstatesteptakephoto", oOutput => {
				Buddy.Sensors.HDCamera.TakePhotograph(OnFinish, true);
			});

			// We open the camera and set the resolution and frame rate.
			Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);

			// We define the function to be called when a new frame is received
			// from the camera
			mCameraTexture = Utils.MatToTexture2D(Buddy.Sensors.HDCamera.Frame);
			Buddy.Sensors.HDCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));

			// We display the camera feedback
			Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mCameraTexture);
		}


		/// <summary>
		/// Callback for every frame updated to update the texture
		/// </summary>
		/// <param name="iInput"></param>
		private void OnFrameCaptured(Mat iInput)
		{
			Debug.Log("on new frame take photo");
			mMatSrc = iInput;

			// We flip the frame from camera to have the "mirror" effect
			Core.flip(mMatSrc, mMatSrc, 1);
			Utils.MatToTexture2D(mMatSrc, mCameraTexture);
		}

		/// <summary>
		/// Callback called when the photo is taken.
		/// </summary>
		/// <param name="iMyPhoto"></param>
		private void OnFinish(Photograph iMyPhoto)
		{
            Sprite lPhotoSprite;
            Debug.Log("1");
            lPhotoSprite = iMyPhoto.Image;
            Debug.Log("2");
            mPhoto = iMyPhoto;
            Debug.Log("3");
            Buddy.GUI.Toaster.Hide();
            // We display the picture while Buddy is still speaking
            Buddy.GUI.Toaster.Display<PictureToast>().With(lPhotoSprite, OnMenuTrigger);

            Debug.Log("lol");
            Buddy.Vocal.SayKey("tpstatephototaken", oOutput => {
				//User can find picture in persistentDataPath/Users/"name of the user"/AppData/"id of the app"/Pictures

				// Save the picture and go back to the menu
				mPhoto.Save();
			});

        }

        private void OnMenuTrigger()
        {
            Buddy.GUI.Toaster.Hide();
            Trigger("MenuTrigger");
        }
	}
}

