using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System;
using System.Linq;

namespace BuddyApp.SandboxApp
{
    public class QAManager : AStateMachineBehaviour
    {
        [SerializeField]
        private string BuddySays;
        [SerializeField]
        private string KeyQuestion;
        [SerializeField]
        private bool IsSoundForButton;
        [SerializeField]
        private FXSound FxSound;
        [Serializable]
        public class QuestionItem
        {
            public string key;
            public string trigger;
        }
    
        [Header("Buttons of the Multiple question ")]
        [SerializeField]
        private List<QuestionItem> items;

        [Header("Binary Question Parameters: ")]
        [SerializeField]
        private bool IsBinaryQuestion;
        [SerializeField]
        private bool IsBinaryToaster;
        //[SerializeField]
        //private string LeftButton;    
        //[SerializeField]
        //private string RightButton;
        //[SerializeField]
        //private string TriggerLeftButton;
        //[SerializeField]
        //private string TriggerRightButton;

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

        public override void Start()
        {
            //items = new List<QuestionItem>();
            Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Header.DisplayParametersButton = false;
            mTTS = Interaction.TextToSpeech;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.VocalManager.OnEndReco = OnSpeechReco;
            Interaction.VocalManager.EnableDefaultErrorHandling = false;
            Interaction.VocalManager.OnError = null;
            
            mListening = false;
            mSoundPlayed = false;
            mKeyList = new List<string>();
            mTimer = 0F;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
            mTimer += Time.deltaTime;
            if(IsBinaryQuestion )
            {

                //Display toaster
                if (IsBinaryToaster && !mIsDisplayed)
                {
                    mActualMood = Interaction.Mood.CurrentMood;
                    mIsDisplayed = true;
                    if (!string.IsNullOrEmpty(BuddySays))
                        if (IsKey(BuddySays))
                            mTTS.Say(Dictionary.GetRandomString(BuddySays));
                        else
                            mTTS.Say(BuddySays);

                    //if (string.IsNullOrEmpty(RightButton))
                    //    RightButton = Dictionary.GetString("yes");
                    //if (string.IsNullOrEmpty(LeftButton))
                    //    LeftButton = Dictionary.GetString("no");

                    //Debug.Log("ITEM COUNT : " + items.Count);
                    //if (IsKey(KeyQuestion))
                    //{
                    //    KeyQuestion = Dictionary.GetString(KeyQuestion);
                    //}
                    //mButtonRight = new ButtonInfo
                    //{
                    //    Label = Dictionary.GetString(RightButton),
                    //    OnClick = delegate () { PressedButton(TriggerRightButton); }
                    //};
                    //mButtonLeft = new ButtonInfo
                    //{
                    //    Label = Dictionary.GetString(LeftButton),
                    //    OnClick = delegate () { PressedButton(TriggerLeftButton); }
                    //};
                    //Debug.Log("RIGHT BUTTON  : " + Dictionary.GetString(RightButton) + " LEFT BUTTON : " + Dictionary.GetString(LeftButton));
                    //Debug.Log("M RIGHT BUTTON  : " + mButtonRight.Label + " M LEFT BUTTON : " + mButtonLeft.Label);
                    //BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With(KeyQuestion, mButtonRight, mButtonLeft);
                    if (items.Count == 0)
                        Debug.Log("pas possible");
                    DisplayQuestion();
                    //mKeyList.Add(mButtonLeft.Label.ToLower());
                    //mKeyList.Add(mButtonRight.Label.ToLower());

                }

                Debug.Log("2");
                //Vocal
                if (mTimer > 6F)
                {
                    Interaction.Mood.Set(mActualMood);
                    mListening = false;
                    mTimer = 0F;
                    mSpeechReco = null;
                }
                Debug.Log("APRES 2  M SPEECH RECO: " + mSpeechReco);
                if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                    return;
                Debug.Log("3");
                if (string.IsNullOrEmpty(mSpeechReco))
                {
                    Interaction.VocalManager.StartInstantReco();
                    Interaction.Mood.Set(MoodType.LISTENING);
                    mListening = true;
                    return;
                }
                Debug.Log("4");
                //List<string> lol = new List<string>(Dictionary.GetPhoneticStrings(mKeyList[0]));
                //List<string> lol2 = new List<string>(Dictionary.GetPhoneticStrings(mKeyList[1]));

                //if (lol == null)
                //    Debug.Log("null");
                //if (lol.Count == 0)
                //    Debug.Log("count 0");
                //for (int i = 0; i < lol.Count - 1; ++i)
                //{
                //    Debug.Log("NTM1");
                //    Debug.Log(" STRING 1 : " + lol[i]);
                //}

                //for (int i = 0; i < lol2.Count - 1; ++i)
                //{
                //    Debug.Log("NTM2");
                //    Debug.Log(" STRING 2 : " + lol2[i]); 
                //}
                //for (int i = 0; i < mKeyList.Count; ++i)
                //{
                //    if (VocalFunctions.ContainsOneOf(mSpeechReco,new List<string>( Dictionary.GetPhoneticStrings(mKeyList[i]))))
                //    {
                //        if (i == 0)
                //            PressedButton(TriggerLeftButton);
                //        else
                //            PressedButton(TriggerRightButton);
                //    }
                //}
 

            }
            else if (IsMultipleQuestion && !mIsDisplayed)
            {
                mIsDisplayed = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Mood.Set(mActualMood);
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
            if (iKey.Where(char.IsUpper).Any() || VocalFunctions.ContainsWhiteSpace(iKey)  || VocalFunctions.ContainsSpecialChar(iKey))
                return false;
            else return true;
        }

        private void DisplayQuestion()
        {
            ButtonInfo[] lButtonsInfo = new ButtonInfo[this.items.Count];
            Debug.Log("Item count = " + items.Count);
            int i = 0;

            foreach (QuestionItem item in items)
            {
                Debug.Log("Index i = " + i);
                lButtonsInfo[i] = new ButtonInfo
                {
                    Label = Dictionary.GetString(item.key),
                    OnClick = delegate () { PressedButton(item.trigger); }
                };

                i++;
            }
            
            BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With(KeyQuestion, lButtonsInfo[0], lButtonsInfo[1]);
            //    //BYOS.Instance.Toaster.Display<ChoiceToast>().With(Dictionary.GetString(KeyQuestion), lButtonsInfo);

        }


    }

}
