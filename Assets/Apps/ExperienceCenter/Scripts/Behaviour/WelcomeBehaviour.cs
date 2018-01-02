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

		void Awake ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
		}

		public void InitBehaviour ()
		{
			mHeadMoving = true;
			mChangeDirection = true;
			mYesAngle = 0;
			StartCoroutine(MoveHeadNoHinge(30,15));
			StartCoroutine(Speaking());
			StartCoroutine(MoveHeadWhenSpeaking());
		}



		private IEnumerator Speaking ()
		{
			yield return new WaitUntil(() => !mHeadMoving);
			Debug.LogWarning ("Here Speak ");
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
			StartCoroutine(MoveHeadNoHinge(0,15f));
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
			Debug.LogWarning ("Here Move ");
			//Comment this line if you need a linear movement of the head 
			StartCoroutine(MoveHeadYesHinge(-5,5));
			mHeadMoving = true;
			BYOS.Instance.Primitive.Motors.NoHinge.SetPosition (lNoAngle, lNoSpeed);
			yield return new WaitUntil(() => Math.Abs(BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition - lNoAngle) < ANGLE_THRESHOLD);
			mHeadMoving = false;
			mChangeDirection = true;
		}

		private IEnumerator MoveHeadWhenSpeaking ()
		{
			if(mTTS.HasFinishedTalking) BYOS.Instance.Primitive.Motors.YesHinge.SetPosition (0, 15f);
			yield return new WaitUntil(() => !mTTS.HasFinishedTalking);
			float lYesAngle = BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition;
			lYesAngle = lYesAngle - (float) (3 * Math.Sin(mYesAngle));
			BYOS.Instance.Primitive.Motors.YesHinge.SetPosition (lYesAngle, 15f);
			mYesAngle += UnityEngine.Random.Range(1.0F, 9.0F);
			yield return new WaitUntil(() => Math.Abs(BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition - lYesAngle) < ANGLE_THRESHOLD || mTTS.HasFinishedTalking);
			StartCoroutine (MoveHeadWhenSpeaking ());
		}

		public void StopBehaviour ()
		{
			mTTS.Stop ();
			StopAllCoroutines ();
		}

	}
}