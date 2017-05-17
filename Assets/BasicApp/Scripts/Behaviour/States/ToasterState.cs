using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.BasicApp
{
    public class ToasterState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(IntroduceToasterSpeech());
        }

        private IEnumerator IntroduceToasterSpeech()
        {
            mTTS.SayKey("toasterstate");

            yield return new WaitForSeconds(7F);

            while (mTTS.IsSpeaking)
                yield return null;

            Trigger(TRIGGER_END_STATE);
        }
    }
}