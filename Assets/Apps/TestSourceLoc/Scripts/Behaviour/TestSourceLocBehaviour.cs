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

        [SerializeField]
        Text mTextSlider;

        [SerializeField]
        Text mTextThresh;

        [SerializeField]
        Text mTextAngle;
        
        [SerializeField]
        Text mTextLastAngle;

        [SerializeField]
        Text mTextTime;

        void Start()
		{

			/*
			* You can setup your App activity here.
			*/
			TestSourceLocActivity.Init(null);
            
			mThresh.value = Buddy.Sensors.Microphones.SoundLocalizationParameters.Threshold;
		}

		public void OnThreshValueChange()
		{
            mTextSlider.text = mThresh.value.ToString();
			Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(Buddy.Sensors.Microphones.SoundLocalizationParameters.Resolution, (int) mThresh.value);
			Debug.LogWarning("SourceLoc threshold set to " + mThresh.value);
		}


		private void Update()
		{
            mTextThresh.text = "Threshold: " + Buddy.Sensors.Microphones.SoundLocalizationParameters.Threshold.ToString();
            mTextAngle.text = "Value: " + Buddy.Sensors.Microphones.SoundLocalization.ToString();


            if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION) {
                mTextLastAngle.text = "LastAngle: " + Buddy.Sensors.Microphones.SoundLocalization.ToString();
                mTextTime.text = DateTime.Now.ToString("mm:ss");
                Debug.LogWarning("New sound loc + time + thresh" + Buddy.Sensors.Microphones.SoundLocalization + "  " + DateTime.Now.ToString() + " " + Buddy.Sensors.Microphones.SoundLocalizationParameters.Threshold);
			}
		}
	}

}