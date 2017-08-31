using UnityEngine;
using Buddy;


namespace BuddyApp.Weather
{
	public class Restitution : AStateMachineBehaviour
	{

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			WeatherInfo lWeatherInfo = GetComponent<WeatherBehaviour>().mWeatherInfo;
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
			//	lAnswer = Dictionary.GetString("today") + " " + Dictionary.GetRandomString("temperaturewillbe") + WeatherInfo.Temperature
			//		+ " " + Dictionary.GetRandomString("degreesanditisa") + " " + WeatherInfo.Type.ToString() + " " + Dictionary.GetString("day");

			//Forecast info
			//} else 

			if (WeatherData.Instance.Forecast != "") {
				string lNoAnswer = lAnswer = Dictionary.GetRandomString("no") + " " + Dictionary.GetRandomString("itwillbe") + " "
					+ Dictionary.GetRandomString(lWeatherInfo.Type.ToString().ToLower()) + " " + lDayString;
				string lYesAnswer = lAnswer = Dictionary.GetRandomString("yes") + " " + Dictionary.GetRandomString("itwillbe") + " "
					+ Dictionary.GetRandomString(lWeatherInfo.Type.ToString().ToLower()) + " " + lDayString;

				if (WeatherData.Instance.Forecast == "snow" || WeatherData.Instance.Forecast == "thunder" || WeatherData.Instance.Forecast == "wind" || WeatherData.Instance.Forecast == "fog")
					if (lWeatherInfo.Type == Buddy.WeatherType.CLOUDY || lWeatherInfo.Type == Buddy.WeatherType.SUNNY)
						lAnswer = lNoAnswer;
					else
						lAnswer = Dictionary.GetRandomString("no") + " " + Dictionary.GetRandomString("but") + " " + Dictionary.GetRandomString("itwillbe") + " " + lWeatherInfo.Type.ToString() + " " + lDayString;

				else if (WeatherData.Instance.Forecast == "cloud")
					if (lWeatherInfo.Type == Buddy.WeatherType.CLOUDY)
						lAnswer = lYesAnswer;
					else
						lAnswer = lNoAnswer;

				else if (WeatherData.Instance.Forecast == "sun")
					if (lWeatherInfo.Type == Buddy.WeatherType.SUNNY)
						lAnswer = lYesAnswer;
					else
						lAnswer = lNoAnswer;

				else if (WeatherData.Instance.Forecast == "rain")
					if (lWeatherInfo.Type == Buddy.WeatherType.RAIN)
						lAnswer = lYesAnswer;
					else
						lAnswer = lNoAnswer;

			} else {
				lAnswer = lDayString + " " + Dictionary.GetRandomString("temperaturewillbe") + " " + lWeatherInfo.Temperature + " " + Dictionary.GetRandomString("degreesanditisa") + " " + Dictionary.GetRandomString(lWeatherInfo.Type.ToString().ToLower());

			}
			Interaction.TextToSpeech.Say(lAnswer + " " + Dictionary.GetRandomString("inlocation") + " " + WeatherData.Instance.Location);
		}

		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (Interaction.TextToSpeech.HasFinishedTalking)
				Trigger("Restart");
		}

	}
}