using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Alarm
{
    public class AlarmListener : MonoBehaviour
    {
        [SerializeField]
        private Counter counter;

        private TextToSpeech mSynthesis;
        private SpeechToText mRecognition;

        void Awake()
        {
            mSynthesis = BYOS.Instance.TextToSpeech;
            mRecognition = BYOS.Instance.SpeechToText;
        }

        // Use this for initialization
        void Start()
        {
            mRecognition.OnBestRecognition.Add(CallBackRecognition);
        }

        public void AskQuestion(string iQuestion)
        {
            mSynthesis.Say(iQuestion);
            StartCoroutine(StartSTT());
        }

        public void Say(string iSentence)
        {
            mSynthesis.Say(iSentence);
        }

        public void STTRequest()
        {
            mRecognition.Request();
        }

        private void CallBackRecognition(string iMsg)
        {
            // here we parse the result in order to use it
            int lTimeForCounter = 0;
            string[] lWords = iMsg.Split(' ');
            for (int i = 1; i < lWords.Length; ++i) {
                if (lWords[i].ToLower().Contains("seconde") || lWords[i].ToLower().Contains("secondes")) {
                    int valueInSeconds = 0;
                    if (lWords[i - 1].ToLower().Contains("une")) {
                        lTimeForCounter = lTimeForCounter + 1;
                    }
                    if (System.Int32.TryParse(lWords[i - 1], out valueInSeconds)) {
                        lTimeForCounter = lTimeForCounter + valueInSeconds;
                    }
                } else if (lWords[i].ToLower().Contains("minute") || lWords[i].ToLower().Contains("minutes")) {
                    int valueInMinute = 0;
                    if (lWords[i - 1].ToLower().Contains("une")) {
                        lTimeForCounter = lTimeForCounter + 60;
                    }
                    if (System.Int32.TryParse(lWords[i - 1], out valueInMinute)) {
                        lTimeForCounter = lTimeForCounter + valueInMinute * 60;
                    }
                } else if (lWords[i].ToLower().Contains("heure") || lWords[i].ToLower().Contains("heures")) {
                    int valueInHeure = 0;
                    if (lWords[i - 1].ToLower().Contains("une")) {
                        lTimeForCounter = lTimeForCounter + 3600;
                    }
                    if (System.Int32.TryParse(lWords[i - 1], out valueInHeure)) {
                        lTimeForCounter = lTimeForCounter + valueInHeure * 3600;
                    }
                }
            }
            if (lTimeForCounter == 0) {
                AskQuestion("Je n'ai pas compris, peux tu repeter ? ");
                return;
            }

            Say("Parfait, Je te reveil dans " + lTimeForCounter.ToString() + " secondes");
            counter.SetCounter(lTimeForCounter);
            counter.StartCounter();
        }

        private IEnumerator StartSTT()
        {
            yield return new WaitForSeconds(4F);
            mRecognition.Request();
        }
    }
}