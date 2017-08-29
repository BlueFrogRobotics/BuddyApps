using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using System;


namespace BuddyApp.Weather
{
	public class APIRequest : AStateMachineBehaviour
	{
        private WebService lWebServiceWeather;
        private Action<WeatherInfo[]> lWeatherInfos;
        private int mDate;
        //Max numberWeatherInfos égal à 64
        private int mNumberWeatherInfos;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

            // Collect data and request the API accordingly
            lWebServiceWeather = new WebService();
            lWeatherInfos = null;
            
            lWebServiceWeather.Weather.At(WeatherData.Instance.Location, lWeatherInfos, mNumberWeatherInfos);

            lWeatherInfos = WeatherProcessing;

        }

        private void WeatherProcessing(WeatherInfo[] iWeather)
        {
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
        }
        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
           
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{

        //}

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}