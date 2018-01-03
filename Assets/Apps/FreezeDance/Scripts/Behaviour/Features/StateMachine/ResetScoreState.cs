using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class ResetScoreState : AStateMachineBehaviour
    {

        private Ranking mRanking;

        public override void Start()
        {
            mRanking = GetComponent<Ranking>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //Interaction.TextToSpeech.SayKey("playagain");
            Interaction.Mood.Set(MoodType.NEUTRAL);
           
            
            Toaster.Display<BinaryQuestionToast>().With(
                Dictionary.GetString("resetscores"),
                () => ResetScores(),
                () => Trigger("Settings")
            );
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Toaster.Hide();
        }

        private void ResetScores()
        {
            mRanking.ResetRanking();
            Trigger("Settings");
        }
    }
}