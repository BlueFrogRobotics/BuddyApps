using Buddy;
using Buddy.Command;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace BuddyApp.WolframAlpha
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class WolframAlphaBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private WolframAlphaData mAppData;
		private bool pendingRequest;

		void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			WolframAlphaActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = WolframAlphaData.Instance;
			pendingRequest = false;
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
			BYOS.Instance.Interaction.VocalManager.OnEndReco = OnSpeechRecognition;
		}
		

		private void Update()
		{
			if(BYOS.Instance.Interaction.TextToSpeech.HasFinishedTalking && BYOS.Instance.Interaction.SpeechToText.HasFinished && !pendingRequest) {
				pendingRequest = true;
				BYOS.Instance.Interaction.VocalManager.StartInstantReco();
			}
		}


		// speech reco callback
		private void OnSpeechRecognition(string iVoiceInput)
		{

			Debug.Log("OnSpeechReco");
			Debug.Log("[Knowledge] : " + iVoiceInput);

			// set active Answer in Dialog
			if (iVoiceInput.ToLower() == "quit") {
				BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
				new StpAppCmd().Execute();
			} else {
				BYOS.Instance.Interaction.Mood.Set(MoodType.THINKING);
				StartCoroutine(RequestWolfram(iVoiceInput));
			}
		}

		IEnumerator RequestWolfram(String iQuestion)
		{
			if (!string.IsNullOrEmpty(iQuestion)) {

				//var requestUriString = string.Format("https://api.wolframalpha.com/v1/result?i={0}%3F&appid=Y5P9Q6-84A9H28PKT", Uri.EscapeUriString(iQuestion));
				var requestUriString = string.Format("http://api.wolframalpha.com/v1/conversation.jsp?appid=XJHHH8-TT59GH9JX8&i={0}", Uri.EscapeUriString(iQuestion));
				Debug.Log(requestUriString);

				using (UnityWebRequest request = UnityWebRequest.Get(requestUriString)) {
					yield return request.Send();

					if (request.isError) // Error
					{
						BYOS.Instance.Interaction.Mood.Set(MoodType.SAD);
						Debug.Log(request.error);
						pendingRequest = false;
					} else // Success
					  {
						BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
						Debug.Log("ANSWER: " + request.downloadHandler.text);

						string lRes = "";
						string[] lFragmant = request.downloadHandler.text.Split('"');

						for (int i = 0; i<lFragmant.Length; ++i) {
							if(lFragmant[i] == "result") {
								lRes = lFragmant[i + 2];
								Debug.Log(lRes);
								break;
							}
						}

						if (!string.IsNullOrEmpty(lRes)) {

							lRes = lRes.Replace("Wolfram|Alpha did not understand your input", "Sorry, I didn't find an answer...");

							lRes = lRes.Replace("an empty list", "Sorry, I have no answer to your question...");
							lRes = lRes.Replace("Wolfram|Alpha", "I");
							lRes = lRes.Replace("Wolfram Alpha", "Buddy");
							lRes = lRes.Replace("No short answer available", "I didn't find any answer...");

						}else
							lRes = "I didn't find any answer...";

						BYOS.Instance.Interaction.TextToSpeech.Say(lRes);
						pendingRequest = false;
					}

					//ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;


				}
			}
		}



	}
}