using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.Tutorial
{
    public class APIState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(API_PANEL_IDX).SetActive(true);
            Primitive.RGBCam.Open(RGBCamResolution.W_176_H_144);
            StartCoroutine(SaySomething());
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(API_PANEL_IDX).SetActive(false);
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Primitive.RGBCam.Close();
        }

        private IEnumerator SaySomething()
        {
            Interaction.TextToSpeech.SayKey("apistate", true);

            yield return new WaitForSeconds(20F);

            while (Interaction.TextToSpeech.IsSpeaking)
                yield return null;

            Interaction.TextToSpeech.SayKey("endapistate", true);

            yield return new WaitForSeconds(3F);

            while (Interaction.TextToSpeech.IsSpeaking)
                yield return null;

            Trigger(TRIGGER_END_STATE);
        }
    }
}