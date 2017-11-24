using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class RestartState : AStateMachineBehaviour
    {
        private MusicPlayer mMusicPlayer;
        private bool mListening;
        private float mTimer = 0.0f;
        private bool mSwitchState = false;

        public override void Start()
        {
            mMusicPlayer = GetComponent<MusicPlayer>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.TextToSpeech.SayKey("playagain");
            Interaction.Mood.Set(MoodType.NEUTRAL);
            
            mMusicPlayer.Restart();
            mListening = false;
            mSwitchState = false;
            mTimer = 0.0f;
            Interaction.SpeechToText.OnBestRecognition.Add(OnRecognition);
            Toaster.Display<BinaryQuestionToast>().With(
                Dictionary.GetString("playagain"),
                () => Restart(),
                () => QuitApp()
            );
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

            if (!mListening && !mSwitchState)
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

        private void Restart()
        {
            mMusicPlayer.Restart();
            Trigger("Start");
        }

        private void OnRecognition(string iText)
        {
            if (Dictionary.ContainsPhonetic(iText, "yes"))
            {
                Toaster.Hide();
                Restart();
                mSwitchState = true;
            }
            else if (Dictionary.ContainsPhonetic(iText, "no"))
            {
                Toaster.Hide();
                QuitApp();
                mSwitchState = true;
            }
            mListening = false;
        }
    }
}