using System.Collections;
using OpenCVUnity;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.TakePhoto
{
	public sealed class LookForUser : AStateMachineBehaviour
	{
		private MotionDetector mMotion;
		private HDCamera mCam;
		private float mTimerLimit;
		private bool mHasTriggered;
		private bool mHasShowWindow;
		private Mat mMat;
		private Texture2D mTexture;
		private int mDetectionCount;
		private Mat mMatDetection;
		private float meanX;
		private float meanY; 

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mMotion = Buddy.Perception.MotionDetector;
			//mMotion.enabled = true;
			mCam = Buddy.Sensors.HDCamera;
           // mMotion.OnDetect.AddP((iInput) => { OnMovementDetected(iInput); });
            mMotion.OnDetect.AddP(OnMovementDetected);
			//mMotion.OnDetect(OnMovementDetected, 4F);
			mMatDetection = null;
			mDetectionCount = 0;
			mHasTriggered = false;
			mHasShowWindow = false;
			mTimerLimit = 0F;
			meanX = 0F;
			meanY = 0F;
			mCam.Mode = HDCameraMode.COLOR_320x240_30FPS_RGB;

			Buddy.Actuators.Head.No.SetPosition(0F, 200F);
			Buddy.Actuators.Head.Yes.SetPosition(0F, 200F);

			mMat = new Mat();

			//StartCoroutine(Defeat());
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimerLimit += Time.deltaTime;
			if (!mHasTriggered) {
				if (!mHasShowWindow && mCam.IsOpen && mCam.Width > 0 && !Buddy.Vocal.IsSpeaking) {
					mTimerLimit = 0F;
					mHasShowWindow = true;

					Mat mMatSrc = mCam.Frame.Mat;
					Core.flip(mMatSrc, mMat, 1);
					mTexture = Utils.MatToTexture2D(mMat);
					//Toaster.Display<PictureToast>().With(Dictionary.GetString("movehands"), Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f)));
                    Buddy.GUI.Toaster.Display<PictureToast>().With(Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f)));
				}
				if (mDetectionCount <= 200 && mHasShowWindow) {
					if (mMatDetection == null) {
						
						Mat mMatSrc = mCam.Frame.Mat.clone();
						Core.flip(mMatSrc, mMat, 1);
						Texture2D lTexture = Utils.MatToTexture2D(mMat);
						mTexture.SetPixels(lTexture.GetPixels());
					} else {
						Mat mMatSrc = mMatDetection;
						Core.flip(mMatSrc, mMat, 1);
						Texture2D lTexture = Utils.MatToTexture2D(mMat);
						mTexture.SetPixels(lTexture.GetPixels());
						mMatDetection = null;
					}

					mTexture.Apply();
				}
				if (mTimerLimit > 10F && mDetectionCount < 30 && mHasShowWindow) {
					Debug.Log("Too long, takephoto anyway");
					Buddy.Behaviour.SetMood(Mood.NEUTRAL);
					TakePhoto();
				} else if (mDetectionCount >= 200) {
					Debug.Log("takephoto 30 motions");
					Buddy.Actuators.Speakers.Media.Play(SoundSample.SURPRISED_1);
					Buddy.Behaviour.SetMood(Mood.HAPPY);
					Position();
				}
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Buddy.Behaviour.SetMood(Mood.NEUTRAL);
			//mMotion.StopOnDetect(OnMovementDetected);
            
			//Toaster.Hide();
		}


		private bool OnMovementDetected(MotionEntity[] iMotions)
		{
			//Debug.Log("detection mouvement");
			if (iMotions.Length > 5 && !mHasTriggered && mHasShowWindow && mTimerLimit > 3F) {
			    Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
				mMatDetection = mCam.Frame.Mat.clone();//Utils.Texture2DToMat(mTexture, OpenCVUnity.CvType.CV_8UC3);
													  //Imgproc.rectangle(mMatDetection, new Point((int)(mMatDetection.width() / 3), 0), new Point((int)(mMatDetection.width() * 2 / 3), mMatDetection.height()), new Scalar(255, 0, 0), 3);
				//MotionBlob[] lBlobs = iMotions.GetBlobs();
                
				//MotionBlob lMainBlob = iMotions.GetMainBlob(lBlobs);

				foreach (MotionEntity lEntity in /*lMainBlob.MotionEntityArray*/ iMotions) {
					Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 6, new Scalar(0, 255, 255), 6);

					// TODO Remove points that are too far from main motion
					meanX += lEntity.RectInFrame.x;
					meanY += lEntity.RectInFrame.y;

					mDetectionCount++;
				}
			}
			//mTexture = Utils.MatToTexture2D(mMat);
			return true;
		}


		private void TakePhoto()
		{

			mHasTriggered = true;
			Buddy.GUI.Toaster.Hide();
			Trigger("Photo");
		}

		private void Position()
		{
			mHasTriggered = true;
			Buddy.GUI.Toaster.Hide();

			//Set head position
			meanX /= mDetectionCount;
			meanY /= mDetectionCount;

			// - because reversed image
			float lAngle = -(meanX / mCam.Width - 0.5F) * 120F;
			
			Buddy.Actuators.Head.No.SetPosition(lAngle, 200F);
			Buddy.Actuators.Head.Yes.SetPosition((meanY / mCam.Height - 0.5F) * 40F, 200F);

			Debug.Log("MoveSideStimulus meanX, meanX/ mMatDetection.width(): " + meanX + "   " + meanX / mCam.Width);
			Debug.Log("MoveSideStimulus head to x + x*90: " + (meanX / mCam.Width - 0.5F) + " " + lAngle);
			Debug.Log("MoveSideStimulus head to y: " + (meanY / mCam.Height - 0.5F) * 40F);
			
			Trigger("Photo");
		}

		//Texture2D FlipTexture(Texture2D iOriginal)
		//{
		//	Texture2D lFlipped = new Texture2D(iOriginal.width, iOriginal.height);

		//	int xN = iOriginal.width;
		//	int yN = iOriginal.height;


		//	for (int i = 0; i < xN; i++) {
		//		for (int j = 0; j < yN; j++) {
		//			lFlipped.SetPixel(xN - i - 1, j, iOriginal.GetPixel(i, j));
		//		}
		//	}
		//	lFlipped.Apply();

		//	return lFlipped;
		//}


	}

}