using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using System.Collections.Generic;



namespace BuddyApp.SandboxApp
{
    /// <summary>
    /// State where we ask the user to choose between the different monitoring modes
    /// </summary>
    public class SAMenuState : AStateMachineBehaviour
    {

        [Serializable]
        public class MenuItem
        {
            public string key;
            public string trigger;
            public bool quitApp;
        }

        [SerializeField]
        private string titleKey;

        [SerializeField]
        private string speechKey;

        [SerializeField]
        private List<MenuItem> items;

        //private List<string> mStartPhonetics;
        //private List<string> mParameterPhonetics;
        //private List<string> mQuitPhonetics;

        private string mSpeechReco;

        private bool mHasDisplayChoices;
        private bool mListening;
        private bool mHasLoadedTTS;

        private float mTimer = 0.0f;

        public override void Start()
        {
            Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Header.DisplayParametersButton = false;
            //mStartPhonetics = new List<string>(Dictionary.GetPhoneticStrings("start"));
            //mParameterPhonetics = new List<string>(Dictionary.GetPhoneticStrings("detectionparameters"));
            //mQuitPhonetics = new List<string>(Dictionary.GetPhoneticStrings("quit"));
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            BYOS.Instance.Header.DisplayParametersButton = false;
            BYOS.Instance.Primitive.TouchScreen.UnlockScreen();
            mHasLoadedTTS = true;
            //Debug.Log("[TTS] Has TTS been setup: " + Interaction.TextToSpeech.IsSetup);
            Interaction.TextToSpeech.Say(Dictionary.GetRandomString(speechKey));

            Interaction.VocalManager.OnEndReco = OnSpeechReco;
            Interaction.VocalManager.EnableDefaultErrorHandling = false;
            Interaction.VocalManager.OnError = Empty;
            mTimer = 0.0f;
            mListening = false;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mHasLoadedTTS)
            {
                mTimer += Time.deltaTime;
                if (mTimer > 6.0f)
                {
                    Interaction.Mood.Set(MoodType.NEUTRAL);
                    mListening = false;
                    mTimer = 0.0f;
                    mSpeechReco = null;
                }

                if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                    return;

                if (!mHasDisplayChoices)
                {
                    DisplayChoices();
                    mHasDisplayChoices = true;
                    return;
                }


                if (string.IsNullOrEmpty(mSpeechReco))
                {

                    Interaction.VocalManager.StartInstantReco();

                    Interaction.Mood.Set(MoodType.LISTENING);
                    mListening = true;
                    return;
                }
                foreach(MenuItem item in items)
                {
                    if (ContainsOneOf(mSpeechReco, new List<string>(Dictionary.GetPhoneticStrings(item.key))))
                    {
                        BYOS.Instance.Toaster.Hide();
                        //if (GuardianData.Instance.FirstRun)
                        //    GotoParameter();
                        //else
                        GotoParameter(item.trigger, item.quitApp);
                        break;
                    }
                }

                //if (ContainsOneOf(mSpeechReco, mStartPhonetics))
                //{
                //    BYOS.Instance.Toaster.Hide();
                //    //if (GuardianData.Instance.FirstRun)
                //    //    GotoParameter();
                //    //else
                //    StartGuardian();
                //}
                //else if (ContainsOneOf(mSpeechReco, mParameterPhonetics))
                //{
                //    BYOS.Instance.Toaster.Hide();
                //    //GotoParameter();
                //}
                //else if (ContainsOneOf(mSpeechReco, mQuitPhonetics))
                //{
                //    BYOS.Instance.Toaster.Hide();
                //    QuitApp();
                //}
                //else
                //{
                //    Interaction.TextToSpeech.SayKey("notunderstand", true);
                //    Interaction.TextToSpeech.Silence(1000, true);
                //    Interaction.TextToSpeech.SayKey("askchoices", true);
                //    mListening = false;
                //    mSpeechReco = null;
                //}
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);

            mSpeechReco = null;
            mHasDisplayChoices = false;
        }

        private IEnumerator WaitTTSLoading()
        {
            yield return new WaitForSeconds(1.0f);
            BYOS.Instance.Header.SpinningWheel = true;
            while (!Interaction.TextToSpeech.IsSpeaking)
                yield return null;
            mHasLoadedTTS = true;
            BYOS.Instance.Header.SpinningWheel = false;
        }


        /// <summary>
        /// Display the choice toaster
        /// </summary>
        private void DisplayChoices()
        {
            ButtonInfo[] lButtonsInfo = new ButtonInfo[items.Count];
            int i = 0;
            foreach(MenuItem item in items)
            {
                lButtonsInfo[i] = new ButtonInfo
                {
                    Label = Dictionary.GetString(item.key),
                    OnClick = delegate () { GotoParameter(item.trigger, item.quitApp); }
                };
                //if (item.quitApp)
                //    lButtonsInfo[i].OnClick = QuitApp;
                i++;
            }


            BYOS.Instance.Toaster.Display<ChoiceToast>().With(Dictionary.GetString(titleKey), lButtonsInfo);

        }

        private void OnSpeechReco(string iVoiceInput)
        {
            mSpeechReco = iVoiceInput;

            Interaction.Mood.Set(MoodType.NEUTRAL);
            mListening = false;
        }

        /// <summary>
        /// Go to the next state
        /// </summary>
        /// <param name="iMode">the chosen mode</param>
        private void StartGuardian()
        {
            mSpeechReco = null;
            Trigger("NextStep");
            //Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechReco);
            Interaction.VocalManager.OnEndReco = Empty;
        }

        /// <summary>
        /// Go to parameters
        /// </summary>
        /// <param name="iMode">the chosen mode</param>
        private void GotoParameter(string iTrigger, bool iQuit)
        {
            mSpeechReco = null;
            if (iQuit)
                QuitApp();
            Trigger(iTrigger);
            Interaction.VocalManager.OnEndReco = Empty;
        }

        private bool ContainsOneOf(string iSpeech, List<string> iListSpeech)
        {
            for (int i = 0; i < iListSpeech.Count; ++i)
            {
                string[] words = iListSpeech[i].Split(' ');
                if (words.Length < 2)
                {
                    words = iSpeech.Split(' ');
                    foreach (string word in words)
                    {
                        if (word == iListSpeech[i].ToLower())
                        {
                            return true;
                        }
                    }
                }
                else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
                    return true;
            }
            return false;
        }

        private void Empty(STTError iError)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        private void Empty(string iVoice)
        {
        }


    }
}