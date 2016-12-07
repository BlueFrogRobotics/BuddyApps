using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace BuddyApp.IOT
{
    public class IOTActiveState : AIOTStateMachineBehaviours
    {
        public enum EnterOrExit { ONENTER, ONEXIT};

        [SerializeField]
        private EnterOrExit when = EnterOrExit.ONENTER;
        [SerializeField]
        private bool setActive = false;
        [SerializeField]
        private List<int> gameobject = new List<int>();

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            if(when == EnterOrExit.ONENTER)
            {
                for(int i = 0; i < gameobject.Count; ++i)
                    GetGameObject(gameobject[i]).SetActive(setActive);
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            if (when == EnterOrExit.ONEXIT)
            {
                for (int i = 0; i < gameobject.Count; ++i)
                    GetGameObject(gameobject[i]).SetActive(setActive);
            }
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
