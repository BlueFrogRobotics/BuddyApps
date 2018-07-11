using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.TakePose
{
    public class RedoPose : AStateMachineBehaviour
    {
        private bool mListening;
        private string mSpeechReco;

        private List<string> mAcceptPhonetics;
        private List<string> mRefusePhonetics;
        private List<string> mAnotherPhonetics;
        private List<string> mQuitPhonetics;

        public override void Start()
        {
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mListening = false;
            mSpeechReco = "";

            mAcceptPhonetics = new List<string>(Buddy.Resources.GetPhoneticStrings("accept"));
            mRefusePhonetics = new List<string>(Buddy.Resources.GetPhoneticStrings("refuse"));
            mAnotherPhonetics = new List<string>(Buddy.Resources.GetPhoneticStrings("another"));
            mQuitPhonetics = new List<string>(Buddy.Resources.GetPhoneticStrings("quit"));

            Debug.Log("REDOPOSE : " + Buddy.Resources.GetString("redopose"));
            Buddy.Vocal.SayKey("redopose");

            Buddy.Vocal.OnEndListening.Add((iInput) => { OnSpeechReco(iInput.Utterance); });
            
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {

                iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetString("redopose"));
            },

                () => { PressedNo(); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("no"),

                () => { PressedYes(); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("yes")

            );
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Buddy.Vocal.IsSpeaking || mListening)
                return;

            if (string.IsNullOrEmpty(mSpeechReco)) {
                Buddy.Vocal.Listen();
                //Interaction.SpeechToText.Request();
                mListening = true;

                Buddy.Behaviour.Mood.Set(FacialExpression.LISTENING);
                return;
            }

            if (mAcceptPhonetics.Contains(mSpeechReco) || mAnotherPhonetics.Contains(mSpeechReco)) {
                Buddy.GUI.Toaster.Hide();
                RedoTakePose();
            } else if (mRefusePhonetics.Contains(mSpeechReco) || mQuitPhonetics.Contains(mSpeechReco)) {
                Buddy.GUI.Toaster.Hide();
                ExitTakePose();
            } else {
                Buddy.Vocal.SayKey("notunderstand", true);
                //Buddy.Vocal.Silence(1000, true);
                Buddy.Vocal.Say("[1000]");
                Buddy.Vocal.SayKey("yesorno", true);
                Buddy.Vocal.Say("[1000]");
                //Buddy.Vocal.Silence(1000, true);
                Buddy.Vocal.SayKey("redopose", true);

                mSpeechReco = "";
            }

        }

        private void PressedYes()
        {
            Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
            RedoTakePose(); 
        }

        private void PressedNo()
        {
            Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
            ExitTakePose();
        }

        private void OnSpeechReco(string iVoiceInput)
        {
            Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
            mSpeechReco = iVoiceInput;
            mListening = false;
        }

        private void RedoTakePose()
        {
            Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
            Trigger("Pose");
        }

        private void ExitTakePose()
        {
            Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
            Buddy.Vocal.SayKey("noredo");
            Trigger("Exit");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
            mSpeechReco = "";
            mListening = false;
        }

    }
}