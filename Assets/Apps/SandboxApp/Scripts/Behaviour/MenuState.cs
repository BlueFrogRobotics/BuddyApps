﻿using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BuddyApp.SandboxApp
{
    /// <summary>
    /// State where we ask the user to choose between the different monitoring modes
    /// </summary>
    public class MenuState : AStateMachineBehaviour
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

        [SerializeField]
        private int Timeout = 3;

        /// <summary>
        /// Name of the grammar Vocon, without "_language"/extension
        /// </summary>
        [SerializeField]
        private string NameVoconGrammarFile;

        [SerializeField]
        private LoadContext context;


        [Header("BML")]
        [SerializeField]
        private bool PlayBML;
        [SerializeField]
        private bool RandomBML;
        [SerializeField]
        private string[] BMLs;

        private int mNumberOfButton;
        private int mIndexButton = 0;

        private string mSpeechReco;
        private string mStartRule;

        private bool mHasDisplayChoices;
        private bool mListening;
        private bool mHasLoadedTTS;
        private float mTimer = 0.0f;
        private int mTimeout;
        private bool mSayText;

        private bool mListClear;
        private bool BmlIsLaunch = false;

        public override void Start()
        {
            Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Header.DisplayParametersButton = false;
            mListClear = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("StateENterShared");
            if (!mListClear)
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
            if (string.IsNullOrEmpty(mSpeechReco))
                mSayText = true;
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
                    if (mSayText)
                    {
                        Debug.Log(mTimeout + " == " + Timeout);
                        if (mTimeout >= Timeout && Timeout != 0)
                        {
                            QuitApp();
                            return;
                        }
                        Debug.Log("StartInstantReco");

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
                        mSayText = false;
                    }
                    if (Interaction.TextToSpeech.HasFinishedTalking)
                    {
                        Interaction.VocalManager.StartInstantReco();
                        mTimeout++;
                        Interaction.Mood.Set(MoodType.LISTENING);
                        mListening = true;
                        return;
                    }
                }

                mStartRule = VocalFunctions.GetRealStartRule(mStartRule);

                int lNumber = 0;
                foreach (MenuItem item in items)
                {
                    if (mStartRule.Equals(item.key))
                    {
                        BYOS.Instance.Toaster.Hide();
                        StartCoroutine(GotoParameter(item.trigger, lNumber, item.quitApp));
                        break;
                    }
                    lNumber++;
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("StateExitShared");
            // Vocon
            Interaction.VocalManager.StopListenBehaviour();
            Interaction.VocalManager.RemoveGrammar(NameVoconGrammarFile, context);
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
            mTimeout = 0;
            mSayText = true;

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
                    OnClick = delegate () { StartCoroutine(GotoParameter(item.trigger, i, item.quitApp)); }
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
        private IEnumerator GotoParameter(string iTrigger, int iNumber, bool iQuit)
        {
            if (PlayBML)
            {
                try
                {
                    BMLLauncher(BMLs[iNumber]);
                }
                catch (Exception e)
                {
                    Utils.LogE(LogContext.APP, e.Message + " You need to have the same number of button and BMLs in Shared Inspector");
                    QuitApp();
                }


                while (!Interaction.BMLManager.DonePlaying)
                    yield return null;
            }
            BmlIsLaunch = false;
            mSpeechReco = null;
            if (iQuit)
                QuitApp();
            Trigger(iTrigger);
        }

        private void BMLLauncher(string iBMLName)
        {
            if (!BmlIsLaunch)
            {
                if (RandomBML)
                {
                    if (!Interaction.BMLManager.LaunchRandom(iBMLName))
                        Utils.LogE(LogContext.APP, "This category of BML doesn't exist in your app and in the OS");
                }
                else
                {
                    if (!Interaction.BMLManager.LaunchByName(iBMLName))
                        Utils.LogE(LogContext.APP, "This BML doesn't exist in your app and in the OS");
                }
                BmlIsLaunch = true;
            }
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