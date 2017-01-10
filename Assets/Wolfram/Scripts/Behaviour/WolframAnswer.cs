using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using BuddyOS.Command;
using UnityEngine.Experimental.Networking;

namespace BuddyApp.Wolfram
{
	public class WolframAnswer : MonoBehaviour
	{
		[SerializeField]
		private AnimManager mAnimationManager;
		SpeechToText mSTT;
		TextToSpeech mTTS;
		private Mood mMood;
		private string mLastSpeech;
		private bool pendingRequest;

		void Start()
		{
			mSTT = BYOS.Instance.SpeechToText;
			mTTS = BYOS.Instance.TextToSpeech;
			mMood = BYOS.Instance.Mood;

			mLastSpeech = "";



			mSTT.OnBestRecognition.Add(OnSpeechRecognition);

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		void Update()
		{
			if (mTTS.HasFinishedTalking && !pendingRequest) {
				pendingRequest = true;
				mMood.Set(MoodType.LISTENING);
				mSTT.Request();
			}
		}


		// speech reco callback
		private void OnSpeechRecognition(string iVoiceInput)
		{

			Debug.Log("OnSpeechReco");
			Debug.Log("[Knowledge] : " + iVoiceInput);

			// set active Answer in Dialog
			if (iVoiceInput.ToLower() == "quit") {
				mMood.Set(MoodType.NEUTRAL);
				new HomeCmd().Execute();
			} else {
				mMood.Set(MoodType.THINKING);
				StartCoroutine(RequestWolfram(iVoiceInput));
			}
		}

		IEnumerator RequestWolfram(String iQuestion)
		{
			if (!string.IsNullOrEmpty(mSTT.LastAnswer)) {

				var requestUriString = string.Format("https://api.wolframalpha.com/v1/result?i={0}%3F&appid=Y5P9Q6-84A9H28PKT", Uri.EscapeUriString(iQuestion));

				Debug.Log(requestUriString);

				using (UnityWebRequest request = UnityWebRequest.Get(requestUriString)) {
					yield return request.Send();

					if (request.isError) // Error
					{
						mMood.Set(MoodType.SAD);
						Debug.Log(request.error);
						pendingRequest = false;
					} else // Success
					  {
						mMood.Set(MoodType.NEUTRAL);
						Debug.Log("ANSWER: " + request.downloadHandler.text);
						string lRes = request.downloadHandler.text.Replace("Wolfram|Alpha did not understand your input", "Sorry, I didn't find an answer...");

						lRes = lRes.Replace("an empty list", "Sorry, I have no answer to your question...");
						lRes = lRes.Replace("Wolfram|Alpha", "I" );
						lRes = lRes.Replace("Wolfram Alpha", "Buddy");
						lRes = lRes.Replace("No short answer available", "I didn't find any answer...");


						BYOS.Instance.TextToSpeech.Say(lRes);
						pendingRequest = false;
					}

					//ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;


				}
			}
		}


		//	IEnumerator RequestWolfram(String iQuestion)
		//{
		//		if (!string.IsNullOrEmpty(mSTT.LastAnswer)) {
		//			var requestUriString = string.Format("https://api.wolframalpha.com/v1/result?i={0}%3F&appid=Y5P9Q6-84A9H28PKT", Uri.EscapeUriString(iQuestion));
		//			Debug.Log(requestUriString);
		//			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
		//			Debug.Log("Pre req");

		//			var responseStream = WebRequest.Create(requestUriString).GetResponse().GetResponseStream();
		//			var responseString = new StreamReader(responseStream).ReadToEnd();
		//			Debug.Log("Post reponse");


		//			Debug.Log("ANSWER: " + responseString);
		//			BYOS.Instance.TextToSpeech.Say(responseString);

		//			pendingRequest = false;
		//			yield return null;

		//		}
		//	}


		public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool isOk = true;
			// If there are errors in the certificate chain, look at each error to determine the cause.
			if (sslPolicyErrors != SslPolicyErrors.None) {
				for (int i = 0; i < chain.ChainStatus.Length; i++) {
					if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown) {
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						bool chainIsValid = chain.Build((X509Certificate2)certificate);
						if (!chainIsValid) {
							isOk = false;
						}
					}
				}
			}
			return isOk;
		}

	}
}
