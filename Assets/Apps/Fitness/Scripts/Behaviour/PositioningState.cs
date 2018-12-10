using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using OpenCVUnity;
using UnityEngine.UI;

namespace BuddyApp.Fitness
{
	public sealed class PositionningState : AStateMachineBehaviour
	{
		// Coefficient to adjust all skeleton point, on the image.
		private float COEFF_X;
		private float COEFF_Y;

		private const float SKELETON_DETECT_TIME = 5F;

		private const int MAX_SKELETON_DETECT = 1;

		// List of Skeleton detect
		private List<SkeletonJoint[]> mSkeletonList;
		private float mDetectTimeStamp;

		// Variable to avoid multiple display
		private bool mDisplayed;
		// This texture will be filled with the camera data
		private Texture2D mCamView;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			// TODO: Buddy gives instruction

			COEFF_X = 1.7F;
			COEFF_Y = 2.45F;


			mDisplayed = false;

			mDetectTimeStamp = -1;

			mSkeletonList = new List<SkeletonJoint[]>();

			// Skeleton detection doesn't open the camera by default
			Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320x240_30FPS_RGB);
			Buddy.Perception.SkeletonDetector.OnDetect.AddP(OnSkeletonDetectPos);

			// Initialize texture.
			mCamView = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
			// Setting of the callback to use camera data
			Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));

			// Hide the default parameter button.
			Buddy.GUI.Header.DisplayParametersButton(false);
			// Set Title with a custom font
			Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
			lHeaderFont.material.color = Color.black;
			Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
			Buddy.GUI.Header.DisplayLightTitle("position");
			Buddy.Vocal.SayKey("position");
		}

		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// If the observation time is reach, go to the settings states.
			if (mDetectTimeStamp != -1 && (Time.time - mDetectTimeStamp) >= SKELETON_DETECT_TIME) {
				if (!Buddy.Behaviour.IsBusy)
					Trigger("TRAINING");
			}

			// Video Mode: Display the camera view with a visual of detection.
			if (!mDisplayed) {
				Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mCamView);
				mDisplayed = true;
			}
		}

		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mSkeletonList.Clear();
			Buddy.GUI.Header.HideTitle();
			Buddy.GUI.Toaster.Hide();
			Buddy.Sensors.RGBCamera.Close();
			Buddy.Perception.SkeletonDetector.OnDetect.Clear();
		}

		//  -----CALLBACK------  //

		// On each frame captured by the camera this function is called, with the matrix of pixel.
		private void OnFrameCaptured(RGBCameraFrame iInput)
		{
			// Always clone the input matrix, this avoid to working with the original matrix, when the C++ part wants to modify it.
			Mat lMatSrc = iInput.Mat.clone();

			// Drawing each links between skeleton joints.
			DrawSkeletonLinks(lMatSrc);

			// Flip to avoid mirror effect.
			Core.flip(lMatSrc, lMatSrc, 1);
			// Use matrice format, to scale the texture.
			mCamView = Utils.ScaleTexture2DFromMat(lMatSrc, mCamView);
			// Use matrice to fill the texture.
			Utils.MatToTexture2D(lMatSrc, mCamView);
		}

		// This function use all joints of a skeleton and draw a line between them
		private void DrawSkeletonLinks(Mat iMat)
		{
			int lSkeletonCount = 0;
			if (Buddy.Sensors.RGBCamera.IsOpen) {
				// We browse the skeleton list
				foreach (var lSkeleton in mSkeletonList) {
					// Number max of skeleton to use for drawing
					if (lSkeletonCount > MAX_SKELETON_DETECT)
						break;
					lSkeletonCount++;
					int lWidth = iMat.cols();
					int lHeight = iMat.rows();

					// Create dictionnary with name to skeletonjoint
					Dictionary<SkeletonJointType, SkeletonJoint> lNameToJoint = new Dictionary<SkeletonJointType, SkeletonJoint>();

					// We browse all joints of the current skeleton
					foreach (var lJoint in lSkeleton) {
						lNameToJoint[lJoint.Type] = lJoint;
						// Draw a circle with the joint point as center
						// 10 is a constant, choose after some test, to get a base for the size.
						// Divide the z coordinate by 1000 to get value in millimeter
						// The result of the pow operation is used to divide a constant, so adding 0.1 avoid a zero division.
						// The pow operation purpose, is to increase the influence of the depth

						Point lCenter = new Point(iMat.cols() / 2, iMat.rows() / 2);
						// Calcul the local position of the joint
						Point lLocal = new Point(lJoint.WorldPosition.x / lJoint.WorldPosition.z, lJoint.WorldPosition.y / lJoint.WorldPosition.z);
						// Conversion of the local position, in the img
						lLocal.x *= COEFF_X * iMat.cols() / 2;
						lLocal.y *= COEFF_Y * iMat.rows() / 2;
						Imgproc.circle(iMat, lCenter - lLocal, (int)(10 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)), new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));

					}


					#region JOINTS_LINKED
					if (lNameToJoint.ContainsKey(SkeletonJointType.HEAD) && lNameToJoint.ContainsKey(SkeletonJointType.SHOULDER_SPINE))
						DrawLine(iMat, lNameToJoint[SkeletonJointType.HEAD], lNameToJoint[SkeletonJointType.SHOULDER_SPINE]);

					// Right Arm
					if (lNameToJoint.ContainsKey(SkeletonJointType.SHOULDER_SPINE)) {
						if (lNameToJoint.ContainsKey(SkeletonJointType.RIGHT_SHOULDER)) {
							DrawLine(iMat, lNameToJoint[SkeletonJointType.SHOULDER_SPINE], lNameToJoint[SkeletonJointType.RIGHT_SHOULDER]);
							if (lNameToJoint.ContainsKey(SkeletonJointType.RIGHT_ELBOW)) {
								DrawLine(iMat, lNameToJoint[SkeletonJointType.RIGHT_SHOULDER], lNameToJoint[SkeletonJointType.RIGHT_ELBOW]);
								if (lNameToJoint.ContainsKey(SkeletonJointType.RIGHT_WRIST)) {
									DrawLine(iMat, lNameToJoint[SkeletonJointType.RIGHT_ELBOW], lNameToJoint[SkeletonJointType.RIGHT_WRIST]);
									if (lNameToJoint.ContainsKey(SkeletonJointType.RIGHT_HAND)) {
										DrawLine(iMat, lNameToJoint[SkeletonJointType.RIGHT_WRIST], lNameToJoint[SkeletonJointType.RIGHT_HAND]);
									}
								}
							}
						}

						// Left Arm
						if (lNameToJoint.ContainsKey(SkeletonJointType.LEFT_SHOULDER)) {
							DrawLine(iMat, lNameToJoint[SkeletonJointType.SHOULDER_SPINE], lNameToJoint[SkeletonJointType.LEFT_SHOULDER]);
							if (lNameToJoint.ContainsKey(SkeletonJointType.LEFT_ELBOW)) {
								DrawLine(iMat, lNameToJoint[SkeletonJointType.LEFT_SHOULDER], lNameToJoint[SkeletonJointType.LEFT_ELBOW]);
								if (lNameToJoint.ContainsKey(SkeletonJointType.LEFT_WRIST)) {
									DrawLine(iMat, lNameToJoint[SkeletonJointType.LEFT_ELBOW], lNameToJoint[SkeletonJointType.LEFT_WRIST]);
									if (lNameToJoint.ContainsKey(SkeletonJointType.LEFT_HAND)) {
										DrawLine(iMat, lNameToJoint[SkeletonJointType.LEFT_WRIST], lNameToJoint[SkeletonJointType.LEFT_HAND]);
									}
								}
							}
						}

						// Mid spine
						if (lNameToJoint.ContainsKey(SkeletonJointType.MID_SPINE)) {
							DrawLine(iMat, lNameToJoint[SkeletonJointType.SHOULDER_SPINE], lNameToJoint[SkeletonJointType.MID_SPINE]);
						}
					}

					// Mid Spine
					if (lNameToJoint.ContainsKey(SkeletonJointType.MID_SPINE)) {
						if (lNameToJoint.ContainsKey(SkeletonJointType.BASE_SPINE))
							DrawLine(iMat, lNameToJoint[SkeletonJointType.MID_SPINE], lNameToJoint[SkeletonJointType.BASE_SPINE]);
					}

					if (lNameToJoint.ContainsKey(SkeletonJointType.BASE_SPINE)) {
						if (lNameToJoint.ContainsKey(SkeletonJointType.RIGHT_HIP)) {
							DrawLine(iMat, lNameToJoint[SkeletonJointType.BASE_SPINE], lNameToJoint[SkeletonJointType.RIGHT_HIP]);
							if (lNameToJoint.ContainsKey(SkeletonJointType.RIGHT_KNEE)) {
								DrawLine(iMat, lNameToJoint[SkeletonJointType.RIGHT_HIP], lNameToJoint[SkeletonJointType.RIGHT_KNEE]);
								if (lNameToJoint.ContainsKey(SkeletonJointType.RIGHT_FOOT)) {
									DrawLine(iMat, lNameToJoint[SkeletonJointType.RIGHT_KNEE], lNameToJoint[SkeletonJointType.RIGHT_FOOT]);
								}
							}
						}

						if (lNameToJoint.ContainsKey(SkeletonJointType.LEFT_HIP)) {
							DrawLine(iMat, lNameToJoint[SkeletonJointType.BASE_SPINE], lNameToJoint[SkeletonJointType.LEFT_HIP]);
							if (lNameToJoint.ContainsKey(SkeletonJointType.LEFT_KNEE)) {
								DrawLine(iMat, lNameToJoint[SkeletonJointType.LEFT_HIP], lNameToJoint[SkeletonJointType.LEFT_KNEE]);
								if (lNameToJoint.ContainsKey(SkeletonJointType.LEFT_FOOT)) {
									DrawLine(iMat, lNameToJoint[SkeletonJointType.LEFT_KNEE], lNameToJoint[SkeletonJointType.LEFT_FOOT]);
								}
							}
						}
					}
					#endregion
				}
			}
		}

		private void DrawLine(Mat iMat, SkeletonJoint iSkeletonJoint1, SkeletonJoint iSkeletonJoint2)
		{

			Point lCenter = new Point(iMat.cols() / 2, iMat.rows() / 2);
			// Calcul the local position of the joint
			Point lLocal1 = new Point(iSkeletonJoint1.WorldPosition.x / iSkeletonJoint1.WorldPosition.z, iSkeletonJoint1.WorldPosition.y / iSkeletonJoint1.WorldPosition.z);
			Point lLocal2 = new Point(iSkeletonJoint2.WorldPosition.x / iSkeletonJoint2.WorldPosition.z, iSkeletonJoint2.WorldPosition.y / iSkeletonJoint2.WorldPosition.z);
			// Conversion of the local position, in the img
			lLocal1.x *= COEFF_X * iMat.cols() / 2;
			lLocal1.y *= COEFF_Y * iMat.rows() / 2;

			lLocal2.x *= COEFF_X * iMat.cols() / 2;
			lLocal2.y *= COEFF_Y * iMat.rows() / 2;

			Imgproc.line(iMat, lCenter - lLocal1, lCenter - lLocal2, new Scalar(100, 0, 0), (int)(8 / Math.Pow(iSkeletonJoint1.WorldPosition.z / 1000F + 0.1, 2)).Clamp(0.5, 32));
		}


		/*
		*   On a skeleton detection this function is called.
		*/
		private bool OnSkeletonDetectPos(SkeletonEntity[] iSkeleton)
		{
			if (mDetectTimeStamp == -1F) {
				// TODO, say that Buddy detect skeleton
				Buddy.Behaviour.Face.PlayEvent(FacialEvent.WHISTLE);
				mDetectTimeStamp = Time.time;
			}


			mSkeletonList.Clear();

			// We add each skeleton to a list, to display them later in OnNewFrame
			foreach (SkeletonEntity lSkeleton in iSkeleton) {
				mSkeletonList.Add(lSkeleton.Joints);
			}

			return true;
		}
	}
}
