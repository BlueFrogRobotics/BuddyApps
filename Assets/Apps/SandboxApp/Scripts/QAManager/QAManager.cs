using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System;

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
            Interaction.VocalManager.OnError = Empty;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(IsBinaryQuestion && !mIsDisplayed)
            {
                mIsDisplayed = true;
                if (string.IsNullOrEmpty(BuddySays))
                    //if()
                    mTTS.Say(Dictionary.GetRandomString(BuddySays));
                if (IsBinaryToaster)
                {
                    if (string.IsNullOrEmpty(RightButton))
                        RightButton = Dictionary.GetString("yes");
                    if (string.IsNullOrEmpty(LeftButton))
                        LeftButton = Dictionary.GetString("no");
                    
                    //if (string.IsNullOrEmpty(KeyQuestion))
                    //    KeyQuestion = " ";
                    mButtonRight = new ButtonInfo
                    {
                        Label = RightButton,
                        OnClick = PressedRightButton
                    };
                    mButtonLeft = new ButtonInfo
                    {
                        Label = Dictionary.GetString("play"), 
                        OnClick = PressedLeftButton
                    };
                    BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With("lol", mButtonLeft, mButtonRight);
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

        private void PressedRightButton()
        {
            Toaster.Hide();
            if(IsSoundForButton)
                Primitive.Speaker.FX.Play(FxSound);
            Trigger(TriggerRightButton);
        }

        private void PressedLeftButton()
        {
            Toaster.Hide();
            if (IsSoundForButton)
                Primitive.Speaker.FX.Play(FxSound);
            Trigger(TriggerLeftButton);
            
        }

        private void OnSpeechReco(string iVoiceInput)
        {
            mSpeechReco = iVoiceInput;

            Interaction.Mood.Set(MoodType.NEUTRAL);
            mListening = false;
        }

        private void Empty(STTError iError)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }
    }

}
