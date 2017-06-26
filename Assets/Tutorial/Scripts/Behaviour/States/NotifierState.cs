using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.Tutorial
{
    public class NotifierState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(IntroduceNotifier());
        }

        private IEnumerator IntroduceNotifier()
        {
            Interaction.TextToSpeech.SayKey("notifierstate");
            Notifier.Display<SimpleNot>(19F).With(Dictionary.GetString("simplenottext"));
            Notifier.Display<MessageNot>(5F).With(
                Dictionary.GetString("messagenottext"),
                () => { },
                () => { }
            );
            Notifier.Display<AlertNot>(5F).With(
                Dictionary.GetString("alertnottext"),
                () => { },
                () => { }
            );

            yield return new WaitForSeconds(29F);

            while (Interaction.TextToSpeech.IsSpeaking)
                yield return null;

            Trigger(TRIGGER_END_STATE);
        }
    }
}