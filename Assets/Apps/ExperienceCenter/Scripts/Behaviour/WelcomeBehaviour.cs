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
			StartCoroutine(MoveHeadNoHinge(30,15));
			StartCoroutine(Speaking());
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
			mTTS.SayKey ("welcomechoix", true);
			mTTS.Silence(500, true);
			mTTS.SayKey ("welcomepose", true);
			mTTS.Silence(3000, true);
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
			//Comment this line if you need a linear movement of the head 
			StartCoroutine(MoveHeadYesHinge(-6,6));
			mHeadMoving = true;
			BYOS.Instance.Primitive.Motors.NoHinge.SetPosition (lNoAngle, lNoSpeed);
			yield return new WaitWhile(() => Math.Abs(BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition - lNoAngle) > 0.1);
			mHeadMoving = false;
			mChangeDirection = true;
		}



	}
}