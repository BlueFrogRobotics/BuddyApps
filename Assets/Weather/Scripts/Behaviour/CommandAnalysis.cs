using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Weather
{
	public class CommandAnalysis : AStateMachineBehaviour
	{

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			Debug.Log("ENTER Command analysis");
			WeatherData.Instance.Location = "";
			WeatherData.Instance.Date = 0;
			WeatherData.Instance.Forecast = "";
			WeatherData.Instance.When = false;

			if (WeatherData.Instance.VocalRequest != "")
				StringAnalysis(WeatherData.Instance.VocalRequest);

			Trigger("Request");
		}


		private void StringAnalysis(string vocalRequest)
		{
			// Analyse string to find parameters (place, date ...)
			if(ContainsOneOf(vocalRequest, Dictionary.GetPhoneticStrings("when")))
			ExtractDate(vocalRequest);
			ExtractLocation(vocalRequest);
			ExtractForecast(vocalRequest);
		}



		private void ExtractDate(string iSpeech)
		{
			if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("today"))) {
				WeatherData.Instance.Date = 0;
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("dayaftertomorrow"))) {
				WeatherData.Instance.Date = 2;
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("tomorrow"))) {
				WeatherData.Instance.Date = 1;
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("intime")) && ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("day"))) {
				int nbDay = 0;
				string[] words = iSpeech.Split(' ');
				for (int iw = 0; iw < words.Length; ++iw) {
					if (words[iw].ToLower() == Dictionary.GetString("intime") && iw + 2 < words.Length) {
						if (Int32.TryParse(words[iw + 1], out nbDay) && ContainsOneOf(words[iw + 2], Dictionary.GetPhoneticStrings("day"))) {
							Debug.Log("contains in days: " + words[iw + 2]);

							WeatherData.Instance.Date = nbDay;
							break;
						}
					}
				}
			}
		}


		private void ExtractForecast(string iSpeech)
		{
			iSpeech = iSpeech.ToLower();
			if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("snow"))) {
				WeatherData.Instance.Forecast = "snow";
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("rain"))) {
				WeatherData.Instance.Forecast = "rain";
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("fog"))) {
				WeatherData.Instance.Forecast = "fog";
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("sun"))) {
				WeatherData.Instance.Forecast = "sun";
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("wind"))) {
				WeatherData.Instance.Forecast = "wind";
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("thunder"))) {
				WeatherData.Instance.Forecast = "thunder";
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("cloud"))) {
				WeatherData.Instance.Forecast = "cloud";
			}
		}

		private void ExtractLocation(string iSpeech)
		{
			string[] words = iSpeech.Split(' ');
			for (int iw = 0; iw < words.Length; ++iw) {
				if (words[iw].ToLower() == Dictionary.GetString("inlocation")) {
					if (iw + 2 < words.Length) {
						if (ContainsOneOf(words[iw + 2], Dictionary.GetPhoneticStrings("day"))) {
							continue;
						} else {
							WeatherData.Instance.Location = words[iw + 1];
							break;
						}
					} else if (iw + 1 < words.Length) {
						WeatherData.Instance.Location = words[iw + 1];
						break;
					}
				}
			}
		}



		//private void ExtractHour(string iSpeech)
		//{
		//	Debug.Log("hour pre: " + link.departureDate);
		//	List<int> hour = ContainsHour(iSpeech);
		//	if (hour.Count == 1) {
		//		link.time = "à partir de " + hour[0].ToString() + " heure";
		//		link.departureDate = link.departureDate.AddHours(hour[0]);
		//		link.departureTimeSet = true;
		//	}

		//	if (iSpeech.Contains("matin")) {
		//		if (string.IsNullOrEmpty(link.time)) {
		//			link.departureDate = link.departureDate.AddHours(4);
		//			link.time = "matin";
		//		} else {
		//			link.time += " du matin";
		//		}
		//		link.departureTimeSet = true;
		//	} else if (iSpeech.Contains("avant midi") || iSpeech.Contains("avant-midi")) {
		//		link.departureDate = link.departureDate.AddHours(4);
		//		if (string.IsNullOrEmpty(link.time)) {
		//			link.time = "avant midi";
		//		}
		//		link.departureTimeSet = true;
		//	} else if (iSpeech.Contains("après-midi") || iSpeech.Contains("midi")) {
		//		link.departureDate = link.departureDate.AddHours(12);
		//		if (string.IsNullOrEmpty(link.time)) {
		//			link.time = "à partir de midi";
		//		} else if (link.departureDate.Hour < 13) {
		//			link.departureDate = link.departureDate.AddHours(12);
		//			link.time += " de l'après midi";
		//		}
		//		link.departureTimeSet = true;
		//	} else if (iSpeech.Contains("soir")) {
		//		if (string.IsNullOrEmpty(link.time)) {
		//			link.departureDate = link.departureDate.AddHours(17);
		//			link.time = "soir";
		//		} else if (link.departureDate.Hour < 13) {
		//			link.departureDate = link.departureDate.AddHours(12);
		//			link.time += " du soir";
		//		}
		//		link.departureTimeSet = true;
		//	}
		//	Debug.Log("hour post: " + link.departureDate);
		//}


		private bool ContainsOneOf(string iSpeech, string[] iListSpeech)
		{
			for (int i = 0; i < iListSpeech.Length; ++i) {
				string[] words = iListSpeech[i].Split(' ');
				if (words.Length < 2) {
					words = iSpeech.Split(' ');
					foreach (string word in words) {
						if (word == iListSpeech[i].ToLower()) {
							return true;
						}
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
					return true;
			}
			return false;
		}
	}
}