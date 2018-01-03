using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy.UI;
using Buddy;

namespace BuddyApp.Somfy
{
    public class SomfyVocalCommands : AStateMachineBehaviour
    {
        private SomfyBehaviour mSomfyBehaviour;
        private bool mListening;
        private string mSpeechReco;
        private bool mInitialized;

        public override void Start()
        {
            mSomfyBehaviour = GetComponent<SomfyBehaviour>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mInitialized = false;
            StartCoroutine(InitDevice());
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mInitialized)
            {
                if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                    return;

                if (string.IsNullOrEmpty(mSpeechReco))
                {
                    Interaction.SpeechToText.Request();
                    mListening = true;

                    Interaction.Mood.Set(MoodType.LISTENING);
                    return;
                }

                else
                {
                    ParseText(mSpeechReco);
                    mSpeechReco = "";
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechReco);
            Interaction.SpeechToText.OnError.Remove(OnEnd);
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        private void OnSpeechReco(string iText)
        {
            Debug.Log("recu: " + iText);
            Interaction.Mood.Set(MoodType.NEUTRAL);

            mSpeechReco = iText;
            mListening = false;
        }

        private void OnEnd(string iError)
        {
            //mSpeechReco = "";
            mListening = false;
        }

        private void ParseText(string iText)
        {
            
            if (Dictionary.ContainsPhonetic(iText, "on") && Dictionary.ContainsPhonetic(iText, "light"))
            {
                mSomfyBehaviour.SwitchOnPlug();
            }
            else if(Dictionary.ContainsPhonetic(iText, "off") && Dictionary.ContainsPhonetic(iText, "light"))
            {
                mSomfyBehaviour.SwitchOffPlug();
            }
            else if (Dictionary.ContainsPhonetic(iText, "open") && Dictionary.ContainsPhonetic(iText, "store"))
            {
                mSomfyBehaviour.OpenStore();
            }
            else if (Dictionary.ContainsPhonetic(iText, "close") && Dictionary.ContainsPhonetic(iText, "store"))
            {
                mSomfyBehaviour.CloseStore();
            }
        }

        private IEnumerator InitDevice()
        {
            Debug.Log("nombre de devices: " + mSomfyBehaviour.Box.Devices.Count);
            while(mSomfyBehaviour.Box.Devices.Count==0)
                yield return null;
            mSomfyBehaviour.GetDevices();
            mListening = false;
            mSpeechReco = "";
            Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechReco);
            Interaction.SpeechToText.OnError.Add(OnEnd);
            mInitialized = true;
        }
        
    }
}