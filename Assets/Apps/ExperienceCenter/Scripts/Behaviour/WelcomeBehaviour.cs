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
		public const float ANGLE_THRESHOLD = 0.1f;

		private AnimatorManager mAnimatorManager;
		private TextToSpeech mTTS;
		private bool mHeadMoving;
		private bool mChangeDirection;
		private float mYesAngle;

		private AttitudeBehaviour mAttitudeBehaviour;

		void Awake ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find("AIBehaviour").GetComponent<AttitudeBehaviour>();
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
		}

		public void InitBehaviour ()
		{
			mHeadMoving = true;
			mChangeDirection = true;
			mYesAngle = 0;
			if (ExperienceCenterData.Instance.EnableMovement) 
				StartCoroutine(MoveHeadNoHinge(30,50));
			StartCoroutine(Speaking());
		}

		private IEnumerator Speaking ()
		{
			yield return new WaitUntil(() => !mHeadMoving || !ExperienceCenterData.Instance.EnableMovement);
			if (ExperienceCenterData.Instance.EnableMovement) 
				mAttitudeBehaviour.MoveHeadWhileSpeaking(-10, 10);

			mTTS.SayKey ("welcomebienvenue", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomeinvitation", true);
			mTTS.Silence(2000, true);
			mTTS.SayKey ("welcomemaison", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomefuture", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomeajd", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomemtnt", true);
			mTTS.Silence(2000, true);
			mTTS.SayKey ("welcomeoffre", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomeassez", true);
			mTTS.Silence(1000, true);

			yield return new WaitUntil(() => mTTS.HasFinishedTalking);
			if (ExperienceCenterData.Instance.EnableMovement) 
				StartCoroutine(MoveHeadNoHinge(0,50f));
			
			mTTS.SayKey ("welcomechoix", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomepose", true);
			mTTS.Silence(3000, true);
			mTTS.SayKey ("welcomeregarder", true);

			yield return new WaitUntil(() => mTTS.HasFinishedTalking);
			mAnimatorManager.ActivateCmd ((byte)(Command.Stop));
		}

		private IEnumerator MoveHeadYesHinge (float lYesAngle, float lYesSpeed)
		{
			yield return new WaitUntil(() => mHeadMoving);
			BYOS.Instance.Primitive.Motors.YesHinge.SetPosition (lYesAngle, lYesSpeed);
			yield return new WaitUntil(() => Math.Abs(BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition - lYesAngle) < ANGLE_THRESHOLD || !mHeadMoving);
			if (mHeadMoving && mChangeDirection) {
				mChangeDirection = false;
				StartCoroutine (MoveHeadYesHinge (0, lYesSpeed));
			}
		}

		private IEnumerator MoveHeadNoHinge (float lNoAngle, float lNoSpeed)
		{
			yield return new WaitUntil(() => mTTS.HasFinishedTalking);
			//Comment this line if you need a linear movement of the head 
			StartCoroutine(MoveHeadYesHinge(-5,50));
			mHeadMoving = true;
			BYOS.Instance.Primitive.Motors.NoHinge.SetPosition (lNoAngle, lNoSpeed);
			yield return new WaitUntil(() => Math.Abs(BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition - lNoAngle) < ANGLE_THRESHOLD);
			mHeadMoving = false;
			mChangeDirection = true;
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop Welcome Behaviour");
			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
			StopAllCoroutines ();
		}



	}
}