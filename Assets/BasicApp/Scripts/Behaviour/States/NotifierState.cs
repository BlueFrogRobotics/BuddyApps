using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.BasicApp
{
    public class NotifierState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(IntroduceNotifier());
        }

        private IEnumerator IntroduceNotifier()
        {
            mTTS.SayKey("notifierstate");
            mNotifier.Display<SimpleNot>(19F).With(mDictionary.GetString("simplenottext"));
            mNotifier.Display<MessageNot>(5F).With(
                mDictionary.GetString("messagenottext"),
                () => { },
                () => { }
            );
            mNotifier.Display<AlertNot>(5F).With(
                mDictionary.GetString("alertnottext"),
                () => { },
                () => { }
            );

            yield return new WaitForSeconds(29F);

            while (mTTS.IsSpeaking)
                yield return null;

            Trigger(TRIGGER_END_STATE);
        }
    }
}