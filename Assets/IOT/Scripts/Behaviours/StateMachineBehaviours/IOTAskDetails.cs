using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTAskDetails : AIOTStateMachineBehaviours
    {
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            Transform lIOTDeviceButton = GetGameObject(2).transform.GetChild(0).GetChild(0).GetChild(0).GetChild(iAnimator.GetInteger(HashList[(int)HashTrigger.Choice])-1);
            GetGameObject(3).GetComponent<IOTDetails>().Object = lIOTDeviceButton.GetComponent<IOTObjectContainer>().Object;
            iAnimator.SetTrigger(HashList[(int)HashTrigger.NEXT]);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }
    }
}
