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
        private bool mChangeState;

        public override void Start()
        {
            mSomfyBehaviour = GetComponent<SomfyBehaviour>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mInitialized = false;
            mChangeState = false;
            if (string.IsNullOrEmpty(SomfyData.Instance.VocalRequest))
                StartCoroutine(InitDevice());
            else
            {
                StartCoroutine(FromCompanion());
            }
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mInitialized)
            {
                if (mChangeState)
                {
                    Interaction.Face.OnClickLeftEye.Remove(OnClick);
                    Interaction.Face.OnClickRightEye.Remove(OnClick);
                    //Interaction.Face.OnClickMouth.Remove(OnClick);
                    Trigger("NextStep");
                }

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
            ResetTrigger("NextStep");
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
            else if (Dictionary.ContainsPhonetic(iText, "off") && Dictionary.ContainsPhonetic(iText, "light"))
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
            else if (Dictionary.ContainsPhonetic(iText, "playmusic"))
            {
                mSomfyBehaviour.PlayMusic();
            }
            else if (Dictionary.ContainsPhonetic(iText, "stopmusic"))
            {
                mSomfyBehaviour.StopMusic();
            }
            else if (Dictionary.ContainsPhonetic(iText, "temperature") && Dictionary.ContainsPhonetic(iText, "what"))
            {
                Interaction.TextToSpeech.Say("The temperature is "+float.Parse(mSomfyBehaviour.GetTemperature()).ToString().Replace(".", " point ") + " degrees");
            }
            else if (Dictionary.ContainsPhonetic(iText, "settemperature"))
            {
                string lParse = iText.ToLower();
                foreach (string lPhonetic in Dictionary.GetPhoneticStrings("settemperature"))
                {
                    lParse = lParse.Replace(lPhonetic, "");
                }
                foreach (string lPhonetic in Dictionary.GetPhoneticStrings("degree"))
                {
                    lParse = lParse.Replace(lPhonetic, "");
                }
                lParse = lParse.Trim();
                float lTemperature = 0;
                if(float.TryParse(lParse, out lTemperature))
                {
                    mSomfyBehaviour.SetTemperature(lTemperature);
                }
                Debug.Log("texte parse: " + lParse);
            }
        }

        private IEnumerator InitDevice()
        {
            Debug.Log("nombre de devices: " + mSomfyBehaviour.Box.Devices.Count);
            while (mSomfyBehaviour.Box.Devices.Count == 0)
                yield return null;
            mSomfyBehaviour.GetDevices();
            mListening = false;
            mSpeechReco = "";
            Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechReco);
            Interaction.SpeechToText.OnError.Add(OnEnd);
            Debug.Log("avant add onclick");
            Interaction.Face.OnClickLeftEye.Add(OnClick);
            Interaction.Face.OnClickRightEye.Add(OnClick);
            //Interaction.Face.OnClickMouth.Add(OnClick);
            mInitialized = true;
            mChangeState = false;
        }

        private IEnumerator FromCompanion()
        {
            Debug.Log("vocal request: " + SomfyData.Instance.VocalRequest);
            while (mSomfyBehaviour.Box.Devices.Count == 0)
                yield return null;
            mSomfyBehaviour.GetDevices();
            ParseText(SomfyData.Instance.VocalRequest);
            while (Interaction.TextToSpeech.IsSpeaking)
                yield return null;
            yield return new WaitForSeconds(1.5f);
            QuitApp();
        }

        private void OnClick()
        {
            Debug.Log("onclick");
            mChangeState = true;
        }
    } 
}