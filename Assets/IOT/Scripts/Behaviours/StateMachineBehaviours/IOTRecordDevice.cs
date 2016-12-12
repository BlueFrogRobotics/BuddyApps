using UnityEngine;
using System.Collections;

namespace BuddyApp.IOT
{
    public class IOTRecordDevice : AIOTStateMachineBehaviours
    {
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            IOTObjects lObject = GetGameObject(7).GetComponent<IOTNewDevice>().IOTObject;
            GetGameObject(5).GetComponent<IOTList>().Objects.Add(lObject);
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