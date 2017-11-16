using UnityEngine;
using Buddy;
using System;


namespace BuddyApp.Weather
{
	public class Restitution : AStateMachineBehaviour
	{
		private float mTimer;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mWeatherB = GetComponent<WeatherBehaviour>();
			Debug.Log("ENTER RESTITUTION");
			mTimer = 0F;
			Debug.Log("ENTER RESTITUTION: " + mWeatherB.mIndice + " Weather info size: " + mWeatherB.mWeatherInfos.Length);
			WeatherInfo lWeatherInfo = new WeatherInfo();
			if (mWeatherB.mIndice != -1)
				lWeatherInfo = mWeatherB.mWeatherInfos[mWeatherB.mIndice];

			// Tell the weather
			string lAnswer = "";
			string lDayString = "";
			if (mWeatherB.mDate < 1)
				lDayString = Dictionary.GetString("today");
			else if (mWeatherB.mDate == 1)
				lDayString = Dictionary.GetString("tomorrow");
			else if (mWeatherB.mDate == 2)
				lDayString = Dictionary.GetString("dayaftertomorrow");
			else
				lDayString = Dictionary.GetString("intime") + " " + mWeatherB.mDate + " " + Dictionary.GetString("days");

			//No info
			//if (mWeatherB.mVocalRequest == "") {
			//	lAnswer = Dictionary.GetString("today") + " " + Dictionary.GetRandomString("temperaturewillbe") + WeatherInfo.Temperature
			//		+ " " + Dictionary.GetRandomString("degreesanditisa") + " " + WeatherInfo.Type.ToString() + " " + Dictionary.GetString("day");

			//Forecast info
			//} else 

			if (mWeatherB.mForecast != WeatherType.UNKNOWN) {
				if (mWeatherB.mWhen) {

					if (mWeatherB.mIndice != -1) {


						// Give date: It will rain ...
						lAnswer = Dictionary.GetRandomString("itwillbe") + " " + Dictionary.GetRandomString((lWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " le " + lWeatherInfo.Day + " à " + lWeatherInfo.Hour + " " + Dictionary.GetRandomString("hour") + " ";
					} else {
						// Deny: There is no rain forecast for upcoming week
						lAnswer = "Il n'est pas prévu de " + Dictionary.GetRandomString(mWeatherB.mForecast.ToString().ToLower());

						if (mWeatherB.mDate == -1) {
							lAnswer += " cette semaine";
						} else {
							lAnswer += " " + lDayString;
						}

					}
				} else {


					string lNoAnswer = Dictionary.GetRandomString("no") + " " + Dictionary.GetRandomString("itwillbe") + " "
					+ Dictionary.GetRandomString((lWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + lDayString + " "  + Dictionary.GetRandomString("at") + " " + lWeatherInfo.Hour + " " + Dictionary.GetRandomString("hour");
					string lYesAnswer = Dictionary.GetRandomString("yes") + " " + Dictionary.GetRandomString("itwillbe") + " "
						+ Dictionary.GetRandomString((lWeatherInfo.Type.ToString().ToLower()).Replace("_", "")) + " " + lDayString + " " + Dictionary.GetRandomString("at") + " " + lWeatherInfo.Hour + " " + Dictionary.GetRandomString("hour");

					if (mWeatherB.mForecast != lWeatherInfo.Type)
						lAnswer = lNoAnswer;
					else
						lAnswer = lYesAnswer;





					//if (mWeatherB.mForecast == WeatherType.SNOWY)
					//	if (lWeatherInfo.Type == Buddy.WeatherType.CLOUDY || lWeatherInfo.Type == Buddy.WeatherType.SUNNY)
					//		lAnswer = lNoAnswer;
					//	else
					//		lAnswer = Dictionary.GetRandomString("no") + " " + Dictionary.GetRandomString("but") + " " + Dictionary.GetRandomString("itwillbe") + " " + lWeatherInfo.Type.ToString() + " " + lDayString;

					//else if (mWeatherB.mForecast == WeatherType.CLOUDY)
					//	if (lWeatherInfo.Type == Buddy.WeatherType.CLOUDY)
					//		lAnswer = lYesAnswer;
					//	else
					//		lAnswer = lNoAnswer;

					//else if (mWeatherB.mForecast == WeatherType.SUNNY)
					//	if (lWeatherInfo.Type == Buddy.WeatherType.SUNNY)
					//		lAnswer = lYesAnswer;
					//	else
					//		lAnswer = lNoAnswer;

					//else if (mWeatherB.mForecast == WeatherType.RAIN)
					//	if (lWeatherInfo.Type == Buddy.WeatherType.RAIN)
					//		lAnswer = lYesAnswer;
					//	else
					//		lAnswer = lNoAnswer;
				}
			} else {
				lAnswer = lDayString + " " + Dictionary.GetRandomString("at") + " " + lWeatherInfo.Hour + " " +  Dictionary.GetRandomString("hour") + " " + Dictionary.GetRandomString("temperaturewillbe") + " " + lWeatherInfo.MinTemperature + " "
					+ Dictionary.GetRandomString("degreesanditisa") + " " + Dictionary.GetRandomString( (lWeatherInfo.Type.ToString().ToLower()).Replace("_", "") );

			}
			if (mWeatherB.mLocation != "")
				Interaction.TextToSpeech.Say(lAnswer + " " + Dictionary.GetRandomString("inlocation") + " " + mWeatherB.mLocation);
		}

		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mTimer += Time.deltaTime;
			if (Interaction.TextToSpeech.HasFinishedTalking && mTimer > 3F) {
				Debug.Log("Restart test");
				mWeatherB.mDate = 0;
				mWeatherB.mForecast = WeatherType.UNKNOWN;
				mWeatherB.mLocation = "";
				WeatherData.Instance.VocalRequest = "";
				Trigger("Restart");
			}
		}

	}
}