using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Somfy
{
    public class IOTCommandsLayout //: AWindowLayout
    {
        private SomfyBehaviour mBehaviour;
        //private OnOff mLight;
        //private LabeledButton mOpenStore;
        //private LabeledButton mCloseStore;
        //private Label mLabelTemperature;
        //private TextField mTemperatureField;

        public IOTCommandsLayout(SomfyBehaviour iBehaviour)
        {
            mBehaviour = iBehaviour;
        }

        //public override void Build()
        //{
        //    mLight = CreateWidget<OnOff>();
        //    mOpenStore = CreateWidget<LabeledButton>();
        //    mCloseStore = CreateWidget<LabeledButton>();
        //    mLabelTemperature = CreateWidget<Label>();
        //    mTemperatureField = CreateWidget<TextField>();

        //    mLight.IsActive = false;
        //    mLight.OnSwitchEvent((bool iVal) =>
        //    {
        //        mBehaviour.SwitchPlug(iVal);
        //    });

        //    mOpenStore.OnClickEvent(() => { mBehaviour.OpenStore(); });
        //    mCloseStore.OnClickEvent(() => { mBehaviour.CloseStore(); });
        //    mTemperatureField.OnEndEditEvent(SetTemperature);
        //}

        //public override void LabelizeWidgets()
        //{
        //    mLight.Label = BYOS.Instance.Dictionary.GetString("light");
        //    mOpenStore.OuterLabel= BYOS.Instance.Dictionary.GetString("openstore");
        //    mOpenStore.InnerLabel = BYOS.Instance.Dictionary.GetString("open");
        //    mCloseStore.OuterLabel = BYOS.Instance.Dictionary.GetString("closestore");
        //    mCloseStore.InnerLabel = BYOS.Instance.Dictionary.GetString("close");
        //    mLabelTemperature.Text = "temperature: " + mBehaviour.GetTemperature()+" °C";
        //    mTemperatureField.Label = "set temperature";
        //}

        private void SetTemperature(string iText)
        {
            float lTemp = 0;
            Debug.Log("texte recu: " + iText);
            float.TryParse(iText, out lTemp);
            if(lTemp>10 && lTemp<30)
            {
                mBehaviour.SetTemperature(lTemp);
            }
        }

    }
}