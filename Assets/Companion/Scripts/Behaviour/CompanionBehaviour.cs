using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    internal class CompanionBehaviour : MonoBehaviour
    {
        private TextToSpeech mTTS;
        private VocalActivation mVocalActivation;

        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mVocalActivation = BYOS.Instance.VocalActivation;
            mVocalActivation.VocalProcessing = VocalProcessing;
            mVocalActivation.StartRecoWithTrigger();

            SpeechToText lSTT = BYOS.Instance.SpeechToText;
            lSTT.OnBestRecognition.Add(VocalProcessing);
        }

        private void VocalProcessing(string iRequest)
        {
            if (iRequest.Contains("poney"))
                mTTS.Say("Prout");
            else if (iRequest.Contains("valentin"))
                mTTS.Say("Moi c'est Beudi");
            else if (iRequest.Contains("vas tu") || iRequest.Contains("ça va"))
                mTTS.Say("Je te mange la cervelle avec de la sauce samouraille");
        }
    }
}