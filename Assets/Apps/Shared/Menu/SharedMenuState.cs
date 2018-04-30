using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BuddyApp.Shared
{
    /// <summary>
    /// State where we show a menu using buttons or the stt to activate triggers
    /// </summary>
    public class SharedMenuState : ASharedSMB
    {

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

        private List<MenuItem> items = new List<MenuItem>(0);

        [SerializeField]
        private string NameOfXML;

        private int mNumberOfButton;
        private int mIndexButton = 0;

        private string mSpeechReco;

        private bool mHasDisplayChoices;
        private bool mListening;
        private bool mHasLoadedTTS;

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
            mHasLoadedTTS = true;
            if(!string.IsNullOrEmpty(speechKey))
            {
                if (!string.IsNullOrEmpty(Dictionary.GetRandomString(speechKey)))
                {
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString(speechKey));
                }
                else if (!string.IsNullOrEmpty(Dictionary.GetString(speechKey)))
                {
                    Interaction.TextToSpeech.Say(Dictionary.GetString(speechKey));
                }
            }
            
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
                foreach (MenuItem item in items)
                {
                    if (SharedVocalFunctions.ContainsOneOf(mSpeechReco, new List<string>(Dictionary.GetPhoneticStrings(item.key))))
                    {
                        BYOS.Instance.Toaster.Hide();
                        GotoParameter(item.trigger, item.quitApp);
                        break;
                    }
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.SpeechToText.Stop();
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
            Debug.Log("display choice");

            FillMenu();
            Debug.Log("display count " + items.Count);
            ButtonInfo[] lButtonsInfo = new ButtonInfo[items.Count];
            int i = 0;
            foreach (MenuItem item in items)
            {
                lButtonsInfo[i] = new ButtonInfo
                {
                    Label = Dictionary.GetString(item.key),
                    OnClick = delegate () { GotoParameter(item.trigger, item.quitApp); }
                };
                i++;
            }
            Debug.Log("apres foreach");

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
        }

        private void Empty(STTError iError)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        void AddNewButton()
        {
            items.Add(new MenuItem());
        }

        private void FillMenu()
        {
            string lPath = BYOS.Instance.Resources.GetPathToRaw("XMLShared/Menu");
            bool lResult = false;
            int lValue = 0;

            if (File.Exists(lPath + "/" + NameOfXML + ".xml"))
            {
                XmlDocument lDoc = new XmlDocument();
                lDoc.Load(lPath + "/" + NameOfXML + ".xml");

                XmlElement lElmt = lDoc.DocumentElement;
                XmlNodeList lNodeList = lElmt.ChildNodes;
                if (lNodeList[0].Name == "ListSize")
                {

                    lResult = int.TryParse(lNodeList[0].InnerText, out lValue);
                    if (lResult)
                        mNumberOfButton = lValue;
                }

                for (int i = 0; i < lNodeList.Count; ++i)
                {
                    if (lNodeList[i].Name == "Button")
                    {
                        AddNewButton();
                        items[mIndexButton].key = lNodeList[i].SelectSingleNode("Key").InnerText;
                        items[mIndexButton].trigger = lNodeList[i].SelectSingleNode("Trigger").InnerText;
                        bool.TryParse(lNodeList[i].SelectSingleNode("QuitApp").InnerText, out items[mIndexButton].quitApp);
                        mIndexButton++;
                    }

                }
            }

        }

    }
}