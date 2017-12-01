using System.Collections;
using Buddy;
using OpenCVUnity;
using UnityEngine;
using Buddy.UI;

namespace BuddyApp.TakePhoto
{
	public class LookForUser : AStateMachineBehaviour
	{
		private MotionDetection mMotion;
		private RGBCam mCam;
		private float mTimerLimit;
		private bool mHasTriggered;
		private bool mHasShowWindow;
		private Mat mMat;
		private Texture2D mTexture;
		private int mDetectionCount;
		private Mat mMatDetection;
		private float meanX;
		private float meanY;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mMotion = Perception.Motion;
			mMotion.enabled = true;
			mCam = Primitive.RGBCam;
			mMotion.OnDetect(OnMovementDetected, 4F);
			mMatDetection = null;
			mDetectionCount = 0;
			mHasTriggered = false;
			mHasShowWindow = false;
			mTimerLimit = 0F;
			meanX = 0F;
			meanY = 0F;
			mCam.Resolution = RGBCamResolution.W_320_H_240;

			Primitive.Motors.NoHinge.SetPosition(0F, 200F);
			Primitive.Motors.YesHinge.SetPosition(0F, 200F);

			//StartCoroutine(Defeat());
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mTimerLimit += Time.deltaTime;
			if (!mHasTriggered) {
				if (!mHasShowWindow && mCam.IsOpen && mCam.Width > 0 && !Interaction.TextToSpeech.IsSpeaking) {
					mTimerLimit = 0F;
					mHasShowWindow = true;

					mMat = mCam.FrameMat;
					mTexture = Utils.MatToTexture2D(mMat);
					Toaster.Display<PictureToast>().With("movehands", Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f)));
				}
				if (mDetectionCount <= 200 && mHasShowWindow) {
					if (mMatDetection == null) {
						mMat = mCam.FrameMat.clone();//Utils.Texture2DToMat(mTexture, OpenCVUnity.CvType.CV_8UC3);
						//Imgproc.rectangle(mMat, new Point((int)(mMat.width() / 3), 0), new Point((int)(mMat.width() * 2 / 3), mMat.height()), new Scalar(255, 0, 0), 3);
						Texture2D lTexture = Utils.MatToTexture2D(mMat);
						mTexture.SetPixels(lTexture.GetPixels());
					} else {
						Texture2D lTexture = Utils.MatToTexture2D(mMatDetection);
						mTexture.SetPixels(lTexture.GetPixels());
						mMatDetection = null;
					}

					mTexture.Apply();
				}
				if (mTimerLimit > 10F && mDetectionCount < 30 && mHasShowWindow) {
					Debug.Log("Too long, takephoto anyway");
					Interaction.Mood.Set(MoodType.NEUTRAL);
					TakePhoto();
				} else if (mDetectionCount >= 200) {
					Debug.Log("takephoto 30 motions");
					Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_1);
					Interaction.Mood.Set(MoodType.HAPPY);
					Position();
				}
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Interaction.Mood.Set(MoodType.NEUTRAL);
			mMotion.StopOnDetect(OnMovementDetected);
			//Toaster.Hide();
		}


		private bool OnMovementDetected(MotionEntity[] iMotions)
		{
			//Debug.Log("detection mouvement");
			if (iMotions.Length > 5 && !mHasTriggered && mHasShowWindow && mTimerLimit > 3F) {
				BYOS.Instance.Primitive.Speaker.FX.Play(FXSound.BEEP_1);
				mMatDetection = mCam.FrameMat.clone();//Utils.Texture2DToMat(mTexture, OpenCVUnity.CvType.CV_8UC3);
													  //Imgproc.rectangle(mMatDetection, new Point((int)(mMatDetection.width() / 3), 0), new Point((int)(mMatDetection.width() * 2 / 3), mMatDetection.height()), new Scalar(255, 0, 0), 3);
				MotionBlob[] lBlobs = iMotions.GetBlobs();
				MotionBlob lMainBlob = iMotions.GetMainBlob(lBlobs);
				foreach (MotionEntity lEntity in lMainBlob.MotionEntityArray) {
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
			Toaster.Hide();
			Trigger("Photo");
		}

		private void Position()
		{
			mHasTriggered = true;
			Toaster.Hide();

			//Set head position
			meanX /= mDetectionCount;
			meanY /= mDetectionCount;

			// - because reversed image
			float lAngle = -(meanX / mCam.Width - 0.5F) * 120F;
			
			Primitive.Motors.NoHinge.SetPosition(lAngle, 200F);
			Primitive.Motors.YesHinge.SetPosition((meanY / mCam.Height - 0.5F) * 40F, 200F);

			Debug.Log("MoveSideStimulus meanX, meanX/ mMatDetection.width(): " + meanX + "   " + meanX / mCam.Width);
			Debug.Log("MoveSideStimulus head to x + x*90: " + (meanX / mCam.Width - 0.5F) + " " + lAngle);
			Debug.Log("MoveSideStimulus head to y: " + (meanY / mCam.Height - 0.5F) * 40F);
			
			Trigger("Photo");
		}


	}

}