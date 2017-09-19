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
		private int mNumberWeatherInfos;
		//public int NumberWeatherInfos { get { return mNumberWeatherInfos; } }
		private bool mAnswerReceived;
		private bool mQuit;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{


			mWeatherB = GetComponent<WeatherBehaviour>();

			Debug.Log("enter api weather type  " + mWeatherB.mForecast + " when " + mWeatherB.mWhen);

			mNumberWeatherInfos = 60;
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
			mWeatherB.mWeatherInfos = iWeather;
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
				Interaction.TextToSpeech.SayKey("locationissue");
				mQuit = true;
				return;
			} else {

				//a changer la facon d'avoir le forecast parce que pour le moment cest dans les persistentData mais au final ça ne doit pas etre dedans.
				
				GetWeatherInfos(iWeather, mWeatherB.mForecast);
			}
			mAnswerReceived = true;
		}

		private void GetWeatherInfos(WeatherInfo[] iWeather, WeatherType iWeatherType)
		{
			Debug.Log("GetWeatherInfos");
			mWeatherB.mIndice = -1;
			mWeatherB.mRequestError = WeatherBehaviour.WeatherRequestError.UNKNOWN;
			bool lFound = false;
			if (iWeatherType != WeatherType.UNKNOWN) {
				Debug.Log("GetWeatherInfos api kikoo date " + mWeatherB.mDate);
				if (mWeatherB.mWhen && mWeatherB.mDate == -1) {
					for (int i = 0; i < mNumberWeatherInfos; ++i) {
						Debug.Log("GetWeatherInfos api indice " + i + " weather type speech " + iWeatherType + " weather type weatherInfo  " + iWeather[i].Type);
						if (iWeather[i].Type == iWeatherType) {

							Debug.Log("GetWeatherInfos api indice " + i + " weather type speech " + iWeatherType + " weather type weatherInfo  " + iWeather[i].Type);
							mWeatherB.mIndice = i;
							lFound = true;
							break;
						}

					}
					if (!lFound) {
						mWeatherB.mRequestError = WeatherBehaviour.WeatherRequestError.TOO_FAR;
					}

				} else {

					int lHour = 0;
					int lDay = 0;

					// Set the date
					if (mWeatherB.mDate < 0)
						lDay = DateTime.Now.Day;
					else
						lDay = mWeatherB.mDate;

					// Set the hour
					if (mWeatherB.mHour == -1)
						lHour = DateTime.Now.Hour;
					else
						lHour = mWeatherB.mHour;


					for (int i = 0; i < mNumberWeatherInfos; ++i) {
						if (iWeather[i].Day == lDay && iWeather[i].Type == iWeatherType && iWeather[i].Hour > lHour) {
							mWeatherB.mIndice = i;
							lFound = true;
							break;
						}
					}
					if (!lFound) {
						for (int i = 0; i < mNumberWeatherInfos; ++i) {
							if (iWeather[i].Hour > lHour) {
								mWeatherB.mIndice = i;
								mWeatherB.mRequestError = WeatherBehaviour.WeatherRequestError.NONE;
								break;
							}
						}
					}
				}

			} else if (iWeatherType == WeatherType.UNKNOWN) {
				//Quel temps va til faire date/heure?
				if (mWeatherB.mDate < 1 ) {
					int lHour = DateTime.Now.Hour;
					if (mWeatherB.mHour == -1) {
						//quel temps va t'il faire aujourdhui?
						for (int i = 0; i<mNumberWeatherInfos; ++i) {
							if (lHour<iWeather[i].Hour) {
								mWeatherB.mIndice = i;
								lFound = true;
								break;
							}
}

					} else if (mWeatherB.mHour >= 0) {
						for (int i = 0; i<mNumberWeatherInfos; ++i) {
							if (mWeatherB.mHour<iWeather[i].Hour) {
								mWeatherB.mIndice = i;
								lFound = true;
								break;
							}
						}
					}


					Debug.Log("GetWeatherInfos today or no date indice: " + mWeatherB.mIndice);

				} else if (mWeatherB.mDate > 0) {
					int lDay = DateTime.Now.Day;
					if (mWeatherB.mHour == -1) {
						for (int i = 0; i<mNumberWeatherInfos; ++i) {
							if (iWeather[i].Hour > 11 && iWeather[i].Day - lDay == mWeatherB.mDate) {
								mWeatherB.mIndice = i;
								lFound = true;
								break;
							}
						}

					} else {
						for (int i = 0; i<mNumberWeatherInfos; ++i) {
							if ( (mWeatherB.mHour<iWeather[i].Hour &&
                                iWeather[i].Day - lDay == mWeatherB.mDate) || ( (iWeather[i].Day - lDay) -  mWeatherB.mDate > 0) ) {
								mWeatherB.mIndice = i;
								lFound = true;
								break;
							}
						}
					}
				}
			}
		}
	}
}