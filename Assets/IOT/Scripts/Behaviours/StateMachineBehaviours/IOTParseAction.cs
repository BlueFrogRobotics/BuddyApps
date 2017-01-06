﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.IOT
{
    public class IOTParseAction : AIOTStateMachineBehaviours
    {
        private List<string> mSayAction = new List<string>();
        private bool mBack = false;
        private bool mErrorBool = false;

        public override void Init()
        {
            mSayAction.Add("I am turning off ");
            mSayAction.Add("I am turning on ");
            mSayAction.Add("I am closing ");
            mSayAction.Add("I am opening ");
            mSayAction.Add("I am changing the values of ");
            mSayAction.Add("I am increasing the value of ");
            mSayAction.Add("I am decreasing the value of ");
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
                mBack = true;
            }
            else
            {
                lObject = FindObject(lListIOT, lMsg, lType);

                if (lObject != null)
                {
                    Action(lObject, iAnimator, lParam);
                    mMood.Set(MoodType.HAPPY);
                    mBack = true;
                }
            }
            if(!mBack)
                mErrorBool = true;
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            iAnimator.SetInteger(HashList[(int)HashTrigger.ACTION], -1);
            iAnimator.SetBool(HashList[(int)HashTrigger.PARAMETERS], false);
            mTTS.SetSpeechRate(1.0f);
            mErrorBool = false;
            mBack = false;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, System.Int32 iLayerIndex)
        {
            if(mTTS.HasFinishedTalking && mBack)
                iAnimator.SetTrigger(HashList[(int)HashTrigger.BACK]);
            else if(mTTS.HasFinishedTalking && mErrorBool)
                iAnimator.SetTrigger(HashList[(int)HashTrigger.MATCH_ERROR]);
        }

        private IOTDevices.DeviceType FindType(ref string iMsg)
        {
            IOTDevices.DeviceType lType = IOTDevices.DeviceType.DEVICE;
            if (iMsg.Contains("lumière") || iMsg.Contains("lamp") || iMsg.Contains("light"))
            {
                lType = IOTDevices.DeviceType.LIGHT;
                iMsg = iMsg.Replace("lumière", "");
                iMsg = iMsg.Replace("light", "");
                iMsg = iMsg.Replace("lamp", "");
            }
            if (iMsg.Contains("prise") || iMsg.Contains("switch") || iMsg.Contains("plug"))
            {
                lType = IOTDevices.DeviceType.SWITCH;
                iMsg = iMsg.Replace("prise", "");
                iMsg = iMsg.Replace("switch", "");
                iMsg = iMsg.Replace("plug", "");
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
            if (iMsg.Contains("thermometer"))
            {
                lType = IOTDevices.DeviceType.THERMOMETER;
                iMsg = iMsg.Replace("thermometer", "");
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
            if(lAction < 7)
            {
                mTTS.SetSpeechRate(2.0f);
                mTTS.Say(mSayAction[lAction] + iObject.Name, true);
            }
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