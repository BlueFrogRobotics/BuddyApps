using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.BuddyLab
{
    public sealed class BLMenu : AStateMachineBehaviour
    {
        private bool mIsListening = false;
        private int mTimeOut;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Toaster.Display<VerticalListToast>().With( (iBuilder) =>
            {
                TVerticalListBox lBox1 = iBuilder.CreateBox();
                lBox1.SetLabel(Buddy.Resources.GetString("menusimple"));
                lBox1.LeftButton.Hide();
                lBox1.SetCenteredLabel(true);
                lBox1.OnClick.Add(() => {
                    Trigger("MakeProject");
                    Buddy.GUI.Toaster.Hide();
                });
                TVerticalListBox lBox2 = iBuilder.CreateBox();
                lBox2.SetLabel(Buddy.Resources.GetString("menuopen"));
                lBox2.LeftButton.Hide();
                lBox2.SetCenteredLabel(true);
                lBox2.OnClick.Add(() => {
                    Trigger("StartOpen");
                    Buddy.GUI.Toaster.Hide();
                });
                //TVerticalListBox lBox3 = iBuilder.CreateBox();
                //lBox3.LeftButton.Hide();
                //lBox3.SetLabel(Buddy.Resources.GetString("menututo"));
                //lBox3.OnClick.Add(() => {
                //    Trigger("StartTuto");
                //    Buddy.GUI.Toaster.Hide();
                //});
            }
                
            );
            mTimeOut = 0;
            Buddy.Vocal.OnEndListening.Add(OnListening);
            Buddy.Vocal.Listen("buddylab", SpeechRecognitionMode.GRAMMAR_ONLY);
            mIsListening = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(!mIsListening && mTimeOut<4) {
                mIsListening = true;
                Buddy.Vocal.Listen("buddylab", SpeechRecognitionMode.GRAMMAR_ONLY);
            }
            else if(mTimeOut>=8) {
                QuitApp();
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.OnEndListening.Remove(OnListening);
        }

        private void OnListening(SpeechInput iSpeech)
        {
            mTimeOut++;
            Debug.Log("[LAB] speech: " + iSpeech.Utterance + " confidence: " + iSpeech.Confidence);
            if (!string.IsNullOrEmpty(iSpeech.Rule) && iSpeech.Confidence>5000) {
                if (iSpeech.Rule.Contains("menusimple")) {
                    Trigger("MakeProject");
                    Buddy.GUI.Toaster.Hide();
                } else if (iSpeech.Rule.Contains("menuopen")) {
                    Trigger("StartOpen");
                    Buddy.GUI.Toaster.Hide();
                } 
                //else {
                //    Buddy.Vocal.Listen("buddylab", SpeechRecognitionMode.GRAMMAR_ONLY);
                //}
                //else if (iSpeech.Rule.Contains("menututo")) {
                 //    Trigger("StartTuto");
                 //    Buddy.GUI.Toaster.Hide();
                 //} 
            } 
            //else {
            //    Buddy.Vocal.Listen(SpeechRecognitionMode.GRAMMAR_ONLY);
            //}
            mIsListening = false;
        }
    }
}

