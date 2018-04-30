using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Buddy;
using OpenCVUnity;

namespace BuddyApp.Diagnostic
{
	public class VocalWindow : MonoBehaviour
	{
		private SpeechToText mSTT;
		private SphinxTrigger mSphinx;
		private TextToSpeech mTTS;
		private string mSentence;
        private NoiseDetection mNoiseDetection;

        private float mIntensity = 0.0f;
        private Queue<float> mSoundIntensities;
        private int mNbSoundPics = 50;
        private Mat mMatShow;
        private Texture2D mTexture;

        public Text mSttState;
		public Text mSttOnBestReco;
		public Text mSttSOnError;
		//public Text mSphinxState;
		public Text mSphinxHasTrig;
        public RawImage mRaw;

        void Start()
		{
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mSTT = BYOS.Instance.Interaction.SpeechToText;
            mNoiseDetection = BYOS.Instance.Perception.Noise;

            mSTT.OnBestRecognition.Add(OnSpeechRecognition);
			mSTT.OnErrorEnum.Add(ErrorSTT);
			mSphinx = BYOS.Instance.Interaction.SphinxTrigger;
			mSentence = "";

            mMatShow = new Mat(480, 640, CvType.CV_8UC3);
            mTexture = new Texture2D(640, 480);

            mIntensity = 0.0f;
            mSoundIntensities = new Queue<float>();
            for (int i = 0; i < mNbSoundPics; i++)
            {
                mSoundIntensities.Enqueue(0.0f);
            }
            mNoiseDetection.OnDetect(OnNewSound, 0.0f);
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

            DisplaySound();
        }

        private void OnDisable()
        {
            mNoiseDetection.StopOnDetect(OnNewSound);
        }

        void OnSpeechRecognition(string iSpeech)
		{
			mSttOnBestReco.text = iSpeech;
            mNoiseDetection.OnDetect(OnNewSound, 0.0f);
        }

		void ErrorSTT(STTError iSpeech)
		{
			mSttSOnError.text = iSpeech.ToString();
            mNoiseDetection.OnDetect(OnNewSound, 0.0f);
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
            mNoiseDetection.StopOnDetect(OnNewSound);
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

        private bool OnNewSound(float iNoise)
        {
            mIntensity = iNoise;
            return true;
        }

        private void DisplaySound()
        {
            mMatShow = new Mat(480, 640, CvType.CV_8UC3, new Scalar(255, 255, 255, 255));
            float lLevelSound = mIntensity * 400.0f / 0.3F;

            mSoundIntensities.Enqueue(lLevelSound);
            mSoundIntensities.Dequeue();
            float lWidthPic = 640.0f / mNbSoundPics;
            int lIt = 0;
            foreach (float intensity in mSoundIntensities)
            {
                Imgproc.rectangle(mMatShow, new Point(lIt * lWidthPic, 480), new Point((lIt + 1) * lWidthPic, 480.0f - intensity), new Scalar(0, 212, 209, 255), -1);
                lIt++;
            }
            
            Utils.MatToTexture2D(mMatShow, mTexture);
            mRaw.texture = mTexture;
        }
    }
}
