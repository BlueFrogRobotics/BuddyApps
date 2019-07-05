using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.TestSourceLoc
{
	/* A basic monobehaviour as "AI" behaviour for your app */
	public class TestSourceLocBehaviour : MonoBehaviour
	{
		/*
         * Data of the application. Save on disc when app is quitted
         */
		private TestSourceLocData mAppData;

		[SerializeField]
		Slider mThresh;

		void Start()
		{

			/*
			* You can setup your App activity here.
			*/
			TestSourceLocActivity.Init(null);

			/*
			* Init your app data
			*/
			mAppData = TestSourceLocData.Instance;
			mThresh.value = Buddy.Sensors.Microphones.SoundLocalizationParameters.Threshold;
		}

		public void OnThreshValueChange()
		{
			Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(Buddy.Sensors.Microphones.SoundLocalizationParameters.Resolution, (int) mThresh.value);
			Debug.LogWarning("SourceLoc threshold set to " + mThresh.value);
		}


		private void Update()
		{
			if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION)
			{
				Debug.LogWarning("New sound loc + time + thresh" + Buddy.Sensors.Microphones.SoundLocalization + "  " + DateTime.Now.ToString() + " " + Buddy.Sensors.Microphones.SoundLocalizationParameters.Threshold);
			}
		}
	}

}