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
        private WeatherInfo[] mWeatherInfoResult;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{


			mWeatherB = GetComponent<WeatherBehaviour>();

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
                
                WeatherType mWeatherType = mWeatherB.mForecast;
                GetWeatherInfos(iWeather, mWeatherType);
			}
			mAnswerReceived = true;
		}

        private void GetWeatherInfos(WeatherInfo[] iWeather, WeatherType iWeatherType)
        {
            GetComponent<WeatherBehaviour>().mRequestError = WeatherBehaviour.WeatherRequestError.UNKNOWN;
            bool lFound = false;
            if (iWeatherType != WeatherType.UNKNOWN)
            {
                if (mWeatherB.mWhen && mWeatherB.mDate == -1)
                {
                    for (int i = 0; i < mNumberWeatherInfos; ++i)
                    {
                        if (iWeather[i].Type == iWeatherType)
                        {
                            GetComponent<WeatherBehaviour>().mIndice = i;
                            lFound = true;
                        }
                            
                    }
                    if(!lFound)
                    {
                        GetComponent<WeatherBehaviour>().mIndice = -1;
                        GetComponent<WeatherBehaviour>().mRequestError = WeatherBehaviour.WeatherRequestError.TOO_FAR;
                    }

                }
                //Date = 0 -> aujourdhui
                if (mWeatherB.mDate == 0)
                {
                    //pas d'heure -> -1
                    //probleme s'il pose cette question dans l'après midi par exemple
                    //Solution : récupérer l'heure et voir si entre cette heure et 23h il pleut
                    //Réglé
                    if (mWeatherB.mHour == -1)
                    {
                        int lHour = DateTime.Now.Hour;
                        int lDay = DateTime.Now.Day;

                        for (int i = 0; i < mNumberWeatherInfos; ++i)
                        {
                            if (iWeather[i].Day == lDay && iWeather[i].Type == iWeatherType && iWeather[i].Hour > lHour)
                            {
                                GetComponent<WeatherBehaviour>().mIndice = i;
                                lFound = true;
                                break;
                            }
                        }
                        if (!lFound)
                        {
                            for (int i = 0; i < mNumberWeatherInfos; ++i)
                            {
                                if (iWeather[i].Hour > lHour)
                                {
                                    GetComponent<WeatherBehaviour>().mIndice = i;
                                    GetComponent<WeatherBehaviour>().mRequestError = WeatherBehaviour.WeatherRequestError.NONE;
                                    break;
                                }
                            }
                        }
                            
                    }
                    else if (mWeatherB.mHour >= 0)
                    {
                        int lHour = DateTime.Now.Hour;
                        int lDay = DateTime.Now.Day;
                        for (int i = 0; i < mNumberWeatherInfos; ++i)
                        {
                            if (iWeather[i].Hour > lHour && iWeather[i].Day == lDay && iWeather[i].Type == iWeatherType)
                            {
                                if (mWeatherB.mHour > iWeather[i].Hour)
                                {
                                    GetComponent<WeatherBehaviour>().mIndice = i;
                                    lFound = true;
                                    break;
                                }
                                    
                            }
                            else if (iWeather[i].Day != lDay)
                                break;
                        }
                        if (!lFound)
                        {
                            for (int i = 0; i < mNumberWeatherInfos; ++i)
                            {
                                if(iWeather[i].Hour > mWeatherB.mHour)
                                {
                                    GetComponent<WeatherBehaviour>().mIndice = i;
                                    GetComponent<WeatherBehaviour>().mRequestError = WeatherBehaviour.WeatherRequestError.NONE;
                                    break;
                                }
                            }
                        }  
                    }
                }
                else if (mWeatherB.mDate > 0)
                {
                    if (mWeatherB.mHour == -1)
                    {
                        int lDay = DateTime.Now.Day;

                        for (int i = 0; i < mNumberWeatherInfos; ++i)
                        {
                            if (iWeather[i].Day - lDay == mWeatherB.mDate)
                            {
                                if (iWeather[i].Type == iWeatherType)
                                {
                                    GetComponent<WeatherBehaviour>().mIndice = i;
                                    lFound = true;
                                    break;
                                }
                            }
                        }
                        if (!lFound)
                        {
                            for (int i = 0; i < mNumberWeatherInfos; ++i)
                            {
                                if (iWeather[i].Hour > 11 && iWeather[i].Day - lDay == mWeatherB.mDate)
                                {
                                    GetComponent<WeatherBehaviour>().mIndice = i;
                                    GetComponent<WeatherBehaviour>().mRequestError = WeatherBehaviour.WeatherRequestError.NONE;
                                    break;
                                }
                            }
                        }

                    }
                    else if (mWeatherB.mHour >= 0)
                    {
                        //Pour le moment donne la météo pour l'horaire voulu mais à mettre sous forme d'intervalle après
                        int lDay = DateTime.Now.Day;

                        for (int i = 0; i < mNumberWeatherInfos; ++i)
                        {
                            if (iWeather[i].Day - lDay == mWeatherB.mDate)
                            {
                                if (iWeather[i].Hour < mWeatherB.mHour && iWeather[i].Type == iWeatherType && iWeather[i-1].Hour < mWeatherB.mHour)
                                {
                                    GetComponent<WeatherBehaviour>().mIndice = i;
                                    lFound = true;
                                    break;
                                }
                            }
                        }
                        if (!lFound)
                        {
                            for (int i = 0; i < mNumberWeatherInfos; ++i)
                            {
                                if (iWeather[i].Day - lDay == mWeatherB.mDate)
                                {
                                    if (iWeather[i].Hour > 11)
                                    {
                                        GetComponent<WeatherBehaviour>().mIndice = i;
                                        GetComponent<WeatherBehaviour>().mRequestError = WeatherBehaviour.WeatherRequestError.NONE;
                                    }
                                }
                            }
                            
                        }
                    }
                    
                }
            }
            else if (iWeatherType == WeatherType.UNKNOWN)
            {
                //Quel temps va til faire date/heure?
                if (mWeatherB.mDate == 0)
                {
                    int lHour = DateTime.Now.Hour;
                    if (mWeatherB.mHour == -1)
                    {
                        //quel temps va t'il faire aujourdhui?
                        for (int i = 0; i < mNumberWeatherInfos; ++i)
                        {
                            if (lHour < iWeather[i].Hour)
                            {
                                GetComponent<WeatherBehaviour>().mIndice = i;
                                lFound = true;
                                break;
                            }
                        }

                    }
                    else if (mWeatherB.mHour >= 0)
                    {
                        for (int i = 0; i < mNumberWeatherInfos; ++i)
                        {
                            if (mWeatherB.mHour < iWeather[i].Hour)
                            {
                                GetComponent<WeatherBehaviour>().mIndice = i;
                                lFound = true;
                                break;
                            }
                        }
                    }
                }
                else if(mWeatherB.mDate > 0)
                {
                    int lDay = DateTime.Now.Day;
                    if (mWeatherB.mHour == -1)
                    {
                        for (int i = 0; i < mNumberWeatherInfos; ++i)
                        {
                            if (iWeather[i].Hour > 11 && iWeather[i].Day - lDay == mWeatherB.mDate)
                            {
                                GetComponent<WeatherBehaviour>().mIndice = i;
                                lFound = true;
                                break;
                            }
                        }

                    }
                    else if (mWeatherB.mHour >= 0)
                    {
                        for (int i = 0; i < mNumberWeatherInfos; ++i)
                        {
                            if (mWeatherB.mHour < iWeather[i].Hour && 
                                iWeather[i].Day - lDay == mWeatherB.mDate &&
                                mWeatherB.mHour > iWeather[i-1].Hour)
                            {
                                GetComponent<WeatherBehaviour>().mIndice = i;
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