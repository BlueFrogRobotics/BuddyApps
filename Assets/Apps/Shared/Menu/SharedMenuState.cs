using UnityEngine;
using UnityEngine.UI;

using BlueQuark;

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
    public sealed class SharedMenuState : ASharedSMB
    {
        public sealed class MenuItem
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
        private Context context;


        [Header("BML")]
        [SerializeField]
        private bool PlayBML;
        [SerializeField]
        private bool RandomBML;
        [SerializeField]
        private string[] NameOfBML;

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
            Buddy.Vocal.EnableTrigger = false;
            Buddy.GUI.Header.DisplayParametersButton(false);

            mListClear = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("StateENterShared");
            if (!mListClear) {
                mListClear = true;
                //items.Clear();
                //items = new List<MenuItem>(0);
            }
            Buddy.GUI.Header.DisplayParametersButton(false);
            //BYOS.Instance.Primitive.TouchScreen.UnlockScreen();
            Buddy.GUI.Screen.Locked = false;
            mHasLoadedTTS = true;
            if (!string.IsNullOrEmpty(speechKey)) {
                if (!string.IsNullOrEmpty(Buddy.Resources.GetRandomString(speechKey))) {
                    Buddy.Vocal.Say(Buddy.Resources.GetRandomString(speechKey));
                } else if (!string.IsNullOrEmpty(Buddy.Resources.GetString(speechKey))) {
                    Buddy.Vocal.Say(Buddy.Resources.GetString(speechKey));
                }
            }

            //Use vocon
            //Interaction.VocalManager.UseVocon = true;

            //Debug.Log(BYOS.Instance.Resources.GetPathToRaw(NameVoconGrammarFile + "_en.bin + " + context));
            //Interaction.VocalManager.AddGrammar(NameVoconGrammarFile, context);
            //Interaction.VocalManager.OnVoconBest = VoconBest;
            //Interaction.VocalManager.OnVoconEvent = EventVocon;

            //Interaction.VocalManager.EnableDefaultErrorHandling = false;
            //Interaction.VocalManager.OnError = Empty;
            Buddy.Vocal.OnEndListening.Add((iInput) => { VoconBest(iInput); });
            Buddy.Vocal.OnListeningStatus.Add((iInput) => { EventVocon(iInput); });

            if(!string.IsNullOrEmpty(NameVoconGrammarFile))
                Buddy.Vocal.Listen(NameVoconGrammarFile, SpeechRecognitionMode.GRAMMAR_ONLY);
            mTimer = 0.0f;
            mListening = false;
        }

        private void EventVocon(SpeechInputStatus iEvent)
        {
            Debug.Log(iEvent);
            if (iEvent.IsError)
                Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }

        private void VoconBest(SpeechInput iBestResult)
        {
            mSpeechReco = iBestResult.Utterance;
            Debug.Log("SpeechReco = " + mSpeechReco);
            if (string.IsNullOrEmpty(mSpeechReco))
                mSayText = true;
            mStartRule = iBestResult.Rule;
            mListening = false;
            //Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mHasLoadedTTS) {
                mTimer += Time.deltaTime;

                if (Buddy.Vocal.IsSpeaking || mListening)
                    return;

                if (!mHasDisplayChoices) {
                    DisplayChoices();
                    mHasDisplayChoices = true;
                    return;
                }
                Debug.Log("mSpeechReco = " + mSpeechReco + "lol");
                if (string.IsNullOrEmpty(mSpeechReco)) {
                    if (mSayText) {
                        Debug.Log(mTimeout + " == " + Timeout);
                        if (mTimeout >= Timeout && Timeout != 0) {
                            QuitApp();
                            return;
                        }
                        Debug.Log("StartInstantReco");

                        if (!string.IsNullOrEmpty(speechKey)) {
                            if (!string.IsNullOrEmpty(Buddy.Resources.GetRandomString(speechKey))) {
                                Buddy.Vocal.Say(Buddy.Resources.GetRandomString(speechKey));
                            } else if (!string.IsNullOrEmpty(Buddy.Resources.GetString(speechKey))) {
                                Buddy.Vocal.Say(Buddy.Resources.GetString(speechKey));
                            }
                        }
                        mSayText = false;
                    }
                    if (!Buddy.Vocal.IsSpeaking) {
                        //Interaction.VocalManager.StartInstantReco();
                        Buddy.Vocal.Listen();
                        mTimeout++;
                        Buddy.Behaviour.SetMood(Mood.LISTENING);
                        mListening = true;
                        return;
                    }
                }

                mStartRule = SharedVocalFunctions.GetRealStartRule(mStartRule);

                int lNumber = 0;
                foreach (MenuItem item in items) {
                    if (mStartRule.Equals(item.key)) {
                        Buddy.GUI.Toaster.Hide();
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
            Buddy.Vocal.Stop();
            //Interaction.VocalManager.StopListenBehaviour();
            //Interaction.VocalManager.RemoveGrammar(NameVoconGrammarFile, context);
            //Interaction.VocalManager.UseVocon = false;
            //Interaction.VocalManager.OnVoconBest = null;
            //Interaction.VocalManager.OnVoconEvent = null;

            mListClear = false;

            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
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
            //Pas encore mis dans le new SDK, a voir quand antoine les aura remis
            //BYOS.Instance.Header.SpinningWheel = true;

            while (!Buddy.Vocal.IsSpeaking)
                yield return null;
            mHasLoadedTTS = true;
            //BYOS.Instance.Header.SpinningWheel = false;
        }

        /// <summary>
        /// Display the choice toaster
        /// </summary>
        private void DisplayChoices()
        {
            Debug.Log("display choice");

            FillMenu();
            //Debug.Log("display count " + items.Count);
            //ButtonInfo[] lButtonsInfo = new ButtonInfo[items.Count];
            //int i = 0;
            //foreach (MenuItem item in items)
            //{
            //    lButtonsInfo[i] = new ButtonInfo
            //    {
            //        Label = Buddy.Resources.GetString(item.key),
            //        OnClick = delegate () { StartCoroutine(GotoParameter(item.trigger, i, item.quitApp)); }
            //    };
            //    i++;
            //}
            Debug.Log("ITEMS COUNT SHARED :" + items.Count);
            List<int> mIndexHashList = new List<int>();
            if (string.IsNullOrEmpty(Buddy.Resources.GetString(titleKey))) {
                //BYOS.Instance.Toaster.Display<ChoiceToast>().With(titleKey, lButtonsInfo);
                Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {
                    for (int i = 0; i < items.Count; ++i) {
                        TVerticalListBox lBox = iBuilder.CreateBox();
                        int mHash = lBox.GetHashCode();
                        mIndexHashList.Add(mHash);
                        lBox.OnClick.Add(() => {
                            int mIndex = mIndexHashList.FindIndex(x => x == mHash);
                            StartCoroutine(GotoParameter(items[mIndex].trigger, mIndex, items[mIndex].quitApp));
                            Buddy.GUI.Toaster.Hide();
                        });
                        lBox.SetLabel(Buddy.Resources.GetString(items[i].key));
                    }
                });
            } else {
                //BYOS.Instance.Toaster.Display<ChoiceToast>().With(Buddy.Resources.GetString(titleKey), lButtonsInfo);
                Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {

                    for (int i = 0; i < items.Count; ++i) {
                        TVerticalListBox lBox = iBuilder.CreateBox();
                        int mHash = lBox.GetHashCode();
                        mIndexHashList.Add(mHash);
                        lBox.OnClick.Add(() => {
                            int mIndex = mIndexHashList.FindIndex(x => x == mHash);
                            StartCoroutine(GotoParameter(items[mIndex].trigger, mIndex, items[mIndex].quitApp));
                            Buddy.GUI.Toaster.Hide();
                        });
                        lBox.SetLabel(Buddy.Resources.GetString(items[i].key));
                    }
                });
            }

        }

        private void OnSpeechReco(string iVoiceInput)
        {
            mSpeechReco = iVoiceInput;

            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mListening = false;
        }

        /// <summary>
        /// Go to parameters
        /// </summary>
        /// <param name="iMode">the chosen mode</param>
        private IEnumerator GotoParameter(string iTrigger, int iNumber, bool iQuit)
        {
            if (PlayBML) {
                try {
                    BMLLauncher(NameOfBML[iNumber]);
                } catch (Exception e) {
                }


                while (Buddy.Behaviour.IsBusy)
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
            if (!BmlIsLaunch) {
                if (RandomBML) {
                    if (!Buddy.Behaviour.Interpreter.Run(iBMLName))
                        ExtLog.E(ExtLogModule.APP, GetType(),
                            LogStatus.FAILURE, LogInfo.NOT_FOUND,
                            "This category of BML doesn't exist in your app and in the OS");
                } else {
                    if (!Buddy.Behaviour.Interpreter.Run(iBMLName))
                        ExtLog.E(ExtLogModule.APP, GetType(),
                            LogStatus.FAILURE, LogInfo.NOT_FOUND, 
                            "This BML doesn't exist in your app and in the OS");
                }
                BmlIsLaunch = true;
            }
        }

        void AddNewButton()
        {
            items.Add(new MenuItem());
        }

        private void FillMenu()
        {
            string lPath = Buddy.Resources.GetRawFullPath("XMLShared/Menu");
            bool lResult = false;
            int lValue = 0;

            if (File.Exists(lPath + "/" + NameOfXML + ".xml")) {
                XmlDocument lDoc = new XmlDocument();
                lDoc.Load(lPath + "/" + NameOfXML + ".xml");

                XmlElement lElmt = lDoc.DocumentElement;
                XmlNodeList lNodeList = lElmt.ChildNodes;
                if (lNodeList[0].Name == "ListSize") {

                    lResult = int.TryParse(lNodeList[0].InnerText, out lValue);
                    if (lResult)
                        mNumberOfButton = lValue;
                }

                for (int i = 0; i < lNodeList.Count; ++i) {
                    if (lNodeList[i].Name == "Button") {
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