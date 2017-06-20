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
            mFixedPhonetics = new List<string>(mDictionary.GetPhoneticStrings("fixed"));
            mMobilePhonetics = new List<string>(mDictionary.GetPhoneticStrings("mobile"));
            mQuitPhonetics = new List<string>(mDictionary.GetPhoneticStrings("quit"));
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            AAppActivity.UnlockScreen();

            mTTS.SayKey("askchoices");

            mSTT.OnBestRecognition.Clear();
            mSTT.OnBestRecognition.Add(OnSpeechReco);
            mTimer = 0.0f;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if(mTimer>6.0f)
            {
                mMood.Set(MoodType.NEUTRAL);
                mListening = false;
                mTimer = 0.0f;
                mSpeechReco = null;
            }

            if (!mTTS.HasFinishedTalking || mListening)
                return;

            if (!mHasDisplayChoices) {
                DisplayChoices();
                mHasDisplayChoices = true;
                return;
            }

            if (string.IsNullOrEmpty(mSpeechReco)) {
                mSTT.Request();

                mMood.Set(MoodType.LISTENING);
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
                mTTS.SayKey("notunderstand", true);
                mTTS.Silence(1000, true);
                mTTS.SayKey("askchoices", true);

                mSpeechReco = null;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMood.Set(MoodType.NEUTRAL);

            mSpeechReco = null;
            mListening = false;
            mHasDisplayChoices = false;
        }

        private void DisplayChoices()
        {
            BYOS.Instance.Toaster.Display<ChoiceToast>().With("", new ButtonInfo[] {
                new ButtonInfo {
                    Label = mDictionary.GetString("fixed"),
                    OnClick = delegate() { SwitchGuardianMode(GuardianMode.FIXED); }
                },

                new ButtonInfo {
                    Label = mDictionary.GetString("mobile"),
                    OnClick = delegate() { SwitchGuardianMode(GuardianMode.MOBILE); }
                },

                new ButtonInfo { Label = mDictionary.GetString("quit"), OnClick = QuitApp },
            });
        }

        private void OnSpeechReco(string iVoiceInput)
        {
            Debug.Log("reco :"+iVoiceInput);
            mSpeechReco = iVoiceInput;

            mMood.Set(MoodType.NEUTRAL);
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