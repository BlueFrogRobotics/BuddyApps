using System;
using UnityEngine;
using Buddy;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.ExperienceCenter
{
	public class AttitudeBehaviour : MonoBehaviour
	{
		private BMLManager mBMLManager;
		private TextToSpeech mTTS;

		private System.Random mRnd;
		private const int WAIT_TIMEOUT = 5;

		public bool IsWaiting { get; set; }

		void Awake()
		{
			mBMLManager = BYOS.Instance.Interaction.BMLManager;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mRnd = new System.Random();
		}
		
		public void StartWaiting()
		{
			IsWaiting = true;
			if (ExperienceCenterData.Instance.EnableBML)
				StartCoroutine(Waiting());
		}

		public void MoveHeadWhileSpeaking(int angleMin, int angleMax)
		{
			//if (ExperienceCenterData.Instance.EnableBML)
				//StartCoroutine(Speaking(angleMin, angleMax));
		}


		private IEnumerator Speaking(int angleMin, int angleMax)
		{
			Dictionary<string, string> lParamDictionary = new Dictionary<string, string>();
			System.Random rnd = new System.Random();
			int initNoAngle = (int) BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition;
			int noHinge;
			String gazeShift;

			yield return new WaitUntil(() => mTTS.IsSpeaking);
			do
			{
				lParamDictionary.Clear();

				noHinge = rnd.Next(angleMin, angleMax);
				if (Math.Abs(noHinge - angleMin) <= (int)0.1 * (angleMax - angleMin))
					gazeShift = "LEFT";
				else if (Math.Abs(angleMax - noHinge) <= (int)0.1 * (angleMax - angleMin))
					gazeShift = "RIGHT";
				else
					gazeShift = "CENTER";

				lParamDictionary.Add("GAZE_DIR", gazeShift);
				lParamDictionary.Add("NO_ANGLE", (noHinge + initNoAngle).ToString());
				lParamDictionary.Add("YES_ANGLE", rnd.Next(angleMin, angleMax).ToString());
				mBMLManager.LaunchByName("Speaking01", lParamDictionary);

				yield return new WaitUntil(() => mBMLManager.DonePlaying);
				yield return new WaitForSeconds(1f);
			} while (!mTTS.HasFinishedTalking);
		}

		private IEnumerator Waiting()
		{
			do
			{
				// Wait for a random few seconds before starting
				yield return new WaitForSeconds((float)mRnd.NextDouble() * WAIT_TIMEOUT);
				// Launch BML and wait until it ends
				bool status = mBMLManager.LaunchRandom("Idle");
				Debug.LogWarningFormat("Launch a random BML : {0}",status);
				if(status)
					yield return new WaitUntil(() => mBMLManager.DonePlaying);
				else
				{
					Debug.LogError("Could not launch idle BML");
					yield return new WaitForSeconds(0.5f);
				}
			} while (IsWaiting);
		}
	}
}
