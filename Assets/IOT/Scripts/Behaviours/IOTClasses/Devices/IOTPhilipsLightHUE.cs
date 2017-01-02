﻿using UnityEngine;
using System.Collections;
using BuddyFeature.Web;
using BuddyOS.UI;

namespace BuddyApp.IOT
{
    public class IOTPhilipsLightHUE : IOTLights
    {
        private Hashtable mState;
        public Hashtable State { get { return mState; } set { mState = value; } }
        private int indice;
        public int Indice { get { return indice; } set { indice = value; } }

        public IOTPhilipsLightHUE() : base()
        {
            mState = new Hashtable();
            mState.Add("on", false);
            mState.Add("bri", 254);
            mState.Add("hue", 14910);
            mState.Add("sat", 144);
            mState.Add("effect", "none");
            mState.Add("ct", 500);
            mState.Add("alert", "none");
            mState.Add("colormode", "xy");
            mState.Add("reachable", false);

            indice = 0;
            mSpriteName = "IOT_Device_Light";
        }

        public override void ChangeName(string iName)
        {
            base.ChangeName(iName);

            Hashtable lLightSettings = new Hashtable();
            lLightSettings.Add("name", iName);

            string lPath = "http://" + mCredentials[0] + "/api/" + mCredentials[1] + "/lights/" + (indice + 1);
            Request lRequest = new Request("PUT", lPath, lLightSettings);
            lRequest.Send((request) =>
            {
            });
        }

        public override void InitializeParams()
        {
            base.InitializeParams();
            GameObject lOnOff = InstanciateParam(ParamType.ONOFF);
            OnOff lOnOffComponent = lOnOff.GetComponent<OnOff>();
            GameObject lIntensity = InstanciateParam(ParamType.GAUGE);
            Gauge lIntensityComponent = lIntensity.GetComponent<Gauge>();
            GameObject lColors = InstanciateParam(ParamType.COLORS);
            IOTColorButton lColorsComponent = lColors.GetComponent<IOTColorButton>();
            GameObject lName = InstanciateParam(ParamType.TEXTFIELD);
            TextField lNameComponent = lName.GetComponent<TextField>();

            lOnOffComponent.Label.text = "ON/OFF";
            lOnOffComponent.IsActive = (bool)mState["on"];
            IOTOnOffCmd lCmdOnOff = new IOTOnOffCmd(this);
            lOnOffComponent.SwitchCommands.Add(lCmdOnOff);

            lColorsComponent.Label.text = "COLORS";
            IOTColorsCmd lCmdColors = new IOTColorsCmd(this);
            lColorsComponent.UpdateCommands.Add(lCmdColors);

            lIntensityComponent.Label.text = "INTENSITY";
            lIntensityComponent.DisplayPercentage = true;
            lIntensityComponent.Slider.minValue = 0;
            lIntensityComponent.Slider.maxValue = 100;
            lIntensityComponent.Slider.value = (float)((int)mState["bri"]/255F)*100;
            IOTSetIntensityCmd lCmd = new IOTSetIntensityCmd(this);
            lIntensityComponent.UpdateCommands.Add(lCmd);

            lNameComponent.Label.text = "NAME";
            IOTChangeNameCmd lCmdChangeName = new IOTChangeNameCmd(this);
            lNameComponent.UpdateCommands.Add(lCmdChangeName);
        }

        private void SetValue(string[] iStr, object[] iVal)
        {
            Hashtable lLightSettings = new Hashtable();

            for (int i = 0; i < iStr.Length; i++)
            {
                string lStr = iStr[i];
                mState[lStr] = iVal[i];
                lLightSettings.Add(lStr, mState[lStr]);
            }

            string lPath = "http://" + mCredentials[0] + "/api/" + mCredentials[1] + "/lights/" + (indice + 1) + "/state";
            Request lRequest = new Request("PUT", lPath, lLightSettings);
            lRequest.Send((request) =>
            {
            });
        }

        public void GetValue()
        {
            string lPath = "http://" + mCredentials[0] + "/api/" + mCredentials[1] + "/lights/" + (indice + 1);
            Request lRequest = new Request("GET", lPath);
            lRequest.Send((lResult) =>
            {
                if (lRequest == null)
                {
                    Debug.LogError("LightHUE not connected");
                    return;
                }

                Hashtable lRes = lResult.response.Object;
                Hashtable lRealState = (Hashtable)lRes["state"];

                mState["on"] = lRealState["on"];
                mState["bri"] = lRealState["bri"];
                mState["hue"] = lRealState["hue"];
                mState["sat"] = lRealState["sat"];
                mState["effect"] = lRealState["effect"];
                mState["ct"] = lRealState["ct"];
                mState["alert"] = lRealState["alert"];
                mState["colormode"] = lRealState["colormode"];
                mState["reachable"] = lRealState["reachable"];

                mName = (string)lRes["name"];

            });
        }

        public void SetColor(Color iColor)
        {
            float lH = 0.0f, lS = 0.0f, lV = 0.0f;
            Color.RGBToHSV(iColor, out lH, out lS, out lV);
            string[] lKey = new string[3] { "bri", "hue", "sat" };
            object[] lValue = new object[3] {
            (int)(lV * 255.0f),
            (int)(lH * 65535.0f),
            (int)(lS * 255.0f)
        };
            SetValue(lKey, lValue);
        }

        public void SetColor(int iColorState)
        {
            Color lColor = new Color();
            switch (iColorState)
            {
                case 0:
                    lColor = Color.white;
                    break;
                case 1:
                    lColor = new Color(0.8F, 0.2F, 0.2F);
                    break;
                case 2:
                    lColor = new Color(0.8F, 0.2F, 0.5F);
                    break;
                case 3:
                    lColor = Color.blue;
                    break;
                case 4:
                    lColor = Color.cyan;
                    break;
                case 5:
                    lColor = new Color(0.0F, 0.8F, 0.1F);
                    break;
                case 6:
                    lColor = new Color(0.0F, 0.6F, 0.6F);
                    break;
                case 7:
                    lColor = Color.yellow;
                    break;
                case 8:
                    lColor = Color.red;
                    break;
            }
            SetColor(lColor);
        }

        public void SetIntensity(float iValue)
        {
            string[] lKey = new string[1] { "bri" };
            object[] lValue = new object[1] { (int)(iValue * 255.0f) };
            SetValue(lKey, lValue);
        }

        public override void OnOff(bool iOnOff)
        {
            string[] lKey = new string[1] { "on" };
            object[] value = new object[1] { iOnOff };
            SetValue(lKey, value);
        }

        public override void Command(int iCommand, float iParam = 0)
        {
            base.Command(iCommand, iParam);
            switch (iCommand)
            {
                case 4:
                    SetColor((int)iParam);
                    break;
                case 5:
                case 6:
                    SetIntensity(iParam);
                    break;
            }
        }
    }
}
