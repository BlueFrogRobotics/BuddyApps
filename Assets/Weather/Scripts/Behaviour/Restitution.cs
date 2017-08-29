using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Weather
{
	public class Restitution : AStateMachineBehaviour
	{

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// Tell the weather
			string lAnswer = "";
			string lDayString = "";
			if (WeatherData.Instance.Date == 0)
				lDayString = Dictionary.GetString("today");
			else if (WeatherData.Instance.Date == 1)
				lDayString = Dictionary.GetString("tomorrow");
			else if (WeatherData.Instance.Date == 2)
				lDayString = Dictionary.GetString("dayaftertomorrow");
			else
				lDayString = Dictionary.GetString("intime") + " " + WeatherData.Instance.Date + " " + Dictionary.GetString("days");

			//No info
			//if (WeatherData.Instance.VocalRequest == "") {
			//	lAnswer = Dictionary.GetString("today") + " " + Dictionary.GetRandomString("temperaturewillbe") + mWeatherInfo.Temperature
			//		+ " " + Dictionary.GetRandomString("degreesanditisa") + " " + mWeatherInfo.Type.ToString() + " " + Dictionary.GetString("day");

			//Forecast info
			//} else 
			if (WeatherData.Instance.Forecast != "") {
				string lNoAnswer = lAnswer = Dictionary.GetRandomString("no") + " " + Dictionary.GetRandomString("itwillbe") + " "
					+ mWeatherInfo.Type.ToString() + " " + lDayString;
				string lYesAnswer = lAnswer = Dictionary.GetRandomString("yes") + " " + Dictionary.GetRandomString("itwillbe") + " "
					+ mWeatherInfo.Type.ToString() + " " + lDayString;

				if (WeatherData.Instance.Forecast == "snow" || WeatherData.Instance.Forecast == "thunder" || WeatherData.Instance.Forecast == "wind" || WeatherData.Instance.Forecast == "fog")
					if (mWeatherInfo.Type == Buddy.WeatherType.CLOUDY || mWeatherInfo.Type == Buddy.WeatherType.SUNNY)
						lAnswer = lNoAnswer;
					else
						lAnswer = Dictionary.GetRandomString("no") + " " + Dictionary.GetRandomString("but") + " " + Dictionary.GetRandomString("itwillbe") + " " + mWeatherInfo.Type.ToString() + " " + lDayString;

				else if (WeatherData.Instance.Forecast == "cloud")
					if (mWeatherInfo.Type == Buddy.WeatherType.CLOUDY)
						lAnswer = lYesAnswer;
					else
						lAnswer = lNoAnswer;

				else if (WeatherData.Instance.Forecast == "sun")
					if (mWeatherInfo.Type == Buddy.WeatherType.SUNNY)
						lAnswer = lYesAnswer;
					else
						lAnswer = lNoAnswer;

				else if (WeatherData.Instance.Forecast == "rain")
					if (mWeatherInfo.Type == Buddy.WeatherType.RAIN)
						lAnswer = lYesAnswer;
					else
						lAnswer = lNoAnswer;

			} else {
					Interaction.TextToSpeech.Say(lDayString + " " + Dictionary.GetRandomString("temperaturewillbe") + mWeatherInfo.Temperature + " " + Dictionary.GetRandomString("degreesanditisa") + " " + mWeatherInfo.Type.ToString());
			}


			Interaction.TextToSpeech.Say(lAnswer + " " + Dictionary.GetRandomString("inlocation") + " " + WeatherData.Instance.Location);
		}





		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
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