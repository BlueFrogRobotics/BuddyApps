using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class RankState : AStateMachineBehaviour
    {
        private Ranking mRanking;
        private ScoreManager mScoreManager;

        public override void Start()
        {
            mRanking = GetComponent<Ranking>();
            mScoreManager = GetComponent<ScoreManager>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRanking.ShowRanking();
            //mRanking.AddPlayer((int)mScoreManager.Score);
            mRanking.Replay.onClick.AddListener(Replay);
            mRanking.GoToMenu.onClick.AddListener(Menu);
            mScoreManager.Reset();
            //Interaction.TextToSpeech.SayKey("won");
            //Interaction.Mood.Set(MoodType.HAPPY);
            //Toaster.Display<VictoryToast>().With(Dictionary.GetString("won"));
            //StartCoroutine(Restart());
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRanking.Replay.onClick.RemoveListener(Replay);
            mRanking.GoToMenu.onClick.RemoveListener(Menu);
            
        }

        private void Replay()
        {
            mRanking.HideRanking();
            Trigger("Restart");
        }

        private void Menu()
        {
            mRanking.HideRanking();
            Trigger("Menu");
        }
    }
}