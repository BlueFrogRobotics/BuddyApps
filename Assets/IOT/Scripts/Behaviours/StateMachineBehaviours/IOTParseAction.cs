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
            float lParam = 0F;
            if(CommonStrings["PARAM"] != "")
                lParam = (float)System.Convert.ToDouble(CommonStrings["PARAM"]);

            List<IOTObjects> lListIOT = GetGameObject(2).GetComponent<IOTList>().Objects;

            IOTDevices.DeviceType lType = FindType(ref lMsg);

            if (lMsg.Contains("tout") || lMsg.Contains("tous") || lMsg.Contains("all") || lMsg.Contains("every"))
            {
                for (int i = 0; i < lListIOT.Count; ++i)
                    ActionAll(lListIOT[i], iAnimator, lType,lParam);

                mMood.Set(MoodType.SURPRISED);
                iAnimator.SetTrigger(HashList[(int)HashTrigger.BACK]);
            }
            else
            {
                lObject = FindObject(lListIOT, lMsg, lType);

                if (lObject != null)
                {
                    Action(lObject, iAnimator, lParam);
                    mMood.Set(MoodType.HAPPY);
                    iAnimator.SetTrigger(HashList[(int)HashTrigger.BACK]);
                }
                else
                    iAnimator.SetTrigger(HashList[(int)HashTrigger.MATCH_ERROR]);
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            iAnimator.SetInteger(HashList[(int)HashTrigger.ACTION], -1);
            iAnimator.SetBool(HashList[(int)HashTrigger.PARAMETERS], false);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            if(iAnimator.GetBool(HashList[(int)HashTrigger.MATCH_ERROR]) != true)
                    iAnimator.SetTrigger(HashList[(int)HashTrigger.BACK]);
        }

        private IOTDevices.DeviceType FindType(ref string iMsg)
        {
            IOTDevices.DeviceType lType = IOTDevices.DeviceType.DEVICE;
            if (iMsg.Contains("lumière") || iMsg.Contains("lampe") || iMsg.Contains("light"))
            {
                lType = IOTDevices.DeviceType.LIGHT;
                iMsg = iMsg.Replace("lumière", "");
                iMsg = iMsg.Replace("light", "");
                iMsg = iMsg.Replace("lampe", "");
            }
            if (iMsg.Contains("prise") || iMsg.Contains("switch"))
            {
                lType = IOTDevices.DeviceType.SWITCH;
                iMsg = iMsg.Replace("prise", "");
                iMsg = iMsg.Replace("switch", "");
            }
            if (iMsg.Contains("volet") || iMsg.Contains("screen") || iMsg.Contains("store"))
            {
                lType = IOTDevices.DeviceType.STORE;
                iMsg = iMsg.Replace("volet", "");
                iMsg = iMsg.Replace("screen", "");
                iMsg = iMsg.Replace("store", "");
            }
            if (iMsg.Contains("thermostat"))
            {
                lType = IOTDevices.DeviceType.THERMOSTAT;
                iMsg = iMsg.Replace("thermostat", "");
            }
            return lType;
        }

        private IOTObjects FindObject(List<IOTObjects> iObjects, string iMsg, IOTDevices.DeviceType iType)
        {
            IOTObjects lRes = null;

            for (int i = 0; i < iObjects.Count; ++i)
                if ((lRes = FindObj(iObjects[i], iMsg, iType)) != null)
                    break;

            return lRes;
        }

        private IOTObjects FindObj(IOTObjects iObject, string iMsg, IOTDevices.DeviceType iType)
        {
            IOTObjects lRes = null;

            if (iObject is IOTSystems)
            {
                IOTSystems iSys = (IOTSystems)iObject;
                for (int i = 0; i < iSys.Devices.Count; ++i)
                    if ((lRes = FindObj(iSys.Devices[i], iMsg, iType)) != null)
                        break;
            }
            else
            {
                if(((IOTDevices)iObject).Type == iType || iType == IOTDevices.DeviceType.DEVICE)
                {
                    foreach (string lName in iObject.Name.Split(' '))
                    {
                        if (iMsg.Contains(lName.ToLower()))
                            lRes = iObject;
                    }
                }
            }
            return lRes;
        }

        private void Action(IOTObjects iObject, Animator iAnimator, float iParam = 0F)
        {
            int lAction = iAnimator.GetInteger(HashList[(int)HashTrigger.ACTION]);
            if (lAction > -1)
                ((IOTDevices)iObject).Command(lAction, iParam);
        }

        private void ActionAll(IOTObjects iObject, Animator iAnimator, IOTDevices.DeviceType iType, float iParam = 0F)
        {
            if (iObject is IOTSystems)
            {
                for (int i = 0; i < ((IOTSystems)iObject).Devices.Count; ++i)
                    ActionAll(((IOTSystems)iObject).Devices[i], iAnimator, iType);
            }
            else
            {
                if (((IOTDevices)iObject).Type == iType || iType == IOTDevices.DeviceType.DEVICE)
                    Action(iObject, iAnimator, iParam);
            }
        }
    }
}