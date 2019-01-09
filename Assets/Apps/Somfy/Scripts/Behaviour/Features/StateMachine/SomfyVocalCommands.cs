using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

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
            {
                Debug.Log("vocal request est nul!!!!!!");
                StartCoroutine(InitDevice());
            }
            else
            {
                Debug.Log("vocal request n est pas du tout nul!!!!!!");
                StartCoroutine(FromCompanion());
            }
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mInitialized)
            {
                if (mChangeState)
                {
                    Buddy.Behaviour.Face.OnTouchLeftEye.Remove(OnClick);
                    Buddy.Behaviour.Face.OnTouchRightEye.Remove(OnClick);
                    //Interaction.Face.OnClickMouth.Remove(OnClick);
                    Trigger("NextStep");
                }

                if (Buddy.Vocal.IsSpeaking || mListening)
                    return;

                if (string.IsNullOrEmpty(mSpeechReco))
                {
                    Buddy.Vocal.Listen();
                    mListening = true;

                    Buddy.Behaviour.Mood = Mood.LISTENING;
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
            Buddy.Vocal.OnEndListening.Remove(OnSpeechReco);
            Buddy.Behaviour.Face.OnTouchLeftEye.Remove(OnClick);
            Buddy.Behaviour.Face.OnTouchRightEye.Remove(OnClick);
            //Interaction.SpeechToText.OnError.Remove(OnEnd);
            Buddy.Behaviour.Mood = Mood.NEUTRAL;
        }

        private void OnSpeechReco(SpeechInput iInput)
        {
            Debug.Log("recu: " + iInput.Utterance);
            Buddy.Behaviour.Mood = Mood.NEUTRAL;

            mSpeechReco = iInput.Utterance;
            mListening = false;
        }

        private void OnEnd(string iError)
        {
            //mSpeechReco = "";
            mListening = false;
        }

        private void ParseText(string iText)
        {

            if (Buddy.Resources.ContainsPhonetic(iText, "on") && Buddy.Resources.ContainsPhonetic(iText, "light") && !Buddy.Resources.ContainsPhonetic(iText, "officeroom") && !Buddy.Resources.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOnLivingRoomPlug();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "off") && Buddy.Resources.ContainsPhonetic(iText, "light") && !Buddy.Resources.ContainsPhonetic(iText, "officeroom") && !Buddy.Resources.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOffLivingRoomPlug();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "on") && Buddy.Resources.ContainsPhonetic(iText, "light") && Buddy.Resources.ContainsPhonetic(iText, "officeroom") && !Buddy.Resources.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOnOfficeRoomPlug();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "off") && Buddy.Resources.ContainsPhonetic(iText, "light") && Buddy.Resources.ContainsPhonetic(iText, "officeroom") && !Buddy.Resources.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOffOfficeRoomPlug();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "on") && Buddy.Resources.ContainsPhonetic(iText, "light") && Buddy.Resources.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOnAllPlugs();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "off") && Buddy.Resources.ContainsPhonetic(iText, "light") && Buddy.Resources.ContainsPhonetic(iText, "all"))
            {
                mSomfyBehaviour.SwitchOffAllPlugs();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "open") && Buddy.Resources.ContainsPhonetic(iText, "store"))
            {
                mSomfyBehaviour.OpenStore();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "close") && Buddy.Resources.ContainsPhonetic(iText, "store"))
            {
                mSomfyBehaviour.CloseStore();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "playmusic") && Buddy.Resources.ContainsPhonetic(iText, "song"))
            {
                mSomfyBehaviour.PlayMusic();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "stopmusic") && Buddy.Resources.ContainsPhonetic(iText, "song"))
            {
                mSomfyBehaviour.StopMusic();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "next") && Buddy.Resources.ContainsPhonetic(iText, "song"))
            {
                mSomfyBehaviour.NextMusic();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "previous") && Buddy.Resources.ContainsPhonetic(iText, "song"))
            {
                mSomfyBehaviour.PreviousMusic();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "rewind") && Buddy.Resources.ContainsPhonetic(iText, "song"))
            {
                mSomfyBehaviour.RewindMusic();
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "temperature") && Buddy.Resources.ContainsPhonetic(iText, "what"))
            {
                //Interaction.TextToSpeech.Say("The temperature is " + float.Parse(mSomfyBehaviour.GetTemperature()).ToString().Replace(".", " point ") + " degrees");
                string lTemp = mSomfyBehaviour.GetTemperature();
                if (Buddy.Platform.Language.SystemInputLanguage.ISO6391Code == ISO6391Code.FR)
                {
                    Debug.Log("dans fr");
                    lTemp = lTemp.Replace('.', ',');
                }
                Buddy.Vocal.Say(Buddy.Resources.GetRandomString("gettemperature").Replace("[temperature]", lTemp));
            }
            else if (Buddy.Resources.ContainsPhonetic(iText, "setvolume"))
            {
                string lParse = iText.ToLower();
                foreach (string lPhonetic in Buddy.Resources.GetPhoneticStrings("setvolume"))
                {
                    lParse = lParse.Replace(lPhonetic, "");
                }
                foreach (string lPhonetic in Buddy.Resources.GetPhoneticStrings("percent"))
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
            else if (Buddy.Resources.ContainsPhonetic(iText, "settemperature"))
            {
                string lParse = iText.ToLower();
                foreach (string lPhonetic in Buddy.Resources.GetPhoneticStrings("settemperature"))
                {
                    lParse = lParse.Replace(lPhonetic, "");
                }
                foreach (string lPhonetic in Buddy.Resources.GetPhoneticStrings("degree"))
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
            Debug.Log("avant get device");
            Debug.Log("les devices trouves: " + mSomfyBehaviour.Box.Devices.Count);
            mSomfyBehaviour.GetDevices();
            mListening = false;
            mSpeechReco = "";
            Buddy.Vocal.OnEndListening.Add(OnSpeechReco);
            //Interaction.SpeechToText.OnError.Add(OnEnd);
            Debug.Log("avant add onclick");
            Buddy.Behaviour.Face.OnTouchLeftEye.Add(OnClick);
            Buddy.Behaviour.Face.OnTouchRightEye.Add(OnClick);
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
            while (Buddy.Vocal.IsSpeaking)
                yield return null;
            yield return new WaitForSeconds(2.5f);
            while (Buddy.Vocal.IsSpeaking)
                yield return null;
            QuitApp();
        }

        private void OnClick()
        {
            Debug.Log("onclick");
            mChangeState = true;
        }
    }
}