using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Buddy;


namespace BuddyApp.ExperienceCenter
{
	public class WelcomeBehaviour : MonoBehaviour
	{
		private const float ANGLE_THRESHOLD = 0.1f;

		private AnimatorManager mAnimatorManager;
		private AttitudeBehaviour mAttitudeBehaviour;
		private TextToSpeech mTTS;

		private bool mHeadMoving;

		private float mWelcomeTimeOut;
		private float mNoHingeSpeed;
		private float mNoHingeAngle;
		private float mHeadPoseTolerance;

		void Awake ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find ("AIBehaviour").GetComponent<AttitudeBehaviour> ();
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
		}

		public void InitBehaviour ()
		{
			mHeadMoving = true;

			if (mWelcomeTimeOut != ExperienceCenterData.Instance.WelcomeTimeOut) {
				mWelcomeTimeOut = ExperienceCenterData.Instance.WelcomeTimeOut; 
				Debug.LogWarningFormat ("[EXCENTER] Welcome TimeOut = {0} s ", mWelcomeTimeOut);
			}

			if (mNoHingeAngle != ExperienceCenterData.Instance.NoHingeAngle) {
				mNoHingeAngle = ExperienceCenterData.Instance.NoHingeAngle; 
				Debug.LogWarningFormat ("[EXCENTER] No Hinge Angle = {0} deg ", mNoHingeAngle);
			}

			if (mNoHingeSpeed != ExperienceCenterData.Instance.NoHingeSpeed) {
				mNoHingeSpeed = ExperienceCenterData.Instance.NoHingeSpeed; 
				Debug.LogWarningFormat ("[EXCENTER] No Hinge Speed = {0} deg/s ", mNoHingeSpeed);
			}

			if (mHeadPoseTolerance != ExperienceCenterData.Instance.HeadPoseTolerance) {
				mHeadPoseTolerance = ExperienceCenterData.Instance.HeadPoseTolerance; 
				Debug.LogWarningFormat ("[EXCENTER] Head Pose Tolerance = {0} deg ", mHeadPoseTolerance);
			}

			if (ExperienceCenterData.Instance.EnableHeadMovement)
				StartCoroutine (MoveHeadNoHinge (mNoHingeAngle, mNoHingeSpeed));
			StartCoroutine (Speaking ());
		}

		private IEnumerator Speaking ()
		{ 
			DateTime lLastTime = DateTime.Now;
			float lHeadAngle = BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition;
			while (mHeadMoving && ExperienceCenterData.Instance.EnableHeadMovement) {
				Debug.LogWarningFormat ("[EXCENTER] NoHinge (velocity = {0}, angle = {1}, old_angle = {2}) ", BYOS.Instance.Primitive.Motors.NoHinge.TargetSpeed, BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition, lHeadAngle);
				if (Math.Abs (BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition) < 0.1 || Math.Abs (BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition - lHeadAngle) <= mHeadPoseTolerance) {
					TimeSpan lElapsedTime = DateTime.Now - lLastTime;
					if (lElapsedTime.TotalSeconds > mWelcomeTimeOut)
						break;
				} else {
					Debug.LogWarning ("[EXCENTER] Robot Head is moving");
				}
				lHeadAngle = BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition;
				yield return new WaitForSeconds (0.5f);
			}
			if (!mHeadMoving)
				Debug.LogWarning ("[EXCENTER] Robot Head reaches angle");

			if (ExperienceCenterData.Instance.EnableHeadMovement)
				mAttitudeBehaviour.MoveHeadWhileSpeaking (-10, 10);

			mTTS.SayKey ("welcomebienvenue", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("welcomeinvitation", true);
			mTTS.Silence (2000, true);
			mTTS.SayKey ("welcomemaison", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("welcomefuture", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("welcomeajd", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("welcomemtnt", true);
			mTTS.Silence (2000, true);
			mTTS.SayKey ("welcomeoffre", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("welcomeassez", true);
			mTTS.Silence (1000, true);

			yield return new WaitUntil (() => mTTS.HasFinishedTalking);
			if (ExperienceCenterData.Instance.EnableHeadMovement)
				StartCoroutine (MoveHeadNoHinge (0, mNoHingeSpeed));
			
			mTTS.SayKey ("welcomechoix", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("welcomepose", true);
			mTTS.Silence (3000, true);
			mTTS.SayKey ("welcomeregarder", true);

			yield return new WaitUntil (() => mTTS.HasFinishedTalking);
			mAnimatorManager.ActivateCmd ((byte)(Command.Stop));
		}

		private IEnumerator MoveHeadNoHinge (float lNoAngle, float lNoSpeed)
		{
			yield return new WaitUntil (() => mTTS.HasFinishedTalking);
			yield return new WaitUntil (() => BYOS.Instance.Interaction.BMLManager.DonePlaying);

			mHeadMoving = true;
			BYOS.Instance.Primitive.Motors.NoHinge.SetPosition (lNoAngle, lNoSpeed);
			yield return new WaitUntil (() => Math.Abs (BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition - lNoAngle) < ANGLE_THRESHOLD);
			mHeadMoving = false;
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("[EXCENTER] Stop Welcome Behaviour");
			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
			StopAllCoroutines ();
		}

	}
}