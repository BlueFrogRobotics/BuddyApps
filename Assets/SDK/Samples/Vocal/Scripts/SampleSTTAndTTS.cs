using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

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
        private VocalManager mVocalManager;

        // Use this for initialization
        void Start()
        {
            STT = BYOS.Instance.SpeechToText;
            TTS = BYOS.Instance.TextToSpeech;
            mVocalManager = BYOS.Instance.VocalManager;
            mVocalManager.OnEndReco = AnswerTextEvent;
        }

        void Update()
        {
            answerText.text = STT.LastAnswer;
        }

        public void LaunchRequestButton()
        {
            mVocalManager.StartInstantReco();
        }

        public void AnswerTextEvent(string iMsg)
        {
            TTS.Say(iMsg);
        }
    }
}