using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Somfy
{
    public class SomfyManualCommands : AStateMachineBehaviour
    {
        private SomfyBehaviour mSomfyBehaviour;
        private bool mInitialized;
        private bool mChangeState;

        private TToggle mLight;
        private TButton mOpenStore;
        private TButton mCloseStore;
        private TText mLabelTemperature;
        private TTextField mTemperatureField;
        private TButton mPlay;
        private TButton mStop;

        public override void Start()
        {
            mSomfyBehaviour = GetComponent<SomfyBehaviour>();
            //mCommandsLayout = new IOTCommandsLayout(mSomfyBehaviour);
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //Toaster.Display<ParameterToast>().With(mCommandsLayout, () => { Trigger("Vocal"); });
            Buddy.GUI.Toaster.Display<ParameterToast>().With(
                (iBuilder) =>
                {
                    mLight = iBuilder.CreateWidget<TToggle>();
                    mLight.SetLabel(Buddy.Resources.GetString("light"));
                    mLight.OnToggle.Add((iVal) => mSomfyBehaviour.SwitchPlug(iVal));

                    mOpenStore = iBuilder.CreateWidget<TButton>();
                    mOpenStore.SetLabel(Buddy.Resources.GetString("openstore"));
                    mOpenStore.OnClick.Add(mSomfyBehaviour.OpenStore);

                    mCloseStore = iBuilder.CreateWidget<TButton>();
                    mCloseStore.SetLabel(Buddy.Resources.GetString("closestore"));
                    mCloseStore.OnClick.Add(mSomfyBehaviour.CloseStore);

                    mLabelTemperature = iBuilder.CreateWidget<TText>();
                    mLabelTemperature.SetLabel("temperature: " + mSomfyBehaviour.GetTemperature() + " °C");

                    mTemperatureField = iBuilder.CreateWidget<TTextField>();
                    mTemperatureField.SetPlaceHolder("temperature");
                    mTemperatureField.OnEndEdit.Add(SetTemperature);

                    mPlay = iBuilder.CreateWidget<TButton>();
                    mPlay.SetLabel("play");
                    mPlay.OnClick.Add(mSomfyBehaviour.PlayMusic);

                    mStop = iBuilder.CreateWidget<TButton>();
                    mStop.SetLabel("stop");
                    mStop.OnClick.Add(mSomfyBehaviour.StopMusic);

                },
                SaveAndQuit, "cancel", SaveAndQuit, "save");
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
     
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Toaster.Hide();
        }

        private void SaveAndQuit()
        {
            Trigger("Vocal");
        }

        private void SetTemperature(string iText)
        {
            float lTemp = 0;
            Debug.Log("texte recu: " + iText);
            float.TryParse(iText, out lTemp);
            if (lTemp > 10 && lTemp < 30)
            {
                mSomfyBehaviour.SetTemperature(lTemp);
            }

        }
    }
}