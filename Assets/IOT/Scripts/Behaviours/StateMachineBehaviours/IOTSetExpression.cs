using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTSetExpression : AIOTStateMachineBehaviours
    {
        [SerializeField]
        private MoodType mood = MoodType.NEUTRAL;
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            mMood.Set(mood);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}