using UnityEngine;
using System.Collections.Generic;
using System.Text;
using BlueQuark;


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

        private JSONNode mNode;

        public IOTSomfyActionCommandsJSON(int iType, string iName, object[] iParameters)
        {
            type = iType;
            name = iName;
            parameters = iParameters;

            mNode = new JSONObject();
            mNode.Add("type", new JSONNumber(type));
            mNode.Add("name", new JSONString(name));

            JSONArray lArrayFloat = new JSONArray();
            if (parameters != null)
            {
                foreach (object param in parameters)
                {
                    if(param is float)
                        lArrayFloat.Add(new JSONNumber((float)param));
                    else if(param is string)
                        lArrayFloat.Add(new JSONString((string)param));
                }
            }

            mNode.Add("parameters", lArrayFloat);
        }

        public JSONNode GetNode()
        {
            return mNode;
        }

    }
    [System.Serializable]
    public class IOTSomfyActionJSON
    {
        public string deviceURL;
        public IOTSomfyActionCommandsJSON[] commands;

        private JSONNode mNode;

        public IOTSomfyActionJSON(string iDeviceURL, IOTSomfyActionCommandsJSON[] iCommands)
        {
            deviceURL = iDeviceURL;
            commands = iCommands;

            mNode = new JSONObject();
            mNode.Add("deviceURL", deviceURL);

            JSONArray lArrayCommands = new JSONArray();
            if (commands != null)
            {
                foreach (IOTSomfyActionCommandsJSON command in commands)
                {
                    lArrayCommands.Add(command.GetNode());
                }
            }

            mNode.Add("commands", lArrayCommands);
        }

        public JSONNode GetNode()
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
                mHasFinishedCommand = true;
            }
        }

        public bool HasFinishedCommand()
        {
            return mHasFinishedCommand;
        }

        protected void PostAction(string iCommand, object[] iParams = null)
        {
            string lUrl = SomfyData.Instance.URL_API+"/exec/apply";

            IOTSomfyActionCommandsJSON[] lCommands = new IOTSomfyActionCommandsJSON[1];
            
            if(iParams != null && iParams.Length > 0)
                Debug.Log(iParams[0]);
            lCommands[0] = new IOTSomfyActionCommandsJSON(1, iCommand, iParams);
            //Debug.Log("l url du device: " + deviceURL);
            IOTSomfyActionJSON[] lApply = new IOTSomfyActionJSON[1];
            lApply[0] = new IOTSomfyActionJSON( deviceURL, lCommands);
            lApply[0].deviceURL = deviceURL;
            IOTSomfyJSONApply lJson = new IOTSomfyJSONApply(creationTime, lastUpdateTime, "switchAction", lApply);
            //Debug.Log("url du json: " + lJson.actions[0].deviceURL);
            //Request lRequest = new Request("POST", lUrl, Encoding.Default.GetBytes(JsonUtility.ToJson(lJson)));
            Debug.Log("avant post");
            Request lRequest = new Request("POST", lUrl, Encoding.Default.GetBytes(lJson.GetNode().ToString()));
            Debug.Log("json post: " + lJson.GetNode().ToString());
            lRequest.cookieJar = null;
            lRequest.SetHeader("cookie", mSessionID);
            lRequest.SetHeader("Content-Type", "application/json");
            Debug.Log("apres post");
            mHasFinishedCommand = false;
            lRequest.Send((lResult) => {
                if (lResult == null)
                {
                    Debug.LogError("Couldn't post action");
                    mHasFinishedCommand = true;
                    return;
                }
                Debug.Log(lResult.response.Text);
                mHasFinishedCommand = true;
            }
            );
            Debug.Log("encore apres post");
        }

        protected void ChangeStateValue(string lStateName)
        {
            string lUrl = SomfyData.Instance.URL_API + "/setup/devices/" + deviceURL.Replace(":","%3A").Replace("/","%252F").Replace("#","%23") + "/states/" + lStateName.Replace(":","%3A");
            
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

            string lUrl = SomfyData.Instance.URL_API + "/setup/devices/" + deviceURL.Replace(":", "%3A").Replace("/", "%252F").Replace("#", "%23") + "/" + iName.Replace(" ", "%20");

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


        private JSONNode mNode;

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

            //Debug.Log("1");
            mNode = new JSONObject();
            //Debug.Log("2");
            mNode.Add("creationTime", new JSONNumber(creationTime));
            //Debug.Log("3");
            mNode.Add("lastUpdateTime", new JSONNumber(lastUpdateTime));
            //Debug.Log("4");
            mNode.Add("label", new JSONString(label));
            //Debug.Log("5");
            mNode.Add("metadata", new JSONString(metadata));
            mNode.Add("shortcut", new JSONBool(shortcut));
            mNode.Add("notificationTypeMask", new JSONNumber(notificationTypeMask));
            mNode.Add("notificationCondition", new JSONString(notificationCondition));
            mNode.Add("notificationText", new JSONString(notificationText));
            mNode.Add("notificationTitle", new JSONString(notificationTitle));
            //Debug.Log("6");
            mNode.Add("targetEmailAddresses", new JSONArray());
            mNode.Add("targetPhoneNumbers", new JSONArray());
            mNode.Add("targetPushSubsciptions", new JSONArray());
            //Debug.Log("7");
            JSONArray lArrayActions = new JSONArray();
            if(actions!=null)
            {
                foreach(IOTSomfyActionJSON action in actions)
                {
                    lArrayActions.Add(action.GetNode());
                }
            }
            //Debug.Log("8");
            mNode.Add("actions", lArrayActions);
            //Debug.Log("9");
        }

        public JSONNode GetNode()
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
            JSONNode lJsonNode = BlueQuark.JSON.Parse(iJson);
            //Debug.Log("le json d apres: " + lJsonNode.ToString());
            devices = new IOTSomfyDevice[lJsonNode["devices"].Count];
            //Debug.Log("device count " + lJsonNode["devices"].Count);
            for (int i = 0; i < lJsonNode["devices"].Count; i++)
            {
                devices[i] = new IOTSomfyDevice();
                long.TryParse(lJsonNode["devices"][i]["creationTime"].Value, out devices[i].creationTime);
                long.TryParse(lJsonNode["devices"][i]["lastUpdateTime"].Value, out devices[i].lastUpdateTime);
                //Debug.Log("1");
                devices[i].label = lJsonNode["devices"][i]["label"].Value;
                //Debug.Log("device: " + lJsonNode["devices"][i]["label"].Value);
                devices[i].deviceURL = lJsonNode["devices"][i]["deviceURL"].Value;
                bool.TryParse(lJsonNode["devices"][i]["shortcut"].Value, out devices[i].shortcut);
                devices[i].uiClass = lJsonNode["devices"][i]["uiClass"].Value;
                //Debug.Log("un device: "+lJsonNode["devices"][i]["states"][0]["name"].ToString());
                devices[i].states = new IOTSomfyStateJSON[lJsonNode["devices"][i]["states"].Count];
                //Debug.Log("state lenght: "+devices[i].states.Length);
                for (int j = 0; j < lJsonNode["devices"][i]["states"].Count; j++)
                {
                    devices[i].states[j] = new IOTSomfyStateJSON();
                    //Debug.Log("le name : " + lJsonNode["devices"][i]["states"][j]["name"].Value);
                    devices[i].states[j].name = lJsonNode["devices"][i]["states"][j]["name"].Value;
                    //int.TryParse(lJsonNode["devices"][i]["states"][j]["type"].Value, out devices[i].states[j].type);
                    devices[i].states[j].value = lJsonNode["devices"][i]["states"][j]["value"].Value;
                }
            }
        }
    }

}