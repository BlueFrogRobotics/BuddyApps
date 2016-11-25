using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class VocalWindow : MonoBehaviour
    {
        private SpeechToText mSTT;
        private SphinxTrigger mSphinx;
        private TextToSpeech mTTS;

        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;
            mSphinx = BYOS.Instance.SphinxTrigger;
        }

        void Update()
        {
        }

        public void SetEnglish()
        {
            mSTT.SetLanguage(Language.ENG);
            mSphinx.SetLanguage(Language.ENG);
            mTTS.SetLanguage(Language.ENG);
        }

        public void SetFrench()
        {
            mSTT.SetLanguage(Language.FRA);
            mSphinx.SetLanguage(Language.FRA);
            mTTS.SetLanguage(Language.FRA);
        }

        public void STTRequest()
        {
            mSTT.Request();
        }
    }
}
