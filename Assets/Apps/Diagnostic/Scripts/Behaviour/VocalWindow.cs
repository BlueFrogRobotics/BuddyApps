using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Buddy;

namespace BuddyApp.Diagnostic
{
	public class VocalWindow : MonoBehaviour
	{
		private SpeechToText mSTT;
		private SphinxTrigger mSphinx;
		private TextToSpeech mTTS;
		private string mSentence;

		public Text mSttState;
		public Text mSttOnBestReco;
		public Text mSttSOnError;
		//public Text mSphinxState;
		public Text mSphinxHasTrig;

		void Start()
		{
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mSTT = BYOS.Instance.Interaction.SpeechToText;

			mSTT.OnBestRecognition.Add(OnSpeechRecognition);
			mSTT.OnErrorEnum.Add(ErrorSTT);
			mSphinx = BYOS.Instance.Interaction.SphinxTrigger;
			mSentence = "";
		}

		void Update()
		{
			if (!mSTT.HasFinished && mSttState.text != "Listenning") {
				mSttState.text = "Listenning";
				if (BYOS.Instance.Interaction.Mood.CurrentMood != MoodType.LISTENING) {
					BYOS.Instance.Interaction.Mood.Set(MoodType.LISTENING);
				}
			}
			else if(mSTT.HasFinished && mSttState.text == "Listenning") {
				mSttState.text = "Not Listenning";
				if (BYOS.Instance.Interaction.Mood.CurrentMood != MoodType.NEUTRAL) {
					BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);

				}
			}


			if (BYOS.Instance.Interaction.SphinxTrigger.HasTriggered && mSphinxHasTrig.text != "SphinxTriggered")
				mSphinxHasTrig.text = "SphinxTriggered";
			else if (!BYOS.Instance.Interaction.SphinxTrigger.HasTriggered && mSphinxHasTrig.text == "SphinxTriggered")
				mSphinxHasTrig.text = "SphinxNotTriggered";
		}

		void OnSpeechRecognition(string iSpeech)
		{
			mSttOnBestReco.text = iSpeech;
		}

		void ErrorSTT(STTError iSpeech)
		{
			mSttSOnError.text = iSpeech.ToString();
		}

		public void UpdateSentence(string iValue)
		{
			mSentence = iValue;
		}

		public void Say()
		{
			Debug.Log("say tts " + mSentence);
			mTTS.Say(mSentence);
		}

		public void SetEnglish()
		{
			//mSTT.SetLanguage(Language.ENG);
			//mSphinx.SetLanguage(Language.ENG);
			//mTTS.SetLanguage(Language.ENG);
		}

		public void SetFrench()
		{
			//mSTT.SetLanguage(Language.FRA);
			//mSphinx.SetLanguage(Language.FRA);
			//mTTS.SetLanguage(Language.FRA);
		}

		public void STTRequest()
		{
			Debug.Log("Start stt request");
			mSTT.Request();
		}


		public void StartSphinx()
		{
			Debug.Log("StartSphinx");
			BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();
		}

		public void StopSphinx()
		{
			Debug.Log("StopSphinx");
			BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
		}
	}
}
