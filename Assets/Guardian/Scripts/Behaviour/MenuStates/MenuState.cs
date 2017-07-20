using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
    public class MenuState : AStateMachineBehaviour
    {
        private List<string> mFixedPhonetics;
        private List<string> mMobilePhonetics;
        private List<string> mQuitPhonetics;

        private string mSpeechReco;

        private bool mHasDisplayChoices;
        private bool mListening;

        private float mTimer = 0.0f;

        public override void Start()
        {
            mFixedPhonetics = new List<string>(Dictionary.GetPhoneticStrings("fixed"));
            mMobilePhonetics = new List<string>(Dictionary.GetPhoneticStrings("mobile"));
            mQuitPhonetics = new List<string>(Dictionary.GetPhoneticStrings("quit"));
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            AAppActivity.UnlockScreen();

            Interaction.TextToSpeech.SayKey("askchoices");

            Interaction.SpeechToText.OnBestRecognition.Clear();
            Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechReco);
            mTimer = 0.0f;
            if(!BYOS.Instance.Primitive.RGBCam.IsOpen)
                BYOS.Instance.Primitive.RGBCam.Open(RGBCamResolution.W_176_H_144);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (mTimer > 6.0f) {
                Interaction.Mood.Set(MoodType.NEUTRAL);
                mListening = false;
                mTimer = 0.0f;
                mSpeechReco = null;
            }

            if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                return;

            if (!mHasDisplayChoices) {
                DisplayChoices();
                mHasDisplayChoices = true;
                return;
            }

            if (string.IsNullOrEmpty(mSpeechReco)) {
                Interaction.SpeechToText.Request();

                Interaction.Mood.Set(MoodType.LISTENING);
                mListening = true;
                return;
            }

            if (mFixedPhonetics.Contains(mSpeechReco)) {
                BYOS.Instance.Toaster.Hide();
                SwitchGuardianMode(GuardianMode.FIXED);
            } else if (mMobilePhonetics.Contains(mSpeechReco)) {
                BYOS.Instance.Toaster.Hide();
                SwitchGuardianMode(GuardianMode.MOBILE);
            } else if (mQuitPhonetics.Contains(mSpeechReco)) {
                BYOS.Instance.Toaster.Hide();
                QuitApp();
            } else {
                Interaction.TextToSpeech.SayKey("notunderstand", true);
                Interaction.TextToSpeech.Silence(1000, true);
                Interaction.TextToSpeech.SayKey("askchoices", true);

                mSpeechReco = null;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);

            mSpeechReco = null;
            mListening = false;
            mHasDisplayChoices = false;
        }

        private void DisplayChoices()
        {
            BYOS.Instance.Toaster.Display<ChoiceToast>().With("Mode", new ButtonInfo[] {
                new ButtonInfo {
                    Label = Dictionary.GetString("fixed"),
                    OnClick = delegate() { SwitchGuardianMode(GuardianMode.FIXED); }
                },

                new ButtonInfo {
                    Label = Dictionary.GetString("mobile"),
                    OnClick = delegate() { SwitchGuardianMode(GuardianMode.MOBILE); }
                },

                new ButtonInfo { Label = Dictionary.GetString("quit"), OnClick = QuitApp },
            });
        }

        private void OnSpeechReco(string iVoiceInput)
        {
            Debug.Log("reco :" + iVoiceInput);
            mSpeechReco = iVoiceInput;

            Interaction.Mood.Set(MoodType.NEUTRAL);
            mListening = false;
        }

        private void SwitchGuardianMode(GuardianMode iMode)
        {
            mSpeechReco = null;
            GuardianData.Instance.Mode = iMode;
            Trigger("NextStep");
        }
    }
}