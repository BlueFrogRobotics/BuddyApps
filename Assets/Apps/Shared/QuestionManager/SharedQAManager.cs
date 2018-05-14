﻿using System.Collections;
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
        /// TODO : avoir un paramètre pour afficher un toast pour la photo enregistré dans TakePhoto

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
       
        private bool mIsDisplayed;
        ButtonInfo mButtonLeft;
        ButtonInfo mButtonRight;

        private string mSpeechReco;
        private bool mListening;
        private TextToSpeech mTTS;
        private float mTimer;
        private MoodType mActualMood;
        private List<string> mKeyList;
        private bool mSoundPlayed;
        private int mNumberOfButton;
        private int mIndexButton = 0;

        public override void Start()
        {
            Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Header.DisplayParametersButton = false;
            mTTS = Interaction.TextToSpeech;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //Debug.Log("0.1");
            //Interaction lol;
            //Debug.Log("0.2");
            //VocalManager lol2;
            Debug.Log("1");
            Interaction.VocalManager.OnEndReco = OnSpeechReco;
            Debug.Log("2");
            Interaction.VocalManager.EnableDefaultErrorHandling = false;
            Debug.Log("3");
            Interaction.VocalManager.OnError = null;
            Debug.Log("4");
            mListening = false;
            Debug.Log("5");
            mSoundPlayed = false;
            Debug.Log("6");
            mKeyList = new List<string>();
            Debug.Log("7");
            mTimer = 0F;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("avant fill Menu");
            FillMenu();
            Debug.Log("apres fill menu");
            mTimer += Time.deltaTime;

            if(mNumberOfButton > 3 && IsBinaryQuestion)
            {
                Debug.Log("---You checked Binary Question with more than 2 buttons---");
                return;
            }
            if(mNumberOfButton < 3 && IsMultipleQuestion)
            {
                Debug.Log("---You checked Multiple Question with less than 3 buttons---");
                return;
            }

            if(IsBinaryQuestion)
            {
                //Display toaster
                if (IsBinaryToaster && !mIsDisplayed)
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
                    if (items.Count == 0)
                    {
                        Debug.Log("items empty : not possible");
                    }  
                    DisplayQuestion();
                }
                
                //Vocal
                if (mTimer > 6F)
                {
                    Interaction.Mood.Set(mActualMood);
                    mListening = false;
                    mTimer = 0F;
                    mSpeechReco = null;
                }

                if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                    return;
                if (string.IsNullOrEmpty(mSpeechReco))
                {
                    Interaction.VocalManager.StartInstantReco();
                    Interaction.Mood.Set(MoodType.LISTENING);
                    mListening = true;
                    return;
                }
                foreach (QuestionItem item in items)
                {
                    if (SharedVocalFunctions.ContainsOneOf(mSpeechReco, new List<string>(Dictionary.GetPhoneticStrings(item.key))))
                    {
                        BYOS.Instance.Toaster.Hide();
                        GotoParameter(item.trigger);
                        break;
                    }
                }
            }
            else if (IsMultipleQuestion )
            {
                if(IsMultipleToaster && !mIsDisplayed)
                {
                    mActualMood = Interaction.Mood.CurrentMood;
                    mIsDisplayed = true;
                    if (!string.IsNullOrEmpty(BuddySays))
                    {
                        if (IsKey(BuddySays))
                        {
                            //Change this line when CORE did the correction about GetRandomString not working if the string is empty
                            if(!string.IsNullOrEmpty( Dictionary.GetRandomString(BuddySays)))
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

                if (mTimer > 6F)
                {
                    Interaction.Mood.Set(mActualMood);
                    mListening = false;
                    mTimer = 0F;
                    mSpeechReco = null;
                }
                
                if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                    return;
                if (string.IsNullOrEmpty(mSpeechReco))
                {
                    Interaction.VocalManager.StartInstantReco();
                    Interaction.Mood.Set(MoodType.LISTENING);
                    mListening = true;
                    return;
                }

                foreach (QuestionItem item in items)
                {
                    if (SharedVocalFunctions.ContainsOneOf(mSpeechReco, new List<string>(Dictionary.GetPhoneticStrings(item.key))))
                    {
                        BYOS.Instance.Toaster.Hide();
                        GotoParameter(item.trigger);
                        break;
                    }
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Mood.Set(mActualMood);
        }

        /// <summary>
        /// Go to parameters
        /// </summary>
        /// <param name="iMode">the chosen mode</param>
        private void GotoParameter(string iTrigger)
        {
            mSpeechReco = null;
            Trigger(iTrigger);
            Interaction.SpeechToText.Stop();
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
            if (iKey.Where(char.IsUpper).Any() || SharedVocalFunctions.ContainsWhiteSpace(iKey)  || SharedVocalFunctions.ContainsSpecialChar(iKey))
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
            else
                Debug.Log("Problem File doesnt exist");
        }  
    } 
}