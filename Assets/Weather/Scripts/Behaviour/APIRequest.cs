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
		private bool mAnswerReceived;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mAnswerReceived = false;
			// Collect data and request the API accordingly

			Debug.Log("Pre web service " + WeatherData.Instance.Location);
			BYOS.Instance.WebService.Weather.At(WeatherData.Instance.Location, WeatherProcessing, mNumberWeatherInfos);

			Debug.Log("Post web service ");


        }



		//OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (mAnswerReceived) {
				Trigger("Restitution");
			}
		}






		private void WeatherProcessing(WeatherInfo[] iWeather)
		{
			Debug.Log("WeatherProcessing");
			if (iWeather == null || iWeather.Length == 0)
            {
                Debug.Log("la location n'est pas reconnue / n'existe pas");
                return;
                //Say la location n'est pas reconnue / n'existe pas
            }
            else
            {
                Debug.Log("LOCATION : " + iWeather[0].Location);
                if (WeatherData.Instance.Date == 0)
                    mDate = 0;
                else if (WeatherData.Instance.Date == 1)
                    mDate += 8;
                else if (WeatherData.Instance.Date == 2)
                    mDate += 16;
                else if (WeatherData.Instance.Date > 2 && WeatherData.Instance.Date < 7)
                {
                    //pas grave si ça dépasse 64 parce que ça ne sera un écart que de quelques heures pour la météo dans 6 jours pour le max
                    mDate = 8 * WeatherData.Instance.Date;
                    
                }
                else if (WeatherData.Instance.Date < 7)
                {
                    Debug.Log("Pas possible d'estimer la météo qu'il fera dans plus d'une semaine");
                    //Say Pas possible destimer la météo qu'il fera dans plus d'une semaine
                }
                WeatherInfo = iWeather[mDate];
			}
			mAnswerReceived = true;
		}
    }
}