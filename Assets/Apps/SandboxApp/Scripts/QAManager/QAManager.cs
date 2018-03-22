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
      
        [Header("Binary Question Parameters: ")]
        [SerializeField]
        private bool IsBinaryQuestion;
        [SerializeField]
        private bool IsBinaryToaster;
        [SerializeField]
        private string BuddySays;
        [SerializeField]
        private string LeftButton;    
        [SerializeField]
        private string RightButton;
        [SerializeField]
        private string TriggerLeftButton;
        [SerializeField]
        private string TriggerRightButton;
        [SerializeField]
        private string KeyQuestion;
        [SerializeField]
        private bool IsSoundForButton;
        [SerializeField]
        private FXSound FxSound;
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

        public override void Start()
        {
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
                        if (!VocalFunctions.ContainsWhiteSpace(BuddySays))
                            mTTS.Say(Dictionary.GetRandomString(BuddySays));
                        else
                            mTTS.Say(BuddySays);

                    bool lResult = KeyQuestion.Where(char.IsUpper).Any();
                    if (string.IsNullOrEmpty(RightButton))
                        RightButton = Dictionary.GetString("yes");
                    if (string.IsNullOrEmpty(LeftButton))
                        LeftButton = Dictionary.GetString("no");
                    Debug.Log("1");
                    if (!VocalFunctions.ContainsWhiteSpace(KeyQuestion) && !lResult && !VocalFunctions.ContainsSpecialChar(KeyQuestion))
                    {
                        KeyQuestion = Dictionary.GetString(KeyQuestion);
                    }
                    mButtonRight = new ButtonInfo
                    {   
                        Label = Dictionary.GetString(RightButton),
                        OnClick = delegate() { PressedButton(TriggerRightButton); }
                    };
                    mButtonLeft = new ButtonInfo
                    {  
                        Label = Dictionary.GetString(LeftButton), 
                        OnClick = delegate () { PressedButton(TriggerLeftButton); }
                    };
                    BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With(KeyQuestion, mButtonRight, mButtonLeft);
                    mKeyList.Add(mButtonLeft.Label);
                    mKeyList.Add(mButtonRight.Label);
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
                for (int i = 0; i < mKeyList.Count; ++i)
                {
                    if (VocalFunctions.ContainsOneOf(mSpeechReco,new List<string>( Dictionary.GetPhoneticStrings(mKeyList[i]))))
                    {
                        if (i == 0)
                            PressedButton(TriggerLeftButton);
                        else
                            PressedButton(TriggerRightButton);
                    }
                }
 

            }
            else if (IsMultipleQuestion && !mIsDisplayed)
            {
                mIsDisplayed = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        } 

        private void PressedButton(string iKey)
        {
            Toaster.Hide();
            if (IsSoundForButton)
            {
                if (FxSound == FXSound.NONE)
                    FxSound = FXSound.BEEP_1;
                Primitive.Speaker.FX.Play(FxSound);
            }
            Trigger(iKey);
        }

        private void OnSpeechReco(string iVoiceInput)
        {
            mSpeechReco = iVoiceInput;

            Interaction.Mood.Set(MoodType.NEUTRAL);
            mListening = false;
        }
        
        private void ActivateTrigger(string iTrigger)
        {
            mSpeechReco = null;
            Trigger(iTrigger);
            Interaction.VocalManager.OnEndReco = null;
        }

    }

}
