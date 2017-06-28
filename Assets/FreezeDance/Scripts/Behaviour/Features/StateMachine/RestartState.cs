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

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.TextToSpeech.SayKey("playagain");
            Interaction.Mood.Set(MoodType.NEUTRAL);
            mMusicPlayer = GetComponent<MusicPlayer>();
            Toaster.Display<BinaryQuestionToast>().With(
                "restart",
                () => Restart(),
                () => QuitApp()
            );
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        private void Restart()
        {
            mMusicPlayer.Restart();
            Trigger("Start");
        }
    }
}