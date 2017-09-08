using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;


namespace BuddyApp.Weather
{
	public class CommandAnalysis : AStateMachineBehaviour
	{

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			Debug.Log("ENTER Command analysis");
			WeatherData.Instance.Location = "";
			WeatherData.Instance.Date = -1;
			WeatherData.Instance.Forecast = WeatherType.UNKNOWN;
			WeatherData.Instance.Hour = -1;
			WeatherData.Instance.When = false;

			if (WeatherData.Instance.VocalRequest != "")
				StringAnalysis(WeatherData.Instance.VocalRequest);

			Trigger("Request");
		}


		private void StringAnalysis(string vocalRequest)
		{
			// Analyse string to find parameters (place, date ...)
			if (ContainsOneOf(vocalRequest, Dictionary.GetPhoneticStrings("when")))
				ExtractDate(vocalRequest);
			ExtractHour(vocalRequest);
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
				WeatherData.Instance.Forecast = WeatherType.SNOWY;
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("rain"))) {
				WeatherData.Instance.Forecast = WeatherType.RAIN;
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("fog"))) {
				WeatherData.Instance.Forecast = WeatherType.OVERCAST;
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("sun"))) {
				WeatherData.Instance.Forecast = WeatherType.SUNNY;
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("wind"))) {
				// TODO
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("thunder"))) {
				WeatherData.Instance.Forecast = WeatherType.OVERCAST;
			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("cloud"))) {
				WeatherData.Instance.Forecast = WeatherType.CLOUDY;
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


		/// <summary>
		/// Extract the hour from the text
		/// may it be from direct hour (8 in the morning)
		/// or words like morning / evening / night
		/// </summary>
		/// <param name="iSpeech"></param>
		private void ExtractHour(string iSpeech)
		{
			Debug.Log("hour pre: " + iSpeech);
			List<int> lHour = ContainsHour(iSpeech);
			if (lHour.Count == 1) {
				WeatherData.Instance.Hour = lHour[0];
			}

			// TODO dico
			if (iSpeech.Contains("matin")) {
				if (WeatherData.Instance.Hour == -1) {
					WeatherData.Instance.Hour = 8;
				} else {
					// a.m. hour
					// mb check that it's < 12?
				}

				//} else if (iSpeech.Contains("avant midi") || iSpeech.Contains("avant-midi")) {
				//	link.departureDate = link.departureDate.AddHours(4);
				//	if (string.IsNullOrEmpty(link.time)) {
				//		link.time = "avant midi";
				//	}
				//	link.departureTimeSet = true;
			} else if (iSpeech.Contains("après-midi") || iSpeech.Contains("midi")) {
				//link.departureDate = link.departureDate.AddHours(12);
				if (WeatherData.Instance.Hour == -1) {
					if (iSpeech.Contains("après-midi"))
						WeatherData.Instance.Hour = 15;
					else
						WeatherData.Instance.Hour = 12;

				} else if (WeatherData.Instance.Hour < 13) {
					WeatherData.Instance.Hour += 12;
				}

			} else if (iSpeech.Contains("soir")) {
				if (WeatherData.Instance.Hour == -1) {
					WeatherData.Instance.Hour = 20;
				} else if (WeatherData.Instance.Hour < 13) {
					WeatherData.Instance.Hour += 12;
				}

			} else if (iSpeech.Contains("nuit")) {
				if (WeatherData.Instance.Hour == -1) {
					WeatherData.Instance.Hour = 23;
				} else if (WeatherData.Instance.Hour < 13) {
					// Do nothing?
				}
			}
			Debug.Log("hour post: " + WeatherData.Instance.Hour);
		}

		private List<int> ContainsHour(string iSpeech)
		{
			List<int> result = new List<int>();

			string[] words = iSpeech.Split(' ');
			for (int iw = 0; iw < words.Length; ++iw) {
				for (int i = 0; i < 24; ++i) {
					//Debug.Log("iMonthList actuel " + iMonthsList[i]);
					//Debug.Log("iword actuel " + word);
					if (words[iw].ToLower() == i.ToString() + "h") {
						result.Add(i);
						Debug.Log("contains hour: " + i);
					} else if ((words[iw].ToLower() == "h" || words[iw].ToLower() == "heures" || words[iw].ToLower() == "heure")) {
						if (words[iw - 1] == i.ToString()) {

							result.Add(i);
							Debug.Log("contains hour: " + i);
						}
					}
				}
			}
			return result;
		}



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