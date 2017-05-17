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

            mAcceptPhonetics = new List<string>(mDictionary.GetPhoneticStrings("accept"));
            mRefusePhonetics = new List<string>(mDictionary.GetPhoneticStrings("refuse"));
            mAnotherPhonetics = new List<string>(mDictionary.GetPhoneticStrings("another"));
            mQuitPhonetics = new List<string>(mDictionary.GetPhoneticStrings("quit"));

            mTTS.SayKey("redopose");

            mSTT.OnBestRecognition.Clear();
            mSTT.OnBestRecognition.Add(OnSpeechReco);

            mToaster.Display<BinaryQuestionToast>().With(mDictionary.GetString("redopose"), PressedYes, PressedNo);
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mTTS.HasFinishedTalking || mListening)
                return;

            if (string.IsNullOrEmpty(mSpeechReco))
            {
                mSTT.Request();
                mListening = true;

                mMood.Set(MoodType.LISTENING);
                return;
            }

            if (mAcceptPhonetics.Contains(mSpeechReco) || mAnotherPhonetics.Contains(mSpeechReco))
            {
                mToaster.Hide();
                RedoTakePose();
            }
            else if (mRefusePhonetics.Contains(mSpeechReco) || mQuitPhonetics.Contains(mSpeechReco))
            {
                mToaster.Hide();
                ExitTakePose();
            }
            else
            {
                mTTS.SayKey("notunderstand", true);
                mTTS.Silence(1000, true);
                mTTS.SayKey("yesorno", true);
                mTTS.Silence(1000, true);
                mTTS.SayKey("redopose", true);

                mSpeechReco = "";
            }

        }

        private void PressedYes()
        {
            mSpeaker.FX.Play(FXSound.BEEP_1);
            RedoTakePose();
        }

        private void PressedNo()
        {
            mSpeaker.FX.Play(FXSound.BEEP_1);
            ExitTakePose();
        }

        private void OnSpeechReco(string iVoiceInput)
        {
            mMood.Set(MoodType.NEUTRAL);

            mSpeechReco = iVoiceInput;
            mListening = false;
        }

        private void RedoTakePose()
        {
            mMood.Set(MoodType.NEUTRAL);
            Trigger("Pose");
        }

        private void ExitTakePose()
        {
            mMood.Set(MoodType.NEUTRAL);
            mTTS.SayKey("noredo");
            Trigger("Hastag");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mMood.Set(MoodType.NEUTRAL);
            mSpeechReco = "";
            mListening = false;
        }

    }
}