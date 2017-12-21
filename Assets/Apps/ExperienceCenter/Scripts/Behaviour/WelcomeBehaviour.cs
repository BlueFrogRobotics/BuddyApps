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
		private AnimatorManager mAnimatorManager;
		private TextToSpeech mTTS;
		private bool mHeadMoving = true;
		private bool mChangeDirection = true;
		private float mYesAngle = 0;

		void Awake ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
		}

		public void InitBehaviour ()
		{
			StartCoroutine(MoveHeadNoHinge(30,15));
			StartCoroutine(Speaking());
			StartCoroutine(MoveHeadWhenSpeaking());
		}


		private IEnumerator Speaking ()
		{
			yield return new WaitWhile(() => mHeadMoving);
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

			yield return new WaitWhile(() => !mTTS.HasFinishedTalking);
			StartCoroutine(MoveHeadNoHinge(0,15f));
			mTTS.SayKey ("welcomechoix", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomepose", true);
			mTTS.Silence(3000, true);
			mTTS.SayKey ("welcomeregarder", true);

			yield return new WaitWhile(() => !mTTS.HasFinishedTalking);
			mAnimatorManager.ActivateCmd ((byte)(Command.Idle));
		}

		private IEnumerator MoveHeadYesHinge (float lYesAngle, float lYesSpeed)
		{
			//if(!mHeadMoving) StartCoroutine (MoveHeadYesHinge (0, lYesSpeed));
			yield return new WaitWhile(() => !mHeadMoving);
			BYOS.Instance.Primitive.Motors.YesHinge.SetPosition (lYesAngle, lYesSpeed);
			yield return new WaitWhile(() => Math.Abs(BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition - lYesAngle) > 0.1 && mHeadMoving);
			if (mHeadMoving && mChangeDirection) {
				mChangeDirection = false;
				StartCoroutine (MoveHeadYesHinge (0, lYesSpeed));
			}
		}

		private IEnumerator MoveHeadNoHinge (float lNoAngle, float lNoSpeed)
		{
			yield return new WaitWhile(() => !mTTS.HasFinishedTalking);
			//Comment this line if you need a linear movement of the head 
			StartCoroutine(MoveHeadYesHinge(-5,5));
			mHeadMoving = true;
			BYOS.Instance.Primitive.Motors.NoHinge.SetPosition (lNoAngle, lNoSpeed);
			yield return new WaitWhile(() => Math.Abs(BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition - lNoAngle) > 0.1);
			mHeadMoving = false;
			mChangeDirection = true;
		}

		private IEnumerator MoveHeadWhenSpeaking ()
		{
			if(mTTS.HasFinishedTalking) BYOS.Instance.Primitive.Motors.YesHinge.SetPosition (0, 15f);
			yield return new WaitWhile(() => mTTS.HasFinishedTalking);
			float lYesAngle = BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition;
			lYesAngle = lYesAngle - (float) (3 * Math.Sin(mYesAngle));
			//Debug.Log ("Current Yes: " + BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition + ", Dist Yes: " + lYesAngle);
			BYOS.Instance.Primitive.Motors.YesHinge.SetPosition (lYesAngle, 15f);
			mYesAngle += UnityEngine.Random.Range(1.0F, 9.0F);
			yield return new WaitWhile(() => Math.Abs(BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition - lYesAngle) > 0.1 && !mTTS.HasFinishedTalking);
			StartCoroutine (MoveHeadWhenSpeaking ());
		}

	}
}