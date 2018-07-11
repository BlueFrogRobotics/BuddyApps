using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System;
using System.Linq;
using System.IO;
using System.Xml;

namespace BuddyApp.Shared
{
    public class SharedQAManager : ASharedSMB
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
        private FXSound FxSound;
        [SerializeField]
        private float Timeout;
        [SerializeField]
        private string NameVoconGrammarFile;
        [SerializeField]
        private string BuddySayWhenQuit;
        [SerializeField]
        private LoadContext context;

        public class QuestionItem
        {
            public string key;
            public string trigger;
        }
        private List<QuestionItem> items = new List<QuestionItem>(0);

        [Header("Binary Question Parameters: ")]
        [SerializeField]
        private bool IsBinaryQuestion;
        [SerializeField]
        private bool IsBinaryToaster;

        [Header("Multiple Question Parameters : ")]
        [SerializeField]
        private bool IsMultipleQuestion;
        [SerializeField]
        private bool IsMultipleToaster;


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
        private TextToSpeech mTTS;
        private float mTimer;
        private MoodType mActualMood;
        private List<string> mKeyList;
        private bool mSoundPlayed;
        private int mNumberOfButton;
        private int mIndexButton = 0;
        private bool BmlIsLaunch = true;


        public override void Start()
        {
            Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Header.DisplayParametersButton = false;
            mStartRule = String.Empty;

            mTTS = Interaction.TextToSpeech;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("OnStateEnter " + IsBinaryQuestion);

            //Use vocon
            Interaction.VocalManager.UseVocon = true;
            Interaction.VocalManager.AddGrammar(NameVoconGrammarFile, context);
            Interaction.VocalManager.OnVoconBest = VoconBest;
            Interaction.VocalManager.OnVoconEvent = EventVocon;

            Interaction.VocalManager.EnableDefaultErrorHandling = false;
            Interaction.VocalManager.OnError = null;

            mListening = false;
            mSoundPlayed = false;
            mIsDisplayed = false;
            mKeyList = new List<string>();
            mTimer = 0F;
        }

        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(VoconResult iBestResult)
        {
            mSpeechReco = iBestResult.Utterance;
            mStartRule = iBestResult.StartRule;
            mListening = false;
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            FillMenu();
            mTimer += Time.deltaTime;

            if (mNumberOfButton > 3 && IsBinaryQuestion)
            {
                Debug.Log("---You checked Binary Question with more than 2 buttons---");
                return;
            }
            if (mNumberOfButton < 3 && IsMultipleQuestion)
            {
                Debug.Log("---You checked Multiple Question with less than 3 buttons---");
                return;
            }

            if (IsBinaryQuestion)
            {
                if (!mIsDisplayed)
                {
                    if (!string.IsNullOrEmpty(BuddySays))
                    {
                        if (IsKey(BuddySays))
                        {
                            Debug.Log(BuddySays);
                            //Change this line when CORE did the correction about GetRandomString not working if the string is empty
                            if (!string.IsNullOrEmpty(Dictionary.GetRandomString(BuddySays)))
                                mTTS.Say(Dictionary.GetRandomString(BuddySays));
                            else
                                mTTS.Say(Dictionary.GetString(BuddySays));
                        }
                        else
                        {
                            mTTS.Say(BuddySays);
                        }
                    }
                    //Display toaster
                    if (IsBinaryToaster)
                    {
                        mActualMood = Interaction.Mood.CurrentMood;
                        if (items.Count == 0)
                        {
                            Debug.Log("items empty : not possible");
                        }
                        DisplayQuestion();
                    }
                    mIsDisplayed = true;
                }

                if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                    return;
                if (string.IsNullOrEmpty(mSpeechReco) && !mListening)
                {
                    Interaction.VocalManager.StartInstantReco();

                    mListening = true;
                    return;
                }

                mStartRule = SharedVocalFunctions.GetRealStartRule(mStartRule);
                int lNumberAnswer = 0;
                foreach (QuestionItem item in items)
                {
                    if (mStartRule.Equals(item.key))
                    {
                        if (IsBinaryToaster)
                            BYOS.Instance.Toaster.Hide();
                        if (item.trigger.Equals("quit"))
                            StartCoroutine(Quit());
                        else
                            StartCoroutine(GotoParameter(item.trigger, lNumberAnswer));
                        break;
                    }
                    else if (mStartRule.Equals("quit"))
                    {
                        if (IsMultipleToaster)
                            BYOS.Instance.Toaster.Hide();
                        StartCoroutine(Quit());
                    }
                    lNumberAnswer++;
                }
            }
            else if (IsMultipleQuestion)
            {
                if (IsMultipleToaster && !mIsDisplayed)
                {
                    mActualMood = Interaction.Mood.CurrentMood;
                    mIsDisplayed = true;
                    if (!string.IsNullOrEmpty(BuddySays))
                    {
                        if (IsKey(BuddySays))
                        {
                            //Change this line when CORE did the correction about GetRandomString not working if the string is empty
                            if (!string.IsNullOrEmpty(Dictionary.GetRandomString(BuddySays)))
                                mTTS.Say(Dictionary.GetRandomString(BuddySays));
                            else
                                mTTS.Say(Dictionary.GetString(BuddySays));
                        }
                        else
                        {
                            mTTS.Say(BuddySays);
                        }
                    }
                    DisplayQuestion();
                }

                if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                    return;
                if (string.IsNullOrEmpty(mSpeechReco))
                {
                    Interaction.VocalManager.StartInstantReco();
                    mListening = true;
                    return;
                }

                mStartRule = SharedVocalFunctions.GetRealStartRule(mStartRule);

                int lNumberAnswer = 0;
                foreach (QuestionItem item in items)
                {
                    if (mStartRule.Equals(item.key))
                    {
                        if (IsMultipleToaster)
                            BYOS.Instance.Toaster.Hide();
                        if (item.trigger.Equals("quit"))
                            StartCoroutine(Quit());
                        else
                            StartCoroutine(GotoParameter(item.trigger, lNumberAnswer));
                        break;
                    }
                    else if (mStartRule.Equals("quit"))
                    {
                        if (IsMultipleToaster)
                            BYOS.Instance.Toaster.Hide();
                        StartCoroutine(Quit());
                    }
                    lNumberAnswer++;
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.VocalManager.RemoveGrammar(NameVoconGrammarFile, LoadContext.APP);
            Interaction.VocalManager.StopListenBehaviour();
            Interaction.Mood.Set(mActualMood);
            mIsDisplayed = false;
            Interaction.VocalManager.UseVocon = false;
            Debug.Log("EXIT");
        }

        /// <summary>
        /// Go to parameters
        /// </summary>
        /// <param name="iMode">the chosen mode</param>
        private IEnumerator GotoParameter(string iTrigger, int iNumberAnswer)
        {
            if (PlayBML)
            {
                try {
                    BMLLauncher(AnswerBML[iNumberAnswer]);
                }
                catch (Exception e) {
                    Utils.LogE(LogContext.APP, e.Message + " You need to have the same number of button and AnswerBML in Shared Inspector");
                    QuitApp();
                }
                while (!Interaction.BMLManager.DonePlaying)
                    yield return null;

                BmlIsLaunch = true;
            }
            mSpeechReco = null;
            Trigger(iTrigger);
            Interaction.SpeechToText.Stop();
        }

        /// <summary>
        /// Quit application
        /// </summary>
        /// <returns></returns>
        private IEnumerator Quit()
        {
            if (!string.IsNullOrEmpty(BuddySayWhenQuit))
            {
                if (IsKey(BuddySayWhenQuit))
                {
                    Debug.Log(BuddySayWhenQuit);
                    //Change this line when CORE did the correction about GetRandomString not working if the string is empty
                    if (!string.IsNullOrEmpty(Dictionary.GetRandomString(BuddySayWhenQuit)))
                        mTTS.Say(Dictionary.GetRandomString(BuddySayWhenQuit));
                    else
                        mTTS.Say(Dictionary.GetString(BuddySayWhenQuit));
                }
                else
                {
                    mTTS.Say(BuddySayWhenQuit);
                }
            }

            try {
                BMLLauncher(QuitBML);
            }
            catch (Exception e) {
                Utils.LogE(LogContext.APP, e.Message + " You need to have the same number of button and AnswerBML in Editor");
                QuitApp();
            }

            while (!Interaction.BMLManager.DonePlaying && !Interaction.TextToSpeech.HasFinishedTalking)
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
            if (BmlIsLaunch)
            {
                if (RandomBML) {
                    if (!Interaction.BMLManager.LaunchRandom(iBMLName))
                        Utils.LogE(LogContext.APP, "This category of BML doesn't exist in your app and in the OS");
                }
                else {
                    if (!Interaction.BMLManager.LaunchByName(iBMLName))
                        Utils.LogE(LogContext.APP, "This BML doesn't exist in your app and in the OS");
                }
                BmlIsLaunch = false;
            }
        }

        private void PressedButton(string iKey)
        {
            Debug.Log("IKEY : " + iKey);
            Toaster.Hide();
            if (IsSoundForButton && !mSoundPlayed)
            {
                if (FxSound == FXSound.NONE)
                    FxSound = FXSound.BEEP_1;
                Primitive.Speaker.FX.Play(FxSound);
                mSoundPlayed = true;
            }
            Trigger(iKey);
        }

        private void OnSpeechReco(string iVoiceInput)
        {
            mSpeechReco = iVoiceInput;
            Interaction.Mood.Set(MoodType.NEUTRAL);
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
            ButtonInfo[] lButtonsInfo = new ButtonInfo[this.items.Count];
            Debug.Log("Item count = " + items.Count);
            int i = 0;

            foreach (QuestionItem item in items)
            {
                lButtonsInfo[i] = new ButtonInfo
                {
                    Label = Dictionary.GetString(item.key),
                    OnClick = delegate () { PressedButton(item.trigger); }
                };

                i++;
            }

            string lTitle;
            if (!string.IsNullOrEmpty(KeyQuestion))
            {
                if (IsKey(KeyQuestion))
                    lTitle = Dictionary.GetString(KeyQuestion);
                else
                    lTitle = KeyQuestion;

                if (IsBinaryToaster)
                    BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With(lTitle, lButtonsInfo[1], lButtonsInfo[0]);
                else if (IsMultipleToaster)
                {
                    BYOS.Instance.Toaster.Display<ChoiceToast>().With(lTitle, lButtonsInfo);
                }
            }
        }

        private void AddNewButton()
        {
            items.Add(new QuestionItem());
        }

        private void FillMenu()
        {
            string lPath = BYOS.Instance.Resources.GetPathToRaw("XMLShared/Question");
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
                        mIndexButton++;
                    }
                }
            }
        }
    }
}
