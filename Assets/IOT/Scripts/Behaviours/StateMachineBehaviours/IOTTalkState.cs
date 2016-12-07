using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTTalkState : AIOTStateMachineBehaviours
    {
        [SerializeField]
        private string Sentence;

        [SerializeField]
        private HashTrigger trigger;
        [SerializeField]
        private bool triggerOrNotTrigger = false;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            mTTS.Say(Sentence);
            GetGameObject(1).GetComponent<IOTSayingThis>().showMessage(Sentence);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            if (triggerOrNotTrigger)
                if (!mTTS.IsSpeaking())
                    iAnimator.SetTrigger(mHashList[(int)trigger]);
        }
    }
}
