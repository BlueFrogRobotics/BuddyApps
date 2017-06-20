using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Buddy;
using Buddy.UI;

namespace BuddyApp.TakePose
{
    public class RedoPose : AStateMachineBehaviour
    {
        private bool mListening;
        private string mSpeechReco;

        private List<string> mAcceptPhonetics;
        private List<string> mRefusePhonetics;
        private List<string> mAnotherPhonetics;
        private List<string> mQuitPhonetics;

        public override void Start()
        {
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mListening = false;
            mSpeechReco = "";

            mAcceptPhonetics = new List<string>(Dictionary.GetPhoneticStrings("accept"));
            mRefusePhonetics = new List<string>(Dictionary.GetPhoneticStrings("refuse"));
            mAnotherPhonetics = new List<string>(Dictionary.GetPhoneticStrings("another"));
            mQuitPhonetics = new List<string>(Dictionary.GetPhoneticStrings("quit"));

            Interaction.TextToSpeech.SayKey("redopose");

            Interaction.SpeechToText.OnBestRecognition.Clear();
            Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechReco);

            Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetString("redopose"), PressedYes, PressedNo);
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                return;

            if (string.IsNullOrEmpty(mSpeechReco)) {
                Interaction.SpeechToText.Request();
                mListening = true;

                Interaction.Mood.Set(MoodType.LISTENING);
                return;
            }

            if (mAcceptPhonetics.Contains(mSpeechReco) || mAnotherPhonetics.Contains(mSpeechReco)) {
                Toaster.Hide();
                RedoTakePose();
            } else if (mRefusePhonetics.Contains(mSpeechReco) || mQuitPhonetics.Contains(mSpeechReco)) {
                Toaster.Hide();
                ExitTakePose();
            } else {
                Interaction.TextToSpeech.SayKey("notunderstand", true);
                Interaction.TextToSpeech.Silence(1000, true);
                Interaction.TextToSpeech.SayKey("yesorno", true);
                Interaction.TextToSpeech.Silence(1000, true);
                Interaction.TextToSpeech.SayKey("redopose", true);

                mSpeechReco = "";
            }

        }

        private void PressedYes()
        {
            Primitive.Speaker.FX.Play(FXSound.BEEP_1);
            RedoTakePose();
        }

        private void PressedNo()
        {
            Primitive.Speaker.FX.Play(FXSound.BEEP_1);
            ExitTakePose();
        }

        private void OnSpeechReco(string iVoiceInput)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);

            mSpeechReco = iVoiceInput;
            mListening = false;
        }

        private void RedoTakePose()
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Trigger("Pose");
        }

        private void ExitTakePose()
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Interaction.TextToSpeech.SayKey("noredo");
            Trigger("Hastag");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
            mSpeechReco = "";
            mListening = false;
        }

    }
}