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
		[SerializeField]
		private Animator mMainAnimator;
		private TextToSpeech mTTS;
		private bool mHeadMoving = true;
		private bool mChangeDirection = true;
		void Awake ()
		{
			mMainAnimator = GameObject.Find ("AIBehaviour").GetComponent<Animator> ();
			mTTS = BYOS.Instance.Interaction.TextToSpeech;

		}

		public void InitState ()
		{
			StartCoroutine(MoveHeadNoHinge(15,7.5f));
			StartCoroutine(Speaking());
		}
			

		private IEnumerator Speaking ()
		{
			yield return new WaitWhile(() => mHeadMoving);
			mTTS.SayKey ("welcomebonjour",true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomebienvenue", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomeinvitation", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomemaison", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomefutur", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomeajd", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomemtnt", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomeoffre", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomeassez", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomechoix", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomepose", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomeregarder", true);
			StartCoroutine(MoveHeadNoHinge(0,7.5f));
		}

		private IEnumerator MoveHeadYesHinge (float lYesAngle, float lYesSpeed)
		{
			yield return new WaitWhile(() => !mHeadMoving);
			BYOS.Instance.Primitive.Motors.YesHinge.SetPosition (lYesAngle, lYesSpeed);
			yield return new WaitWhile(() => Math.Abs(BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition - lYesAngle) > 0.02 && mHeadMoving);
			if (mHeadMoving && mChangeDirection) {
				mChangeDirection = false;
				StartCoroutine (MoveHeadYesHinge (0, lYesSpeed));
			}
		}
	
		private IEnumerator MoveHeadNoHinge (float lNoAngle, float lNoSpeed)
		{
			yield return new WaitWhile(() => !mTTS.HasFinishedTalking);
			//StartCoroutine(MoveHeadYesHinge(-3,3));
			mHeadMoving = true;
			BYOS.Instance.Primitive.Motors.NoHinge.SetPosition (lNoAngle, lNoSpeed);
			yield return new WaitWhile(() => Math.Abs(BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition - lNoAngle) > 0.1);
			mHeadMoving = false;
			mChangeDirection = true;
		}



	}
}