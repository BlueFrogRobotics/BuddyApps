using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.FreezeDance
{
    public class SettingsState : AStateMachineBehaviour
    {
        private SettingsWindow mSettings;
        private MusicPlayer mMusicPlayer;

        public override void Start()
        {
            mSettings = GetComponent<SettingsWindow>();
            mMusicPlayer = GetComponent<MusicPlayer>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mSettings.ShowSettings();
            mSettings.ButtonMenu.onClick.AddListener(Menu);
            mSettings.ButtonPlay.onClick.AddListener(Play);
            mSettings.ButtonResetScores.onClick.AddListener(ResetScore);
            mMusicPlayer.Restart();
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mMusicPlayer.ReinitMusic(mSettings.MusicId);
            mSettings.ButtonMenu.onClick.RemoveListener(Menu);
            mSettings.ButtonPlay.onClick.RemoveListener(Play);
            mSettings.ButtonResetScores.onClick.RemoveListener(ResetScore);
        }

        private void Play()
        {
            mSettings.HideSettings();
            Trigger("Start");
        }

        private void Menu()
        {
            mSettings.HideSettings();
            Trigger("Menu");
        }

        private void ResetScore()
        {
            mSettings.HideSettings();
            Trigger("ResetScores");
        }
    }
}
