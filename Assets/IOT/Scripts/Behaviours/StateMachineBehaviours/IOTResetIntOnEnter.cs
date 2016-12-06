using UnityEngine;
using System.Collections;
namespace BuddyApp.IOT
{
    public class IOTResetIntOnEnter : AIOTStateMachineBehaviours
    {
        [SerializeField]
        private int resetTo = -1;
        [SerializeField]
        private HashTrigger parameter;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            Debug.Log(parameter);
            if (iAnimator.GetParameter((int)parameter).type == AnimatorControllerParameterType.Int)
                iAnimator.SetInteger(HashList[(int)parameter], resetTo);
            else
                Debug.LogError("Reseting parameter type " + iAnimator.GetParameter((int)parameter).type.ToString());
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
