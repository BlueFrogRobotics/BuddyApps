using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BlueQuark;
using System.Text;
using UnityEngine.Networking;

namespace BuddyApp.Somfy
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

        public override void OnOff(bool iOnOff)
        {
            string lUrl = SomfyData.Instance.URL_API + "/exec/apply";

           

            IOTSomfyActionCommandsJSON[] lCommands = new IOTSomfyActionCommandsJSON[1];
            if (iOnOff)
                lCommands[0] = new IOTSomfyActionCommandsJSON(1, "on", null);
            else
                lCommands[0] = new IOTSomfyActionCommandsJSON(1, "off", null);

            IOTSomfyActionJSON[] lApply = new IOTSomfyActionJSON[1];
            lApply[0] = new IOTSomfyActionJSON( deviceURL, lCommands);
            //lApply[0].deviceURL = deviceURL;
            Debug.Log("l url: " + deviceURL);
            IOTSomfyJSONApply lJson = new IOTSomfyJSONApply(creationTime, lastUpdateTime, "switchAction", lApply);

            /////
            byte[] bytePostData = Encoding.UTF8.GetBytes(lJson.GetNode().ToString());
            UnityWebRequest request = UnityWebRequest.Put(lUrl, bytePostData); //use PUT method to send simple stream of bytes
            request.method = "POST"; //hack to send POST to server instead of PUT
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Cookie", "JSESSIONID=" + System.Uri.EscapeDataString(mSessionID));
            mHasFinishedCommand = false;
            request.SendWebRequest().completed += OnEndRequest;
            /////
            ///

            //lJson.actionList = new List<IOTSomfyActionJSON>();
            //lJson.actionList.Add(lApply[0]);
            //lJson.liste = new IOTSomfyListActions();
            //lJson.liste.actions = lApply;
            //Debug.Log("meuporg mdr: " + JsonUtility.ToJson(lJson));
            //Debug.Log("le fameux jsnode: " + lJson.GetNode().ToString());
            //Request lRequest = new Request("POST", lUrl, Encoding.Default.GetBytes(JsonUtility.ToJson(lJson)));
            //Request lRequest = new Request("POST", lUrl, Encoding.Default.GetBytes(lJson.GetNode().ToString()));
            //lRequest.cookieJar = null;
            //lRequest.SetHeader("Cookie", mSessionID);
            //lRequest.SetHeader("Content-Type", "application/json");
            //Debug.Log("switch on off");
            //Debug.Log("request switch: " + lRequest.bytes);
            //mHasFinishedCommand = false;
            //lRequest.Send((lResult) =>
            //{
            //    if (lResult == null)
            //    {
            //        mHasFinishedCommand = true;
            //        return;
            //    }
            //    string s = System.Text.Encoding.UTF8.GetString(lResult.bytes, 0, lResult.bytes.Length);
            //    Debug.Log("le result: " + s);
            //    mHasFinishedCommand = true;
            //}
            //);
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

        private void OnEndRequest(AsyncOperation lOp)
        {
            mHasFinishedCommand = true;
        }
    }
}
