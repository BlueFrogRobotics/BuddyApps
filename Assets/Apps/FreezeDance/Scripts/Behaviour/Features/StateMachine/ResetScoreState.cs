using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


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
            //Buddy.Vocal.SayKey("playagain");
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
           
            
            //Toaster.Display<BinaryQuestionToast>().With(
            //    Buddy.Resources.GetString("resetscores"),
            //    () => ResetScores(),
            //    () => Trigger("Settings")
            //);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Toaster.Hide();
        }

        private void ResetScores()
        {
            mRanking.ResetRanking();
            Trigger("Settings");
        }
    }
}