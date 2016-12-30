using UnityEngine;
using BuddyOS.App;
using System;
using BuddyFeature.Vision;
using BuddyOS;
using System.Collections.Generic;
using OpenCVUnity;

namespace BuddyApp.TakePhoto
{
	[RequireComponent(typeof(FaceCascadeTracker))]
	public class FindFace : AStateMachineBehaviour
	{

		private bool mFollowFace;
		private float mSearchTimer;
		private const float mTimeMax = 8.0f;
		private float mFaceLostTime;
		private int mSearchStep;

		private int mRGBCamWidthCenter;
		private int mRGBCamHeightCenter;

		private float mHeadYesAngle;
		private float mHeadNoAngle;

		private FaceCascadeTracker mFaceTracker;
		private List<OpenCVUnity.Rect> mTrackedObjects;

		public override void Init()
		{
			BYOS.Instance.VocalActivation.enabled = false;
			mSearchTimer = 0.0f;
		}

		protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mFollowFace = false;
			mSearchStep = 1;
			mRGBCam = BYOS.Instance.RGBCam;
			mFaceTracker = GetComponent<FaceCascadeTracker>();

			mTTS.SayKey("look4face");

			if (!mRGBCam.IsOpen)
				mRGBCam.Open();
		}

		protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mFollowFace) {
				// Center the face before escape
				mTrackedObjects = mFaceTracker.TrackedObjects;

				if (mTrackedObjects.Count > 0) {
					float lXCenter = 0.0f;
					float lYCenter = 0.0f;
					for (int i = 0; i < mTrackedObjects.Count; ++i) {
						lXCenter = mTrackedObjects[i].x + mTrackedObjects[i].width / 2;
						lYCenter = mTrackedObjects[i].y + mTrackedObjects[i].height / 2;
					}
					lXCenter = lXCenter / mTrackedObjects.Count;
					lYCenter = lYCenter / mTrackedObjects.Count;


					Debug.Log("Tracking face : XCenter " + lXCenter + " / YCenter " + lYCenter);


					if (!(mRGBCamWidthCenter - 25 < lXCenter && lXCenter < mRGBCamWidthCenter + 5))
						mHeadNoAngle -= Mathf.Sign(lXCenter - mRGBCamWidthCenter) * 1.5F;
					if (!(mRGBCamHeightCenter - 5 < lYCenter && lYCenter < mRGBCamHeightCenter + 25))
						mHeadYesAngle += Mathf.Sign(lYCenter - mRGBCamHeightCenter) * 1.5F;

					Debug.Log("Setting angles Yes : " + Mathf.Sign(lYCenter - mRGBCamHeightCenter) +
						" / No : " + Mathf.Sign(lXCenter - mRGBCamWidthCenter));

					mYesHinge.SetPosition(mHeadYesAngle);
					mNoHinge.SetPosition(mHeadNoAngle);


					if ((mRGBCamWidthCenter - 25 < lXCenter && lXCenter < mRGBCamWidthCenter + 5) && (mRGBCamHeightCenter - 5 < lYCenter && lYCenter < mRGBCamHeightCenter + 25)) {

						Debug.Log("Face centered");
						iAnimator.SetTrigger("Face");
					}
				} else {
					mFaceLostTime += Time.deltaTime;
					if (mFaceLostTime > 4.0f) {
						mFollowFace = false;
						Debug.Log("We loose the face");
						mFaceLostTime = 0.0f;
						mSearchTimer = 0.0f;
						mSearchStep = 1;
					}
				}


			} else {
				mSearchTimer += Time.deltaTime;

				if (mFaceTracker.TrackedObjects.Count > 0) {
					//Exit, follow face
					Debug.Log("face found!");
					mHeadNoAngle = mNoHinge.CurrentAnglePosition;
					mHeadYesAngle = mYesHinge.CurrentAnglePosition;
					mRGBCamWidthCenter = BYOS.Instance.RGBCam.Width / 2;
					mRGBCamHeightCenter = BYOS.Instance.RGBCam.Height / 2;
					mFollowFace = true;
					//iAnimator.SetTrigger("Face");
				} else {
					if (mSearchTimer > mTimeMax) {
						//Exit, no face
						iAnimator.SetTrigger("NoFace");
					} else {
						//move head to look for face
						if (mSearchTimer < 2.0f && mSearchStep == 1) {
							mHeadYesAngle = mYesHinge.CurrentAnglePosition - 20.0f;
							mYesHinge.SetPosition(mHeadYesAngle);
							mSearchStep++;
						} else if (mSearchTimer > 2.0f && mSearchStep == 2) {
							mHeadNoAngle = mNoHinge.CurrentAnglePosition - 25.0f;
							mNoHinge.SetPosition(mHeadNoAngle);
							mSearchStep++;
						} else if (mSearchTimer > 4.0f && mSearchStep == 3) {
							mHeadNoAngle = mNoHinge.CurrentAnglePosition + 50.0f;
							mNoHinge.SetPosition(mHeadNoAngle);
							mSearchStep++;
						} else if (mSearchTimer > 6.0f && mSearchStep == 4) {
							mNoHinge.SetPosition(0.0f);
							mSearchStep++;
						}
					}
				}
			}
		}

		protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}
