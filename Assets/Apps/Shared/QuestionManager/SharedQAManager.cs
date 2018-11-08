using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using System.Linq;
using System.IO;
using System.Xml;

namespace BuddyApp.Shared
{
    public sealed class SharedQAManager : ASharedSMB
    {
        //TODO : Add a timeout to all the question
        [SerializeField]
        private string NameOfXML;
        [SerializeField]
        private string BuddySays;
        [SerializeField]
        private string KeyQuestion;
        [SerializeField]
        private bool IsSoundForButton;
        [SerializeField]
        private SoundSample FxSound;
        //[SerializeField]
        //private float Timeout;
        [SerializeField]
        private string NameVoconGrammarFile;
        [SerializeField]
        private string BuddySaysWhenQuit;
        [SerializeField]
        private Context context;


        public sealed class QuestionItem
        {
            public string key;
            public string trigger;
        }
        private List<QuestionItem> items = new List<QuestionItem>(0);

        [Header("Binary Question Parameters: ")]
        [SerializeField]
        private bool IsBinaryQuestion;
        [SerializeField]
        private bool WantBinaryToasterDisplay;

        [Header("Multiple Question Parameters : ")]
        [SerializeField]
        private bool IsMultipleQuestion;
        [SerializeField]
        private bool WantMultipleQuestionToasterDisplay;


        [Header("BML")]
        [SerializeField]
        private bool PlayBML;
        [SerializeField]
        private bool RandomBML;
        [SerializeField]
        private string[] AnswerBML;
        [SerializeField]
        private string QuitBML;

        private bool mIsDisplayed;

        private string mSpeechReco;
        private string mStartRule;
        private bool mListening;
        private Vocal mTTS;
        private float mTimer;
        private Mood mActualMood;
        //private List<string> mKeyList;
        private bool mSoundPlayed;
        private int mNumberOfButton;
        private int mIndexButton = 0;
        private bool BmlIsLaunch = true;


        public override void Start()
        {
            Buddy.Vocal.EnableTrigger = false;
            Buddy.GUI.Header.DisplayParametersButton(false);
            mStartRule = String.Empty;

            mTTS = Buddy.Vocal;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("OnStateEnter " + IsBinaryQuestion);

            //Use vocon
            Buddy.Vocal.OnEndListening.Add((iInput) => { VoconBest(iInput); });
            Buddy.Vocal.OnListeningStatus.Add((iInput) => { EventVocon(iInput); });
            Buddy.Vocal.Listen(NameVoconGrammarFile, SpeechRecognitionMode.GRAMMAR_ONLY);
            //Interaction.VocalManager.AddGrammar(NameVoconGrammarFile, context);
            //Interaction.VocalManager.OnVoconBest = VoconBest;
            //Interaction.VocalManager.OnVoconEvent = EventVocon;

            //Interaction.VocalManager.EnableDefaultErrorHandling = false;
            //Interaction.VocalManager.OnError = null;

            mListening = false;
            mSoundPlayed = false;
            mIsDisplayed = false;
            //mKeyList = new List<string>();
            mTimer = 0F;
        }

        private void EventVocon(SpeechInputStatus iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(SpeechInput iBestResult)
        {
            mSpeechReco = iBestResult.Utterance;
            mStartRule = iBestResult.Rule;
            mListening = false;
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            FillMenu();
            mTimer += Time.deltaTime;
            Debug.Log(mNumberOfButton + " " + IsBinaryQuestion);
            if (mNumberOfButton > 2 && IsBinaryQuestion) {
                Debug.Log("---You checked Binary Question with more than 2 buttons---");
                ExtLog.E(ExtLogModule.APP, GetType(),
                         LogStatus.FAILURE, LogInfo.BAD_ARGUMENT,
                         "---You checked Binary Question with more than 2 buttons---");
                return;
            }
            if (mNumberOfButton < 3 && IsMultipleQuestion) {
                Debug.Log("---You checked Multiple Question with less than 3 buttons---");
                ExtLog.E(ExtLogModule.APP, GetType(),
                         LogStatus.FAILURE, LogInfo.BAD_ARGUMENT,
                         "---You checked Multiple Question with less than 3 buttons---");
                return;
            }

            if (IsBinaryQuestion) {
                if (!mIsDisplayed) {
                    if (!string.IsNullOrEmpty(BuddySays)) {
                        if (IsKey(BuddySays)) {
                            //Change this line when CORE did the correction about GetRandomString not working if the string is empty
                            if (!string.IsNullOrEmpty(Buddy.Resources.GetRandomString(BuddySays)))
                                mTTS.Say(Buddy.Resources.GetRandomString(BuddySays));
                            else
                                mTTS.Say(Buddy.Resources.GetString(BuddySays));
                        } else {
                            mTTS.Say(BuddySays);
                        }
                    }
                    //Display toaster
                    if (WantBinaryToasterDisplay) {
                        mActualMood = Buddy.Behaviour.Mood;
                        if (items.Count == 0) {
                            Debug.Log("items empty : not possible");
                        }
                        Debug.Log("BNARY QUESTION AVANT TOASTER");
                        DisplayQuestion();
                    }
                    mIsDisplayed = true;
                }

                if (Buddy.Vocal.IsSpeaking || mListening)
                    return;
                if (string.IsNullOrEmpty(mSpeechReco) && !mListening) {
                    Buddy.Vocal.Listen();
                    //Interaction.VocalManager.StartInstantReco();

                    mListening = true;
                    return;
                }

                mStartRule = SharedVocalFunctions.GetRealStartRule(mStartRule);
                int lNumberAnswer = 0;
                foreach (QuestionItem item in items) {
                    if (mStartRule.Equals(item.key)) {
                        if (WantBinaryToasterDisplay)
                            Buddy.GUI.Toaster.Hide();
                        if (item.trigger.Equals("quit"))
                            StartCoroutine(Quit());
                        else
                            StartCoroutine(GotoParameter(item.trigger, lNumberAnswer));
                        break;
                    } else if (mStartRule.Equals("quit")) {
                        if (WantMultipleQuestionToasterDisplay)
                            Buddy.GUI.Toaster.Hide();
                        StartCoroutine(Quit());
                    }
                    lNumberAnswer++;
                }
            } else if (IsMultipleQuestion) {
                if (WantMultipleQuestionToasterDisplay && !mIsDisplayed) {
                    mActualMood = Buddy.Behaviour.Mood;
                    mIsDisplayed = true;
                    if (!string.IsNullOrEmpty(BuddySays)) {
                        if (IsKey(BuddySays)) {
                            //Change this line when CORE did the correction about GetRandomString not working if the string is empty
                            if (!string.IsNullOrEmpty(Buddy.Resources.GetRandomString(BuddySays)))
                                mTTS.Say(Buddy.Resources.GetRandomString(BuddySays));
                            else
                                mTTS.Say(Buddy.Resources.GetString(BuddySays));
                        } else {
                            mTTS.Say(BuddySays);
                        }
                    }
                    DisplayQuestion();
                }

                if (Buddy.Vocal.IsSpeaking || mListening)
                    return;
                if (string.IsNullOrEmpty(mSpeechReco)) {
                    Buddy.Vocal.Listen();
                    mListening = true;
                    return;
                }

                mStartRule = SharedVocalFunctions.GetRealStartRule(mStartRule);

                int lNumberAnswer = 0;
                foreach (QuestionItem item in items) {
                    if (mStartRule.Equals(item.key)) {
                        if (WantMultipleQuestionToasterDisplay)
                            Buddy.GUI.Toaster.Hide();
                        if (item.trigger.Equals("quit"))
                            StartCoroutine(Quit());
                        else
                            StartCoroutine(GotoParameter(item.trigger, lNumberAnswer));
                        break;
                    } else if (mStartRule.Equals("quit")) {
                        if (WantMultipleQuestionToasterDisplay)
                            Buddy.GUI.Toaster.Hide();
                        StartCoroutine(Quit());
                    }
                    lNumberAnswer++;
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ON STATE EXIT QA MANAGER");
            Buddy.Vocal.Stop();
            //Interaction.VocalManager.RemoveGrammar(NameVoconGrammarFile, Context.APP);
            //Interaction.VocalManager.StopListenBehaviour();
            Buddy.Behaviour.SetMood(mActualMood);
            mIsDisplayed = false;
            //Interaction.VocalManager.UseVocon = false;
            Debug.Log("EXIT");
        }

        /// <summary>
        /// Go to parameters
        /// </summary>
        /// <param name="iMode">the chosen mode</param>
        private IEnumerator GotoParameter(string iTrigger, int iNumberAnswer)
        {
            if (PlayBML) {
                try {
                    BMLLauncher(AnswerBML[iNumberAnswer]);
                } catch (Exception e) {
                    ExtLog.E(ExtLogModule.APP, GetType(),
                         LogStatus.FAILURE, LogInfo.BAD_ARGUMENT,
                         e.Message + " You need to have the same number of button and AnswerBML in Shared Inspector");
                    QuitApp();
                }
                while (Buddy.Behaviour.IsBusy)
                    yield return null;

                BmlIsLaunch = true;
            }
            mSpeechReco = null;
            Trigger(iTrigger);
            //Interaction.SpeechToText.Stop();
            Buddy.Vocal.StopListening();
        }

        /// <summary>
        /// Quit application
        /// </summary>
        /// <returns></returns>
        private IEnumerator Quit()
        {
            if (!string.IsNullOrEmpty(BuddySaysWhenQuit)) {
                if (IsKey(BuddySaysWhenQuit)) {
                    Debug.Log(BuddySaysWhenQuit);
                    //Change this line when CORE did the correction about GetRandomString not working if the string is empty
                    if (!string.IsNullOrEmpty(Buddy.Resources.GetRandomString(BuddySaysWhenQuit)))
                        mTTS.Say(Buddy.Resources.GetRandomString(BuddySaysWhenQuit));
                    else
                        mTTS.Say(Buddy.Resources.GetString(BuddySaysWhenQuit));
                } else {
                    mTTS.Say(BuddySaysWhenQuit);
                }
            }

            try {
                BMLLauncher(QuitBML);
            } catch (Exception e) {
                ExtLog.E(ExtLogModule.APP, GetType(),
                         LogStatus.FAILURE, LogInfo.BAD_ARGUMENT,
                         e.Message + " You need to have the same number of button and AnswerBML in Editor");
                QuitApp();
            }

            while (Buddy.Behaviour.IsBusy && Buddy.Vocal.IsSpeaking)
                yield return null;
            BmlIsLaunch = true;
            QuitApp();
        }

        /// <summary>
        /// Launch Random or By name BML
        /// </summary>
        /// <param name="iBMLName"></param>
        private void BMLLauncher(string iBMLName)
        {
            if (BmlIsLaunch) {
                if (RandomBML) {
                    if (!Buddy.Behaviour.Interpreter.RunRandom(iBMLName))
                        ExtLog.E(ExtLogModule.APP, GetType(),
                         LogStatus.FAILURE, LogInfo.NOT_FOUND,
                         "This category of BML doesn't exist in your app and in the OS");
                } else {
                    if (!Buddy.Behaviour.Interpreter.RunRandom(iBMLName))
                        ExtLog.E(ExtLogModule.APP, GetType(),
                         LogStatus.FAILURE, LogInfo.NOT_FOUND, 
                         "This BML doesn't exist in your app and in the OS");
                }
                BmlIsLaunch = false;
            }
        }

        private void PressedButton(string iKey)
        {
            Debug.Log("IKEY : " + iKey);
            Buddy.GUI.Toaster.Hide();
            if (IsSoundForButton && !mSoundPlayed) {
                if (FxSound == SoundSample.NONE)
                    FxSound = SoundSample.BEEP_1;
                Buddy.Actuators.Speakers.Media.Play(FxSound);
                mSoundPlayed = true;
            }
            Trigger(iKey);
        }

        private void OnSpeechReco(string iVoiceInput)
        {
            mSpeechReco = iVoiceInput;
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            mListening = false;
        }


        private bool IsKey(string iKey)
        {
            if (iKey.Where(char.IsUpper).Any() || SharedVocalFunctions.ContainsWhiteSpace(iKey) || SharedVocalFunctions.ContainsSpecialChar(iKey))
                return false;
            else return true;
        }

        private void DisplayQuestion()
        {
            Debug.Log("DISPLAY QUESTION");
            string lTitle;
            if (!string.IsNullOrEmpty(KeyQuestion)) {
                if (IsKey(KeyQuestion))
                    lTitle = Buddy.Resources.GetString(KeyQuestion);
                else
                    lTitle = KeyQuestion;

                if (WantBinaryToasterDisplay) {
                    for (int i = 0; i < items.Count; ++i) {
                        if (string.IsNullOrEmpty(Buddy.Resources.GetString(items[i].key))) {
                            Debug.Log("---Your key doesn't exist in the dictionnary---");
                            ExtLog.E(ExtLogModule.APP, GetType(),
                                LogStatus.FAILURE, LogInfo.NOT_FOUND,
                                "---Your key doesn't exist in the dictionnary---");
                            return;
                        }
                    }
                    //BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With(lTitle, lButtonsInfo[1], lButtonsInfo[0]);
                    Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => iBuilder.CreateWidget<TText>().SetLabel(lTitle),
                        () => { PressedButton(items[0].trigger); }, Buddy.Resources.GetString(items[0].key),
                        () => { PressedButton(items[1].trigger); }, Buddy.Resources.GetString(items[1].key));
                } else if (WantMultipleQuestionToasterDisplay) {
                    //BYOS.Instance.Toaster.Display<ChoiceToast>().With(lTitle, lButtonsInfo);
                    //A VOIR SI JE LE FAIS APRES (pas utilisé pour le moment)
                    //Buddy.GUI.Toaster.Display<VerticalListToast>().With();
                }
            }
        }

        private void AddNewButton()
        {
            items.Add(new QuestionItem());
        }

        private void FillMenu()
        {
            string lPath = Buddy.Resources.GetRawFullPath("XMLShared/Question");
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
                        mIndexButton++;
                    }
                }
            }
        }
    }
}
