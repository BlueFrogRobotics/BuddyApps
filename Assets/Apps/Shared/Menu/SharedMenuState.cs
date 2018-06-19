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

        /// <summary>
        /// Name of the grammar Vocon, without "_language"/extension
        /// </summary>
        [SerializeField]
        private string NameVoconGrammarFile;

        [SerializeField]
        private LoadContext context;

        private int mNumberOfButton;
        private int mIndexButton = 0;

        private string mSpeechReco;
        private string mStartRule;

        private bool mHasDisplayChoices;
        private bool mListening;
        private bool mHasLoadedTTS;

        private float mTimer = 0.0f;

        private bool mListClear;

        public override void Start()
        {
            Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Header.DisplayParametersButton = false;
            mListClear = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("StateENterShared");
            if(!mListClear)
            {
                mListClear = true;
                //items.Clear();
                //items = new List<MenuItem>(0);
            }
            BYOS.Instance.Header.DisplayParametersButton = false;
            BYOS.Instance.Primitive.TouchScreen.UnlockScreen();
            mHasLoadedTTS = true;
            if (!string.IsNullOrEmpty(speechKey))
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

            //Use vocon
            Interaction.VocalManager.UseVocon = true;

            Debug.Log(BYOS.Instance.Resources.GetPathToRaw(NameVoconGrammarFile + "_en.bin + " + context));
            Interaction.VocalManager.AddGrammar(NameVoconGrammarFile, context);
            Interaction.VocalManager.OnVoconBest = VoconBest;
            Interaction.VocalManager.OnVoconEvent = EventVocon;

            Interaction.VocalManager.EnableDefaultErrorHandling = false;
            Interaction.VocalManager.OnError = Empty;
            mTimer = 0.0f;
            mListening = false;
        }

        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(VoconResult iBestResult)
        {
            mSpeechReco = iBestResult.Utterance;
            Debug.Log("SpeechReco = " + mSpeechReco);
            mStartRule = iBestResult.StartRule;
            mListening = false;
            //Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mHasLoadedTTS)
            {
                mTimer += Time.deltaTime;

                if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                    return;

                if (!mHasDisplayChoices)
                {
                    DisplayChoices();
                    mHasDisplayChoices = true;
                    return;
                }
                Debug.Log("mSpeechReco = " + mSpeechReco + "lol");
                if (string.IsNullOrEmpty(mSpeechReco))
                {
                    Debug.Log("StartInstantReco");
                    Interaction.VocalManager.StartInstantReco();

                    Interaction.Mood.Set(MoodType.LISTENING);
                    mListening = true;
                    return;
                }

                mStartRule = SharedVocalFunctions.GetRealStartRule(mStartRule);

                foreach (MenuItem item in items)
                {
                    if (mStartRule.Equals(item.key))
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
            Debug.Log("StateExitShared");
            // Vocon
            Interaction.VocalManager.StopListenBehaviour(); 
            Interaction.VocalManager.RemoveGrammar(NameVoconGrammarFile, LoadContext.APP);
            Interaction.VocalManager.UseVocon = false;
            Interaction.VocalManager.OnVoconBest = null;
            Interaction.VocalManager.OnVoconEvent = null;
            
            mListClear = false;
            Interaction.SpeechToText.Stop();
            Interaction.Mood.Set(MoodType.NEUTRAL);
            mIndexButton = 0;
            items.Clear();
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

            if (string.IsNullOrEmpty(Dictionary.GetString(titleKey)))
                BYOS.Instance.Toaster.Display<ChoiceToast>().With(titleKey, lButtonsInfo);
            else
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