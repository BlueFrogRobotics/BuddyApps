using UnityEngine;
using System.Collections.Generic;
using BuddyFeature.Web;
using System.Text;

namespace BuddyApp.IOT
{
    [System.Serializable]
    public class IOTSomfyLoginJSON
    {
        public string userId;
        public string userPassword;

        public IOTSomfyLoginJSON(string iID, string iPass)
        {
            userId = iID;
            userPassword = iPass;
        }

    }
    [System.Serializable]
    public class IOTSomfyActionCommandsJSON
    {
        public int type;
        public string name;
        public float[] parameters;

        public IOTSomfyActionCommandsJSON(int iType, string iName, float[] iParameters)
        {
            type = iType;
            name = iName;
            parameters = iParameters;
        }

    }
    [System.Serializable]
    public class IOTSomfyActionJSON
    {
        public string deviceURL;
        public IOTSomfyActionCommandsJSON[] commands;

        public IOTSomfyActionJSON(string iDeviceURL, IOTSomfyActionCommandsJSON[] iCommands)
        {
            deviceURL = iDeviceURL;
            commands = iCommands;
        }
    }
    [System.Serializable]
    public class IOTSomfyStateJSON
    {
        public string name;
        public int type;
        public string value;
    }
    [System.Serializable]
    public class IOTSomfyStatesJSON
    {
        public bool eventBased;
        public string[] values;
        public string type;
        public string qualifiedName;
    }
    [System.Serializable]
    public class IOTSomfyEventsJSON
    {
        public string commandName;
        public int nparams;
    }
    [System.Serializable]
    public class IOTSomfyCommandsJSON
    {
        public string commandName;
        public int nparams;
    }
    [System.Serializable]
    public class IOTSomfyDefinitionJSON
    {
        public IOTSomfyCommandsJSON[] commands;
        public IOTSomfyEventsJSON[] events;
        public IOTSomfyStatesJSON[] states;
        public string[] dataProperties;
        public string widgetName;
        public string uiClass;
        public string qualifiedName;
        public string type;
    }
    [System.Serializable]
    public class IOTSomfyDevice : IOTDevices
    {
        public long creationTime;
        public long lastUpdateTime;
        public string label;
        public string deviceURL;
        public bool shortcut;
        public string controllableName;
        public IOTSomfyDefinitionJSON definition;
        public IOTSomfyStateJSON[] states;
        public string[] attributes;
        public bool available;
        public bool enabled;
        public string placeOID;
        public string widget;
        public int type;
        public string oid;
        public string uiClass;

        [System.NonSerialized]
        protected string mSessionID;

        public IOTSomfyDevice() : base() { mAvailable = available; }

        public IOTSomfyDevice(IOTSomfyDevice iObject) : base()
        {
            if(iObject != null)
            {
                creationTime = iObject.creationTime;
                lastUpdateTime = iObject.lastUpdateTime;
                label = iObject.label;
                deviceURL = iObject.deviceURL;
                shortcut = iObject.shortcut;
                controllableName = iObject.controllableName;
                definition = iObject.definition;
                states = iObject.states;
                attributes = iObject.attributes;
                available = iObject.available;
                enabled = iObject.enabled;
                placeOID = iObject.placeOID;
                widget = iObject.widget;
                type = iObject.type;
                oid = iObject.oid;
                uiClass = iObject.uiClass;

                mAvailable = available;
            }
        }


        protected void PostAction(string iCommand, float[] iParams = null)
        {
            string lUrl = "https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI/exec/apply";

            IOTSomfyActionCommandsJSON[] lCommands = new IOTSomfyActionCommandsJSON[1];
            
            if(iParams != null && iParams.Length > 0)
                Debug.Log(iParams[0]);
            lCommands[0] = new IOTSomfyActionCommandsJSON(1, iCommand, iParams);

            IOTSomfyActionJSON[] lApply = new IOTSomfyActionJSON[1];
            lApply[0] = new IOTSomfyActionJSON(deviceURL, lCommands);

            IOTSomfyJSONApply lJson = new IOTSomfyJSONApply(creationTime, lastUpdateTime, "switchAction", lApply);

            Request lRequest = new Request("POST", lUrl, Encoding.Default.GetBytes(JsonUtility.ToJson(lJson)));
            lRequest.cookieJar = null;
            lRequest.SetHeader("cookie", mSessionID);
            lRequest.SetHeader("Content-Type", "application/json");
            lRequest.Send((lResult) => {
                if (lResult == null)
                {
                    Debug.LogError("Couldn't post action");
                    return;
                }
                Debug.Log(lResult.response.Text);
            }
            );
        }

        protected void ChangeStateValue(string lStateName)
        {
            string lUrl = "https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI/setup/devices/" + deviceURL.Replace(":","%3A").Replace("/","%252F").Replace("#","%23") + "/states/" + lStateName.Replace(":","%3A");
            
            Request lRequest = new Request("GET", lUrl);
            lRequest.cookieJar = null;
            lRequest.SetHeader("cookie", mSessionID);
            lRequest.uri = new System.Uri(lUrl);

            IOTSomfyStateJSON lState = null;
            lRequest.Send(lUrl, (lResult) => {
                if (lResult == null)
                {
                    Debug.Log("Couldn't get state");
                    return;
                }
                lState = JsonUtility.FromJson<IOTSomfyStateJSON>(lResult.response.Text);
                for(int i = 0; i < states.Length; ++i)
                {
                    if(states[i].name == lStateName)
                        states[i] = lState;
                }
            }
            );
        }

        public override void ChangeName(string iName)
        {
            base.ChangeName(iName);

            string lUrl = "https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI/setup/devices/" + deviceURL.Replace(":", "%3A").Replace("/", "%252F").Replace("#", "%23") + "/" + iName.Replace(" ", "%20");

            Request lRequest = new Request("PUT", lUrl);
            lRequest.cookieJar = null;
            lRequest.SetHeader("cookie", mSessionID);
            lRequest.Send((lResult) => {
                Debug.Log(lResult.response.Text);
                if (lResult == null)
                {
                    Debug.LogError("Couldn't change name");
                    return;
                }
            }
            );
        }
    }

    public class IOTSomfyJSONApply
    {
        public long creationTime;
        public long lastUpdateTime;
        public string label;
        public string metadata;
        public bool shortcut;
        public int notificationTypeMask;
        public string notificationCondition;
        public string notificationText;
        public string notificationTitle;
        public string[] targetEmailAddresses;
        public string[] targetPhoneNumbers;
        public string[] targetPushSubsciptions;
        public IOTSomfyActionJSON[] actions;

        public IOTSomfyJSONApply(long iCTime, long iUTime, string iLabel, IOTSomfyActionJSON[] iActions)
        {
            creationTime = iCTime;
            creationTime = iUTime;
            label = iLabel;
            metadata = "";
            shortcut = true;
            notificationTypeMask = 1;
            notificationCondition = "ON_SUCCESS";
            notificationText = "";
            notificationTitle = "";
            targetEmailAddresses = null;
            targetPhoneNumbers = null;
            targetPushSubsciptions = null;
            actions = iActions;
        }
    }

    [System.Serializable]
    public class IOTSomfyDeviceCollection
    {
        public IOTSomfyDevice[] devices;
    }

}