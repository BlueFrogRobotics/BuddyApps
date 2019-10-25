using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.FreezeDance
{
    public class MenuState : AStateMachineBehaviour
    {
        private bool mListening;
        private float mTimer = 0.0f;
        private bool mSwitchState = false;
        private MusicPlayer mMusicPlayer;
        private bool IsClick;

        public override void Start()
        {
            mMusicPlayer = GetComponent<MusicPlayer>();
        }

        public void Ft_IsClick(string name)
        {
            if (IsClick)
            {
                IsClick = false;
                Trigger(name);
            }
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            SetBool("ScoreBool", false);
            IsClick = true;
            mMusicPlayer.Restart();
            Buddy.Vocal.SayKey("nextaction");
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            //Buddy.Vocal.OnEndListening.Add(OnRecognition);
            mSwitchState = false;
            mListening = false;
            //Toaster.Display<ChoiceToast>().With(
            //       "menu",
            //       new ButtonInfo()
            //       {
            //           Label = Buddy.Resources.GetString("play"),//"start",
            //           OnClick = () => Ft_IsClick("Start")
            //       },
            //       //new ButtonInfo()
            //       //{
            //       //    Label = Buddy.Resources.GetString("setupandplay"),//"help",
            //       //    OnClick = () => Trigger("Settings")
            //       //},
            //       new ButtonInfo()
            //       {

            //           Label = Buddy.Resources.GetString("bestscores"),//"quit",
            //           OnClick = () => Ft_IsClick("Ranking"),

            //       });
            mTimer = 0.0f;
        }


        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            if (mTimer > 6.0f)
            {
                Buddy.Behaviour.SetMood(Mood.NEUTRAL);
                mListening = false;
                mTimer = 0.0f;
            }

            if (Buddy.Vocal.IsBusy || mListening)
                return;

            if(!mListening && !mSwitchState)
            {
                Buddy.Behaviour.SetMood(Mood.LISTENING);
                //Interaction.SpeechToText.Request(); 
                mListening = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Toaster.Hide();
            //Interaction.SpeechToText.OnBestRecognition.Remove(OnRecognition);
        }

        private void OnRecognition(string iText)
        {
            if(Buddy.Resources.ContainsPhonetic(iText, "quit"))
            {
                Buddy.GUI.Toaster.Hide();
                QuitApp();
                mSwitchState = true;
            }
            else if(Buddy.Resources.ContainsPhonetic(iText, "play"))
            {
                Buddy.GUI.Toaster.Hide();
                Trigger("Start");
                mSwitchState = true;
            }
            mListening = false;
        }
    }
}