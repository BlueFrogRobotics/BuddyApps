using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Radio
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class RadioBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private RadioData mAppData;
        /*
         * RadioService manages the radio webservice
         */
        private RadioService mRadioService;
        /*
         * Delta volume used in volume changes if no delta volume specified in the vocal request
         */
        private const float DELTA_VOLUME = 20.0F;

        //FButton mEchoCancelButton;

        void Start()
        {
            /*
			* You can setup your App activity here.
			*/
            RadioActivity.Init(null);

            /*
			* Init your app data
			*/
            mAppData = RadioData.Instance;

            mRadioService = GetComponent<RadioService>();

            // Hide parameters button
            Buddy.GUI.Header.DisplayParametersButton(false);

            // Enable echo cancellation
            if (!Buddy.Sensors.Microphones.EnableEchoCancellation)
            {
                Buddy.Sensors.Microphones.EnableEchoCancellation = true;
                RadioData.Instance.StopEchoCancellation = true;
            }
            else
                // Echo cancellation already on
                RadioData.Instance.StopEchoCancellation = false;

            //mEchoCancelButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            //mEchoCancelButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_sound_on", Context.OS));
            //mEchoCancelButton.OnClick.Add(ToggleEchoCancellation);

            Buddy.Vocal.OnEndListening.Add(OnEndListening);

            StartListen();

            mRadioService.PlayRadio();
        }

        //void ToggleEchoCancellation()
        //{
        //    Buddy.Sensors.Microphones.EnableEchoCancellation = !Buddy.Sensors.Microphones.EnableEchoCancellation;
            
        //    string icon = Buddy.Sensors.Microphones.EnableEchoCancellation ? "os_icon_sound_off" : "os_icon_sound_on";
        //    mEchoCancelButton.SetIcon(Buddy.Resources.Get<Sprite>(icon, Context.OS));
        //}

        void StartListen()
        {
            Buddy.Vocal.Listen(new SpeechInputParameters()
            {
                Grammars = new[] { "radio" },
                RecognitionThreshold = 4500
            });
        }

        private void OnEndListening(SpeechInput iSpeechInput)
        {
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                if (iSpeechInput.Rule.EndsWith("quit"))
                {
                    AAppActivity.QuitApp();
                    return;
                }
                else if (iSpeechInput.Rule.EndsWith("radio"))
                {
                    mRadioService.StopRadio();
                    RadioData.Instance.SetRadio(iSpeechInput.Utterance);
                    mRadioService.PlayRadio();
                }
                else if (iSpeechInput.Rule.EndsWith("volumeup"))
                {
                    float delta_volume = GetDeltaVolume(iSpeechInput.Utterance) / 100F;
                    if ((Buddy.Actuators.Speakers.Volume + delta_volume) > 1)
                        Buddy.Actuators.Speakers.Volume = 1;
                    else
                        Buddy.Actuators.Speakers.Volume += delta_volume;
                }
                else if (iSpeechInput.Rule.EndsWith("volumedown"))
                {
                    float delta_volume = GetDeltaVolume(iSpeechInput.Utterance) / 100F;
                    if ((Buddy.Actuators.Speakers.Volume - delta_volume) < 0)
                        Buddy.Actuators.Speakers.Volume = 0;
                    else
                        Buddy.Actuators.Speakers.Volume -= delta_volume;
                }
            }
            StartListen();
        }

        /*
         * Parse change volume vocal request to get delta volume if specified
         */
        private float GetDeltaVolume(string iText)
        {
            string[] words = iText.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == Buddy.Resources.GetString("by")
                    && i < (words.Length - 1))
                {
                    int delta_volume = 0;
                    if (int.TryParse(words[i + 1], out delta_volume))
                        return delta_volume;
                }
            }
            return DELTA_VOLUME;
        }
    }
}