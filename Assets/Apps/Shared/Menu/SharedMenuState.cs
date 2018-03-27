using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using System.Collections.Generic;



namespace BuddyApp.Shared
{
    /// <summary>
    /// State where we show a menu using buttons or the stt to activate triggers
    /// </summary>
    public class SharedMenuState : ASharedSMB
    {

        [Serializable]
        public class MenuItem
        {
            public string key;
            public string trigger;
            public bool quitApp;
        }

        [Header("Key of menu title")]
        [SerializeField]
        private string titleKey;

        [Header("Key of the text to be said")]
        [SerializeField]
        private string speechKey;

        [Header("Buttons of the menu")]
        [SerializeField]
        private List<MenuItem> items;

        private string mSpeechReco;

        private bool mHasDisplayChoices;
        private bool mListening;

        private float mTimer = 0.0f;

        public override void Start()
        {
            Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Header.DisplayParametersButton = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            BYOS.Instance.Header.DisplayParametersButton = false;
            BYOS.Instance.Primitive.TouchScreen.UnlockScreen();
            Interaction.TextToSpeech.Say(Dictionary.GetRandomString(speechKey));

            Interaction.VocalManager.OnEndReco = OnSpeechReco;
            Interaction.VocalManager.EnableDefaultErrorHandling = false;
            Interaction.VocalManager.OnError = Empty;
            mTimer = 0.0f;
            mListening = false;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
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
                    ActivateTrigger(item.trigger, item.quitApp);
                    break;
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);

            mSpeechReco = null;
            mHasDisplayChoices = false;
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
                    OnClick = delegate () { ActivateTrigger(item.trigger, item.quitApp); }
                };
                
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
        /// Activate choosen trigger or quit the app
        /// </summary>
        /// <param name="iTrigger">the trigger to activate</param>
        /// <param name="iQuit">will quit the app if set to true</param>
        private void ActivateTrigger(string iTrigger, bool iQuit)
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