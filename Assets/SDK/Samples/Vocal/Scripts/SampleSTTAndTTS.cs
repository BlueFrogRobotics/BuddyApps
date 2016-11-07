using UnityEngine;
using UnityEngine.UI;
using System;
using BuddyOS;
using System.Collections;

namespace BuddySample
{
    /// <summary>
    /// Sample to handle TTS and STT requests
    /// </summary>
    public class SampleSTTAndTTS : MonoBehaviour
    {
        [SerializeField]
        private Text answerText;

        private TextToSpeech TTS;
        private SpeechToText STT;
        private VocalActivation mVocalActivation;

        // Use this for initialization
        void Start()
        {
            STT = BYOS.Instance.SpeechToText;
            TTS = BYOS.Instance.TextToSpeech;
            mVocalActivation = BYOS.Instance.VocalActivation;
            mVocalActivation.VocalProcessing = AnswerTextEvent;
        }

        void Update()
        {
            answerText.text = STT.LastAnswer;
        }

        public void LaunchRequestButton()
        {
            mVocalActivation.StartInstantReco();
        }

        public void AnswerTextEvent(string iMsg)
        {
            TTS.Say(iMsg);
        }
    }
}