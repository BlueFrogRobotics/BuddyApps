using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.BasicApp
{
    public class APIState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(API_PANEL_IDX).SetActive(true);
            mRGBCam.Open(RGBCamResolution.W_176_H_144);
            StartCoroutine(SaySomething());
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(API_PANEL_IDX).SetActive(false);
            mMood.Set(MoodType.NEUTRAL);
            mRGBCam.Close();
        }

        private IEnumerator SaySomething()
        {
            mTTS.SayKey("apistate", true);

            yield return new WaitForSeconds(20F);

            while (mTTS.IsSpeaking)
                yield return null;

            mTTS.SayKey("endapistate", true);

            yield return new WaitForSeconds(3F);

            while (mTTS.IsSpeaking)
                yield return null;

            Trigger(TRIGGER_END_STATE);
        }
    }
}