using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLSayStartGameplay : AStateMachineBehaviour
    {
        public override void Start()
        {

        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(SayStartGame());
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        private IEnumerator SayStartGame()
        {
            Interaction.Mood.Set(MoodType.HAPPY);
            yield return SayKeyAndWait("greatstartgame");
            Trigger("StartGame");
        }
    }
}

