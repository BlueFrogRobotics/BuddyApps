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

        public override void Start()
        {
            mRanking = GetComponent<Ranking>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mRanking.ShowRanking();
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
        }
    }
}