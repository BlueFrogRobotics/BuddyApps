﻿using System.Collections;
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

            if (Dictionary.ContainsPhonetic(iText, "on") && Dictionary.ContainsPhonetic(iText, "light") && !Dictionary.ContainsPhonetic(iText, "officeroom") && !Dictionary.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOnLivingRoomPlug();
            }
            else if (Dictionary.ContainsPhonetic(iText, "off") && Dictionary.ContainsPhonetic(iText, "light") && !Dictionary.ContainsPhonetic(iText, "officeroom") && !Dictionary.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOffLivingRoomPlug();
            }
            else if (Dictionary.ContainsPhonetic(iText, "on") && Dictionary.ContainsPhonetic(iText, "light") && Dictionary.ContainsPhonetic(iText, "officeroom") && !Dictionary.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOnOfficeRoomPlug();
            }
            else if (Dictionary.ContainsPhonetic(iText, "off") && Dictionary.ContainsPhonetic(iText, "light") && Dictionary.ContainsPhonetic(iText, "officeroom") && !Dictionary.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOffOfficeRoomPlug();
            }
            else if (Dictionary.ContainsPhonetic(iText, "on") && Dictionary.ContainsPhonetic(iText, "light") && Dictionary.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOnAllPlugs();
            }
            else if (Dictionary.ContainsPhonetic(iText, "off") && Dictionary.ContainsPhonetic(iText, "light") && Dictionary.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOffAllPlugs();
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
            else if (Dictionary.ContainsPhonetic(iText, "next") && Dictionary.ContainsPhonetic(iText, "song"))
            {
                mSomfyBehaviour.NextMusic();
            }
            else if (Dictionary.ContainsPhonetic(iText, "previous") && Dictionary.ContainsPhonetic(iText, "song"))
            {
                mSomfyBehaviour.PreviousMusic();
            }
            else if (Dictionary.ContainsPhonetic(iText, "rewind") && Dictionary.ContainsPhonetic(iText, "song"))
            {
                mSomfyBehaviour.RewindMusic();
            }
            else if (Dictionary.ContainsPhonetic(iText, "temperature") && Dictionary.ContainsPhonetic(iText, "what"))
            {
                //Interaction.TextToSpeech.Say("The temperature is " + float.Parse(mSomfyBehaviour.GetTemperature()).ToString().Replace(".", " point ") + " degrees");
                string lTemp = mSomfyBehaviour.GetTemperature();
                if (BYOS.Instance.Language.CurrentLang == Language.FR)
                {
                    Debug.Log("dans fr");
                    lTemp = lTemp.Replace('.', ',');
                }
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("gettemperature").Replace("[temperature]", lTemp));
            }
            else if (Dictionary.ContainsPhonetic(iText, "setvolume"))
            {
                string lParse = iText.ToLower();
                foreach (string lPhonetic in Dictionary.GetPhoneticStrings("setvolume"))
                {
                    lParse = lParse.Replace(lPhonetic, "");
                }
                foreach (string lPhonetic in Dictionary.GetPhoneticStrings("percent"))
                {
                    lParse = lParse.Replace(lPhonetic, "");
                }
                lParse = lParse.Trim();
                float lVolume = 0;
                if (float.TryParse(lParse, out lVolume))
                {
                    mSomfyBehaviour.SetSonosVolume(lVolume);
                }
                Debug.Log("texte parse: " + lParse);
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
                if (float.TryParse(lParse, out lTemperature))
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