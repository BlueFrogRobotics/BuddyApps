using UnityEngine;
using System.Collections;
namespace BuddyApp.IOT
{
    public class IOTOpenState : AIOTStateMachineBehaviours
    {
        [SerializeField]
        private bool setActive;
        [SerializeField]
        private int gameobject;
        [SerializeField]
        private HashTrigger trigger;
        [SerializeField]
        private bool triggerOrNotTrigger = false;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            GetGameObject(gameobject).SetActive(setActive);
            if(triggerOrNotTrigger)
                iAnimator.SetTrigger(HashList[(int)trigger]);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
