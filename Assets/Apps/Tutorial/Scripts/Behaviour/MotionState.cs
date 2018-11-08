using UnityEngine;
using BlueQuark;
using OpenCVUnity;

namespace BuddyApp.Tutorial
{
	/// <summary>
	/// In this state we display what buddy sees and we will detect motion
	/// </summary>
	public sealed class MotionState : AStateMachineBehaviour
	{
		private bool mDisplayed;
		private Mat mMatDetect;
		private Mat mMatFlip;
		private Texture2D mTextureCam;
		private Color mColorOfDisplay;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// This bool is used to know when the display is active
			mDisplayed = false;

			// This define a color for the circle for the detected motion
			mColorOfDisplay = new Color(255, 0, 0);

			// initialize the texture to avoid nullref and crashes
			mTextureCam = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);

			// This command is to make Buddy say the key from the dictionnary
			Buddy.Vocal.SayKey("motionstateintro");

			// We get the frame from the RGBCamera with the OnNewFrame
			Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));

			// We parameter the motion detection : regionOfInterest represents where the detection will be done in the current frame
			// sensibilityThreshold represents the threshold for the detection
			MotionDetectorParameter mMotionDetectorParameter = new MotionDetectorParameter();
			mMotionDetectorParameter.RegionOfInterest = new OpenCVUnity.Rect(0, 0, Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
			mMotionDetectorParameter.SensibilityThreshold = 2.5F;

			// OnDetect opens the camera itself so we don't have to do it. Default resolution is 640*480
			Buddy.Perception.MotionDetector.OnDetect.AddP(OnMovementDetected, mMotionDetectorParameter);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// If buddy is not listenning or speaking (i.e. we wait for the end of speech) and the display is not activated yet
			if (!Buddy.Vocal.IsBusy && !mDisplayed) {
				// We activate the display
				Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTextureCam, OnDisplayClicked);

				// We wait 500 ms then Buddy tells a random sentence from the dictionnary
				Buddy.Vocal.Say("[500]" + Buddy.Resources.GetRandomString("motionstatedisplayed"));
				mDisplayed = true;
			}
		}

		/// <summary>
		/// Callback for every frame updated from the camera
		/// </summary>
		/// <param name="iInput">The matrix returned by the camera</param>
		private void OnFrameCaptured(Mat iInput)
		{
			// Always clone the input matrix to avoid working with the matrix when the C++ part wants to modify it. It will crash.
			mMatDetect = iInput.clone();
			mMatFlip = iInput.clone();

			// We flip the image to have a "mirror" effect
			Core.flip(mMatFlip, mMatFlip, 1);

			// We convert the matrix into a texture
			mTextureCam = Utils.ScaleTexture2DFromMat(mMatFlip, mTextureCam);
			Utils.MatToTexture2D(mMatFlip, mTextureCam);
		}

		/// <summary>
		/// Callback when there is motions detected
		/// </summary>
		/// <param name="iMotions">The list of motion entities</param>
		/// <returns></returns>
		private bool OnMovementDetected(MotionEntity[] iMotions)
		{

			//Draw circle on every motions detected in the image.
			foreach (MotionEntity lEntity in iMotions) {
				Imgproc.circle(mMatDetect, Utils.Center(lEntity.RectInFrame), 3, new Scalar(mColorOfDisplay), 3);
			}

            // Give mirror effect to the mat with detection
            //More information from opencv doc for the flipcode : 
            // a flag to specify how to flip the array; 0 means flipping around the x-axis
            //and positive value (for example, 1) means flipping around y-axis. 
            //Negative value (for example, -1) means flipping around both axes (see the discussion  in opencv doc for the formulas).
            Core.flip(mMatDetect, mMatDetect, 1);

			// We convert the matrix into a texture
			mTextureCam = Utils.ScaleTexture2DFromMat(mMatDetect, mTextureCam);
			Utils.MatToTexture2D(mMatDetect, mTextureCam);

			return true;
		}

		private void OnDisplayClicked()
		{
            // When the user touches the screen, the app goes back to the menu,
            // so we hide the toast and trigger the menu transition
            Buddy.Perception.MotionDetector.OnDetect.RemoveP(OnMovementDetected);
            Buddy.GUI.Toaster.Hide();
			Trigger("MenuTrigger");
		}

	}
}

