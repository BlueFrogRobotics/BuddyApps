﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class MenuState : AStateMachineBehaviour
    {
        private bool mListening;
        private float mTimer = 0.0f;
        private bool mSwitchState = false;

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.TextToSpeech.SayKey("nextaction");
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Interaction.SpeechToText.OnBestRecognition.Add(OnRecognition);
            mSwitchState = false;
            mListening = false;
            Toaster.Display<ChoiceToast>().With(
                   "menu",
                   new ButtonInfo()
                   {
                       Label = "start",
                       OnClick = () => Trigger("Start")
                   },
                   new ButtonInfo()
                   {
                       Label = "help",
                       OnClick = () => Trigger("Start")
                   },
                   new ButtonInfo()
                   {
                       Label = "quit",
                       OnClick = () => QuitApp()
                   });
            mTimer = 0.0f;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            if (mTimer > 6.0f)
            {
                Interaction.Mood.Set(MoodType.NEUTRAL);
                mListening = false;
                mTimer = 0.0f;
            }

            if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                return;

            if(!mListening && !mSwitchState)
            {
                Interaction.Mood.Set(MoodType.LISTENING);
                Interaction.SpeechToText.Request(); 
                mListening = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.SpeechToText.OnBestRecognition.Remove(OnRecognition);
        }

        private void OnRecognition(string iText)
        {
            if(Dictionary.ContainsPhonetic(iText, "quit"))
            {
                Toaster.Hide();
                QuitApp();
                mSwitchState = true;
            }
            else if(Dictionary.ContainsPhonetic(iText, "play"))
            {
                Toaster.Hide();
                Trigger("Start");
                mSwitchState = true;
            }
            mListening = false;
        }
    }
}