using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using System;


namespace BuddyApp.Weather
{
	public class APIRequest : AStateMachineBehaviour
	{
		private int mDate;
		//Max numberWeatherInfos égal à 64
		private int mNumberWeatherInfos = 60;
		private bool mAnswerReceived;
		private bool mQuit;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mWeatherB = GetComponent<WeatherBehaviour>();
			Debug.Log("ENTER API REQUEST");
			mAnswerReceived = false;
			mQuit = false;
			// Collect data and request the API accordingly

			if (Application.internetReachability == NetworkReachability.NotReachable) {
				Debug.Log("Error. Check internet connection!");
			}


			Debug.Log("Pre web service " + mWeatherB.mLocation);
			BYOS.Instance.WebService.Weather.At(mWeatherB.mLocation, WeatherProcessing, mNumberWeatherInfos);

			Debug.Log("Post web service ");


		}



		//OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (mAnswerReceived) {
				Trigger("Restitution");
			} else if (mQuit && Interaction.SpeechToText.HasFinished)
				QuitApp();
		}






		private void WeatherProcessing(WeatherInfo[] iWeather, WeatherError iError)
		{
			Debug.Log("WeatherProcessing");
			if (iError != WeatherError.NONE) {
				if (iError == WeatherError.UNKNOWN_LOCATION)
					Interaction.TextToSpeech.SayKey("locationissue");
				else if (iError == WeatherError.GEOLOCALIZATION_FAILED)
					Interaction.TextToSpeech.SayKey("geolocfailed");
				else if (iError == WeatherError.GEOLOCALIZATION_DISABLED)
					Interaction.TextToSpeech.SayKey("geolocdisable");




				mQuit = true;
				return;
			}
			// If error are handle correctly, this shouldn't happen!
			else if (iWeather == null || iWeather.Length == 0) {
				//Unknown location
				Interaction.TextToSpeech.SayKey("locationissue");
				mQuit = true;
				return;
				//Say la location n'est pas reconnue / n'existe pas
			} else {
				Debug.Log("LOCATION : " + iWeather[0].Location);
				if (mWeatherB.mDate < 7) {
					//pas grave si ça dépasse 64 parce que ça ne sera un écart que de quelques heures pour la météo dans 6 jours pour le max
					mDate = 8 * mWeatherB.mDate + 2;

				} else if (mWeatherB.mDate > 7) {
					Interaction.TextToSpeech.SayKey("oneweekmax");
					mQuit = true;
					return;
				}
				GetComponent<WeatherBehaviour>().mWeatherInfo = iWeather[mDate];
				Debug.Log("API req + temp" + GetComponent<WeatherBehaviour>().mWeatherInfo.Hour);
			}
			mAnswerReceived = true;
		}
	}
}