using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.UI;
using BuddyFeature.Web;
using System.Text;

namespace BuddyApp.IOT
{
    public class IOTSomfySwitch : IOTSomfyDevice
    {
        public IOTSomfySwitch(IOTSomfyDevice iObject) : base(iObject)
        {
            mType = DeviceType.SWITCH;
            mName = iObject.label;
            mSpriteName = "IOT_Device_Plug";
        }

        public IOTSomfySwitch(IOTSomfyDevice iObject, string iSessionID) : base(iObject)
        {
            mType = DeviceType.SWITCH;
            mName = iObject.label;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Plug";
        }

        public IOTSomfySwitch(string iName, string iURL, string iSessionID) : base(null)
        {
            mType = DeviceType.SWITCH;
            mName = iName;
            mSessionID = iSessionID;
            mSpriteName = "IOT_Device_Plug";
        }

        public override void InitializeParams()
        {
            base.InitializeParams();
            GameObject lOnOff = InstanciateParam(ParamType.ONOFF);
            OnOff lOnOffComponent = lOnOff.GetComponent<OnOff>();
            GameObject lName = InstanciateParam(ParamType.TEXTFIELD);
            TextField lNameComponent = lName.GetComponent<TextField>();

            lOnOffComponent.Label.text = "ON/OFF";
            lOnOffComponent.IsActive = states[1].value == "on" ? true : false;
            IOTOnOffCmd lCmdOnOff = new IOTOnOffCmd(this);
            lOnOffComponent.SwitchCommands.Add(lCmdOnOff);

            lNameComponent.Label.text = "NAME";
            IOTChangeNameCmd lCmdChangeName = new IOTChangeNameCmd(this);
            lNameComponent.EndEditCommands.Add(lCmdChangeName);
        }

        public override void OnOff(bool iOnOff)
        {
            string lUrl = "https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI/exec/apply";

            IOTSomfyActionCommandsJSON[] lCommands = new IOTSomfyActionCommandsJSON[1];
            if (iOnOff)
                lCommands[0] = new IOTSomfyActionCommandsJSON(1, "on", null);
            else
                lCommands[0] = new IOTSomfyActionCommandsJSON(1, "off", null);

            IOTSomfyActionJSON[] lApply = new IOTSomfyActionJSON[1];
            lApply[0] = new IOTSomfyActionJSON(deviceURL, lCommands);

            IOTSomfyJSONApply lJson = new IOTSomfyJSONApply(creationTime, lastUpdateTime, "switchAction", lApply);

            Request lRequest = new Request("POST", lUrl, Encoding.Default.GetBytes(JsonUtility.ToJson(lJson)));
            lRequest.cookieJar = null;
            lRequest.SetHeader("cookie", mSessionID);
            lRequest.SetHeader("Content-Type", "application/json");
            lRequest.Send((lResult) => {
                if (lResult == null)
                    return;
            }
            );
        }

        private void postAction(string iUrl, bool iOnOff)
        {
            if (mSessionID != null)
            {
                WWWForm lForm = new WWWForm();
                lForm.AddField("Cookie", mSessionID);
                WWW lWWW = new WWW(iUrl, lForm);

                while(lWWW.progress < 1F)
                {

                }
                Debug.Log(lWWW.text);
            }
            else
            {
                Debug.Log("Login error");
            }
        }
    }
}
