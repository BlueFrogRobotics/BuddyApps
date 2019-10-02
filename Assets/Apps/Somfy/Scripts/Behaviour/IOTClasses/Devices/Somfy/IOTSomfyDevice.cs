using BlueQuark;

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.Networking;

using System.Text;
using System.Collections.Generic;

namespace BuddyApp.Somfy
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
        public object[] parameters;

        private JObject mNode;

        public IOTSomfyActionCommandsJSON(int iType, string iName, object[] iParameters)
        {
            type = iType;
            name = iName;
            parameters = iParameters;

            mNode = new JObject();
            mNode.Add("type", type);
            mNode.Add("name", name);

            JArray lArrayFloat = new JArray();
            if (parameters != null) {
                foreach (object param in parameters)
                    lArrayFloat.Add(param);
            }

            mNode.Add("parameters", lArrayFloat);
        }

        public JObject GetNode()
        {
            return mNode;
        }

    }
    [System.Serializable]
    public class IOTSomfyActionJSON
    {
        public string deviceURL;
        public IOTSomfyActionCommandsJSON[] commands;

        private JObject mNode;

        public IOTSomfyActionJSON(string iDeviceURL, IOTSomfyActionCommandsJSON[] iCommands)
        {
            deviceURL = iDeviceURL;
            commands = iCommands;

            mNode = new JObject();
            mNode.Add("deviceURL", deviceURL);

            JArray lArrayCommands = new JArray();
            if (commands != null) {
                foreach (IOTSomfyActionCommandsJSON command in commands) {
                    lArrayCommands.Add(command.GetNode());
                }
            }

            mNode.Add("commands", lArrayCommands);
        }

        public JObject GetNode()
        {
            return mNode;
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

        protected bool mHasFinishedCommand;

        public IOTSomfyDevice() : base() { mAvailable = available; mHasFinishedCommand = true; }

        public IOTSomfyDevice(IOTSomfyDevice iObject) : base()
        {
            if (iObject != null) {
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
                mHasFinishedCommand = true;
            }
        }

        public bool HasFinishedCommand()
        {
            return mHasFinishedCommand;
        }

        protected void PostAction(string iCommand, object[] iParams = null)
        {
            string lUrl = SomfyData.Instance.URL_API + "/exec/apply";

            IOTSomfyActionCommandsJSON[] lCommands = new IOTSomfyActionCommandsJSON[1];

            if (iParams != null && iParams.Length > 0)
                Debug.Log(iParams[0]);
            lCommands[0] = new IOTSomfyActionCommandsJSON(1, iCommand, iParams);
            //Debug.Log("l url du device: " + deviceURL);
            IOTSomfyActionJSON[] lApply = new IOTSomfyActionJSON[1];
            lApply[0] = new IOTSomfyActionJSON(deviceURL, lCommands);
            lApply[0].deviceURL = deviceURL;
            IOTSomfyJSONApply lJson = new IOTSomfyJSONApply(creationTime, lastUpdateTime, "switchAction", lApply);
            //Debug.Log("url du json: " + lJson.actions[0].deviceURL);
            //Request lRequest = new Request("POST", lUrl, Encoding.Default.GetBytes(JsonUtility.ToJson(lJson)));

            ///
            byte[] bytePostData = Encoding.UTF8.GetBytes(lJson.GetNode().ToString());
            UnityWebRequest request = UnityWebRequest.Put(lUrl, bytePostData); //use PUT method to send simple stream of bytes
            request.method = "POST"; //hack to send POST to server instead of PUT
            request.SetRequestHeader("Content-Type", "application/json");

            if (Application.platform == RuntimePlatform.WindowsEditor)
                request.SetRequestHeader("Cookie", "JSESSIONID=" + System.Uri.EscapeDataString(mSessionID));

            mHasFinishedCommand = false;
            request.SendWebRequest().completed += OnEndRequest;
            ///

            //Debug.Log("avant post");
            //Request lRequest = new Request("POST", lUrl, Encoding.Default.GetBytes(lJson.GetNode().ToString()));
            //Debug.Log("json post: " + lJson.GetNode().ToString());
            //lRequest.cookieJar = null;
            //lRequest.SetHeader("cookie", mSessionID);
            //lRequest.SetHeader("Content-Type", "application/json");
            //Debug.Log("apres post");
            //mHasFinishedCommand = false;
            //lRequest.Send((lResult) => {
            //    if (lResult == null)
            //    {
            //        Debug.LogError("Couldn't post action");
            //        mHasFinishedCommand = true;
            //        return;
            //    }
            //    Debug.Log(lResult.response.Text);
            //    mHasFinishedCommand = true;
            //}
            //);
            //Debug.Log("encore apres post");
        }

        protected void ChangeStateValue(string lStateName)
        {
            string lUrl = SomfyData.Instance.URL_API + "/setup/devices/" + deviceURL.Replace(":", "%3A").Replace("/", "%252F").Replace("#", "%23") + "/states/" + lStateName.Replace(":", "%3A");

            Request lRequest = new Request("GET", lUrl);
            lRequest.cookieJar = null;
            lRequest.SetHeader("cookie", mSessionID);
            lRequest.uri = new System.Uri(lUrl);

            IOTSomfyStateJSON lState = null;
            lRequest.Send(lUrl, (lResult) => {
                if (lResult == null) {
                    Debug.Log("Couldn't get state");
                    return;
                }
                lState = JsonUtility.FromJson<IOTSomfyStateJSON>(lResult.response.Text);
                for (int i = 0; i < states.Length; ++i) {
                    if (states[i].name == lStateName)
                        states[i] = lState;
                }
            }
            );
        }

        private void OnEndRequest(AsyncOperation lOp)
        {
            mHasFinishedCommand = true;
        }

        public override void ChangeName(string iName)
        {
            base.ChangeName(iName);

            string lUrl = SomfyData.Instance.URL_API + "/setup/devices/" + deviceURL.Replace(":", "%3A").Replace("/", "%252F").Replace("#", "%23") + "/" + iName.Replace(" ", "%20");

            Request lRequest = new Request("PUT", lUrl);
            lRequest.cookieJar = null;
            lRequest.SetHeader("cookie", mSessionID);
            lRequest.Send((lResult) => {
                Debug.Log(lResult.response.Text);
                if (lResult == null) {
                    Debug.LogError("Couldn't change name");
                    return;
                }
            }
            );
        }
    }

    [System.Serializable]
    public class IOTSomfyListActions
    {
        public IOTSomfyActionJSON[] actions;
    }

    [System.Serializable]
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
        //public IOTSomfyListActions liste;
        //public List<IOTSomfyActionJSON> actionList;
        public IOTSomfyActionJSON[] actions;

        private JObject mNode;

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

            mNode = new JObject();
            mNode.Add("creationTime", creationTime);
            mNode.Add("lastUpdateTime", lastUpdateTime);
            mNode.Add("label", label);
            mNode.Add("metadata", metadata);
            mNode.Add("shortcut", shortcut);
            mNode.Add("notificationTypeMask", notificationTypeMask);
            mNode.Add("notificationCondition", notificationCondition);
            mNode.Add("notificationText", notificationText);
            mNode.Add("notificationTitle", notificationTitle);
            mNode.Add("targetEmailAddresses", new JArray());
            mNode.Add("targetPhoneNumbers", new JArray());
            mNode.Add("targetPushSubsciptions", new JArray());
            JArray lArrayActions = new JArray();
            if (actions != null) {
                foreach (IOTSomfyActionJSON action in actions) {
                    lArrayActions.Add(action.GetNode());
                }
            }
            mNode.Add("actions", lArrayActions);
        }

        public JObject GetNode()
        {
            return mNode;
        }
    }

    [System.Serializable]
    public class IOTSomfyDeviceCollection
    {
        public IOTSomfyDevice[] devices;

        public IOTSomfyDeviceCollection()
        {
            devices = new IOTSomfyDevice[0];
        }

        /// <summary>
        /// Parse json and create object using it
        /// </summary>
        /// <param name="iJson"></param>
        public IOTSomfyDeviceCollection(string iJson)
        {
            //Debug.Log("le json d origin: " + iJson);
            JObject lJsonNode = Utils.UnserializeJSONtoObject(iJson);
            JArray lJArray = (JArray)lJsonNode["devices"];
            //Debug.Log("le json d apres: " + lJsonNode.ToString());
            devices = new IOTSomfyDevice[lJArray.Count];
            //Debug.Log("device count " + lJArray.Count);
            for (int i = 0; i < lJArray.Count; i++) {
                devices[i] = new IOTSomfyDevice();
                long.TryParse((string)lJArray[i]["creationTime"], out devices[i].creationTime);
                long.TryParse((string)lJArray[i]["lastUpdateTime"], out devices[i].lastUpdateTime);
                //Debug.Log("1");
                devices[i].label = (string)lJArray[i]["label"];
                //Debug.Log("device: " + lJArray[i]["label"].Value);
                devices[i].deviceURL = (string)lJArray[i]["deviceURL"];
                bool.TryParse((string)lJArray[i]["shortcut"], out devices[i].shortcut);
                devices[i].uiClass = (string)lJArray[i]["uiClass"];
                //Debug.Log("un device: "+lJArray[i]["states"][0]["name"].ToString());
                JArray lStatesArray = (JArray)lJArray[i]["states"];
                devices[i].states = new IOTSomfyStateJSON[lStatesArray.Count];
                //Debug.Log("state lenght: "+devices[i].states.Length);
                for (int j = 0; j < lStatesArray.Count; j++) {
                    devices[i].states[j] = new IOTSomfyStateJSON();
                    //Debug.Log("le name : " + lJArray[i]["states"][j]["name"].Value);
                    devices[i].states[j].name = (string)lStatesArray[j]["name"];
                    //int.TryParse(lJArray[i]["states"][j]["type"].Value, out devices[i].states[j].type);
                    devices[i].states[j].value = (string)lStatesArray[j]["value"];
                }
            }
        }
    }
}