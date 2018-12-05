using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BlueQuark;


namespace BuddyApp.ExperienceCenter
{
	public class WelcomeBehaviour : MonoBehaviour
	{
		private const float ANGLE_THRESHOLD = 0.1f;

		private AnimatorManager mAnimatorManager;
		private AttitudeBehaviour mAttitudeBehaviour;
		//private TextToSpeech mTTS;

		private bool mHeadMoving;

		private float mWelcomeTimeOut;
		private float mNoHingeSpeed;
		private float mNoHingeAngle;
		private float mHeadPoseTolerance;

		void Awake ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find ("AIBehaviour").GetComponent<AttitudeBehaviour> ();
			//mTTS = BYOS.Instance.Interaction.TextToSpeech;
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
            float lHeadAngle = Buddy.Actuators.Head.No.Angle;//.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition;
			while (mHeadMoving && ExperienceCenterData.Instance.EnableHeadMovement) {
				Debug.LogWarningFormat ("[EXCENTER] NoHinge (velocity = {0}, angle = {1}, old_angle = {2}) ", Buddy.Actuators.Head.No.Speed, Buddy.Actuators.Head.No.Angle, lHeadAngle);
				if (Math.Abs (Buddy.Actuators.Head.No.Angle) < 0.1 || Math.Abs (Buddy.Actuators.Head.No.Angle - lHeadAngle) <= mHeadPoseTolerance) {
					TimeSpan lElapsedTime = DateTime.Now - lLastTime;
					if (lElapsedTime.TotalSeconds > mWelcomeTimeOut)
						break;
				} else {
					Debug.LogWarning ("[EXCENTER] Robot Head is moving");
				}
				lHeadAngle = Buddy.Actuators.Head.No.Angle;
				yield return new WaitForSeconds (0.5f);
			}
			if (!mHeadMoving)
				Debug.LogWarning ("[EXCENTER] Robot Head reaches angle");

			if (ExperienceCenterData.Instance.EnableHeadMovement)
				mAttitudeBehaviour.MoveHeadWhileSpeaking (-10, 10);

			Buddy.Vocal.SayKey ("welcomebienvenue", true);
            Buddy.Vocal.Say("[500]", true);
			Buddy.Vocal.SayKey ("welcomeinvitation", true);
            Buddy.Vocal.Say("[2000]", true);
            Buddy.Vocal.SayKey ("welcomemaison", true);
            Buddy.Vocal.Say("[500]", true);
            Buddy.Vocal.SayKey ("welcomefuture", true);
            Buddy.Vocal.Say("[500]", true);
            Buddy.Vocal.SayKey ("welcomeajd", true);
            Buddy.Vocal.Say("[500]", true);
            Buddy.Vocal.SayKey ("welcomemtnt", true);
            Buddy.Vocal.Say("[2000]", true);
			Buddy.Vocal.SayKey ("welcomeoffre", true);
            Buddy.Vocal.Say("[500]", true);
            Buddy.Vocal.SayKey ("welcomeassez", true);
            Buddy.Vocal.Say("[1000]", true);

            yield return new WaitUntil (() => !Buddy.Vocal.IsSpeaking);
			if (ExperienceCenterData.Instance.EnableHeadMovement)
				StartCoroutine (MoveHeadNoHinge (0, mNoHingeSpeed));
			
			Buddy.Vocal.SayKey ("welcomechoix", true);
            Buddy.Vocal.Say("[500]", true);
            Buddy.Vocal.SayKey ("welcomepose", true);
            Buddy.Vocal.Say("[3000]", true);
            Buddy.Vocal.SayKey ("welcomeregarder", true);

			yield return new WaitUntil (() => !Buddy.Vocal.IsSpeaking);
			mAnimatorManager.ActivateCmd ((byte)(Command.Stop));
		}

		private IEnumerator MoveHeadNoHinge (float lNoAngle, float lNoSpeed)
		{
			yield return new WaitUntil (() => !Buddy.Vocal.IsSpeaking);
			//yield return new WaitUntil (() => BYOS.Instance.Interaction.BMLManager.DonePlaying);

			mHeadMoving = true;
            Buddy.Actuators.Head.No.SetPosition (lNoAngle, lNoSpeed);
			yield return new WaitUntil (() => Math.Abs (Buddy.Actuators.Head.No.Angle - lNoAngle) < ANGLE_THRESHOLD);
			mHeadMoving = false;
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("[EXCENTER] Stop Welcome Behaviour");
			if (Buddy.Vocal.IsSpeaking)
                Buddy.Vocal.StopListening ();
			StopAllCoroutines ();
		}

	}
}