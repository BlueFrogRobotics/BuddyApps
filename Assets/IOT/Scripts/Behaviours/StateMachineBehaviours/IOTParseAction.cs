using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTParseAction : AIOTStateMachineBehaviours
    {
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            IOTObjects lObject = null;
            string lMsg = CommonStrings["STT"].ToLower();

            List<IOTObjects> lListIOT = GetGameObject(2).GetComponent<IOTList>().Objects;
            for (int i = 0; i < lListIOT.Count; ++i)
            {
                foreach (string lName in lListIOT[i].Name.Split(' '))
                {
                    if (lMsg.Contains(lName.ToLower()))
                    {
                        lObject = lListIOT[i];
                        break;
                    }
                }
                if (lObject != null)
                    break;
            }

            if (lObject != null)
            {
                if (lObject is IOTSystems)
                    ActionSystem(lObject, lMsg, iAnimator);
                else if (lObject is IOTDevices)
                    ActionDevice(lObject, lMsg, iAnimator);
            }else
            {

            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
        }

        private void ActionSystem(IOTObjects iObject, string iMsg, Animator iAnimator)
        {
            IOTDevices lDevice = null;
            List<IOTDevices> lListDevices = ((IOTSystems)iObject).Devices;

            for (int j = 0; j < lListDevices.Count; ++j)
            {
                foreach (string lName2 in iObject.Name.Split(' '))
                {
                    if (iMsg.Contains(lName2.ToLower()))
                    {
                        lDevice = lListDevices[j];
                        break;
                    }
                }
            }
            if (lDevice == null)
                for (int j = 0; j < lListDevices.Count; ++j)
                    lListDevices[j].OnOff(iAnimator.GetInteger(HashList[(int)HashTrigger.ACTION]) > 0 ? true : false);
            else
                lDevice.OnOff(iAnimator.GetInteger(HashList[(int)HashTrigger.ACTION]) > 0 ? true : false);
        }

        private void ActionDevice(IOTObjects iObject, string iMsg, Animator iAnimator)
        {

        }
    }
}