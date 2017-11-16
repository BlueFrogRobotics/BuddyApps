//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Buddy;

namespace BuddyApp.Weather
{
}

//namespace BuddyApp.Weather
//{

//	public class CommandAnalysis : AStateMachineBehaviour
//	{

//		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
//		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//		{
//			mWeatherB = GetComponent<WeatherBehaviour>();
//			//Debug.Log("ENTER Command analysis: " + DateTime.Now.Hour);
//			mWeatherB.mLocation = "";
//			mWeatherB.mDate = -1;
//			mWeatherB.mForecast = WeatherType.UNKNOWN;
//			mWeatherB.mHour = -1;
//			mWeatherB.mWhen = false;

//			if (WeatherData.Instance.VocalRequest != "")
//				StringAnalysis(WeatherData.Instance.VocalRequest.ToLower());

//			Trigger("Request");
//		}


//		private void StringAnalysis(string vocalRequest)
//		{
//			// Analyse string to find parameters (place, date ...)
//			if (ContainsOneOf(vocalRequest, Dictionary.GetPhoneticStrings("when")))
//				mWeatherB.mWhen = true;
//			ExtractDate(vocalRequest);
//			ExtractHour(vocalRequest);
//			ExtractLocation(vocalRequest);
//			ExtractForecast(vocalRequest);
//		}



//		private void ExtractDate(string iSpeech)
//		{
//			if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("today"))) {
//				mWeatherB.mDate = 0;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("dayaftertomorrow"))) {
//				mWeatherB.mDate = 2;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("tomorrow"))) {
//				mWeatherB.mDate = 1;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("intime")) && ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("day"))) {
//				int nbDay = 0;
//				string[] words = iSpeech.Split(' ');
//				for (int iw = 0; iw < words.Length; ++iw) {
//					if (words[iw].ToLower() == Dictionary.GetString("intime") && iw + 2 < words.Length) {
//						if (Int32.TryParse(words[iw + 1], out nbDay) && ContainsOneOf(words[iw + 2], Dictionary.GetPhoneticStrings("day"))) {
//							Debug.Log("contains in days: " + words[iw + 2]);

//							mWeatherB.mDate = nbDay;
//							break;
//						}
//					}
//				}
//			}
//		}


//		private void ExtractForecast(string iSpeech)
//		{
//			iSpeech = iSpeech.ToLower();
//			if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("snow"))) {
//				mWeatherB.mForecast = WeatherType.SNOWY;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("rain"))) {
//				mWeatherB.mForecast = WeatherType.RAIN;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("fog"))) {
//				mWeatherB.mForecast = WeatherType.OVERCAST;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("sun"))) {
//				mWeatherB.mForecast = WeatherType.SUNNY;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("wind"))) {
//				// TODO
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("thunder"))) {
//				mWeatherB.mForecast = WeatherType.OVERCAST;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("cloud"))) {
//				mWeatherB.mForecast = WeatherType.CLOUDY;
//			}
//		}

//		private void ExtractLocation(string iSpeech)
//		{
//			string[] words = iSpeech.Split(' ');
//			for (int iw = 0; iw < words.Length; ++iw) {
//				if (words[iw].ToLower() == Dictionary.GetString("inlocation")) {
//					if (iw + 2 < words.Length) {
//						if (ContainsOneOf(words[iw + 2], Dictionary.GetPhoneticStrings("day"))) {
//							// TODO add hours exception
//							continue;
//						} else {
//							mWeatherB.mLocation = words[iw + 1];
//							break;
//						}
//					} else if (iw + 1 < words.Length) {
//						mWeatherB.mLocation = words[iw + 1];
//						break;
//					}
//				}
//			}
//		}


//		/// <summary>
//		/// Extract the hour from the text
//		/// may it be from direct hour (8 in the morning)
//		/// or words like morning / evening / night
//		/// </summary>
//		/// <param name="iSpeech"></param>
//		private void ExtractHour(string iSpeech)
//		{
//			Debug.Log("hour pre: " + iSpeech);
//			List<int> lHour = ContainsHour(iSpeech);
//			if (lHour.Count == 1) {
//				mWeatherB.mHour = lHour[0];
//			}

//			// TODO dico
//			if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("morning"))) {
//				if (mWeatherB.mHour == -1) {
//					mWeatherB.mHour = 8;
//				} else {
//					// a.m. hour
//					// mb check that it's < 12?
//				}

//				//} else if (iSpeech.Contains("avant midi") || iSpeech.Contains("avant-midi")) {
//				//	link.departureDate = link.departureDate.AddHours(4);
//				//	if (string.IsNullOrEmpty(link.time)) {
//				//		link.time = "avant midi";
//				//	}
//				//	link.departureTimeSet = true;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("noon")) || ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("pm"))) {
//				//link.departureDate = link.departureDate.AddHours(12);
//				if (mWeatherB.mHour == -1) {
//					if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("pm")))
//						mWeatherB.mHour = 14;
//					else
//						mWeatherB.mHour = 11;

//				} else if (mWeatherB.mHour < 13) {
//					mWeatherB.mHour += 12;
//				}
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("midnight"))) {
//				if (mWeatherB.mHour == -1)
//						mWeatherB.mHour = 24;
//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("evening"))) {
//				if (mWeatherB.mHour == -1) {
//					mWeatherB.mHour = 20;
//				} else if (mWeatherB.mHour < 13) {
//					mWeatherB.mHour += 12;
//				}

//			} else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("night"))) {
//				if (mWeatherB.mHour == -1) {
//					mWeatherB.mHour = 23;
//				} else if (mWeatherB.mHour < 13) {
//					// Do nothing?
//				}
//			}
//			Debug.Log("hour post: " + mWeatherB.mHour);
//		}

//		private List<int> ContainsHour(string iSpeech)
//		{
//			List<int> result = new List<int>();

//			string[] words = iSpeech.Split(' ');
//			for (int iw = 0; iw < words.Length; ++iw) {
//				for (int i = 0; i < 24; ++i) {
//					//Debug.Log("iMonthList actuel " + iMonthsList[i]);
//					//Debug.Log("iword actuel " + word);
//					if (words[iw].ToLower() == i.ToString() + "h") {
//						result.Add(i);
//						Debug.Log("contains hour: " + i);
//					} else if (ContainsOneOf(words[iw].ToLower(), Dictionary.GetPhoneticStrings("hour"))) {
//						if (words[iw - 1] == i.ToString()) {

//							result.Add(i);
//							Debug.Log("contains hour: " + i);
//						}
//					}
//				}
//			}
//			return result;
//		}



//		private bool ContainsOneOf(string iSpeech, string[] iListSpeech)
//		{
//			iSpeech = iSpeech.ToLower();
//			for (int i = 0; i < iListSpeech.Length; ++i) {
//				string[] words = iListSpeech[i].Split(' ');
//				if (words.Length < 2) {
//					words = iSpeech.Split(' ');
//					foreach (string word in words) {
//						if (word == iListSpeech[i].ToLower()) {
//							return true;
//						}
//					}
//				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
//					return true;
//			}
//			return false;
//		}
//	}
//}