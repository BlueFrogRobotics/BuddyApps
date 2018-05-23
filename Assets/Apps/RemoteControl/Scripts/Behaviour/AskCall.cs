using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RemoteControl
{
    public class AskCall : AStateMachineBehaviour
    {
        private bool mListening;
        private string mSpeechReco;

        [SerializeField]
        private string option1Key;

        [SerializeField]
        private string option1Trigger;

        [SerializeField]
        private string option2Key;

        [SerializeField]
        private string option2Trigger;

        [SerializeField]
        private string questionKey;

        [SerializeField]
        private string QuitTrigger;

        [SerializeField]
        private Sprite spriteCall;

        private int mError;
        private bool mQuit;
        private bool mHasInitializedRemote;

        private RemoteControlBehaviour mRemoteControlBehaviour;

        public override void Start()
        {
            mRemoteControlBehaviour = GetComponent<RemoteControlBehaviour>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Ask new request");
            mError = 0;
            mListening = false;
            mSpeechReco = "";

            mQuit = false;
            mHasInitializedRemote = false;
            //Interaction.TextToSpeech.SayKey(questionKey);
            StartCoroutine(ActivateDisplay());
            
            //Dictionary.GetString(questionKey)
            //Sprite[] lSpriteTab = new Sprite[1];
            //lSpriteTab[0] = spriteCall;
            //List<int> lSelectedImage = new List<int>();
            //Toaster.Display<PictureToast>().With(lSpriteTab, PressedYes, PressedNo, ref lSelectedImage);

        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Interaction.TextToSpeech.HasFinishedTalking || mListening || !mHasInitializedRemote)
                return;
            else if (mQuit)
            {
                QuitApp();
            }

            if (string.IsNullOrEmpty(mSpeechReco))
            {
                Interaction.SpeechToText.Request();
                mListening = true;

                Interaction.Mood.Set(MoodType.LISTENING);
                return;
            }
            Debug.Log("chips");
            if (ContainsOneOf(mSpeechReco, Dictionary.GetPhoneticStrings(option2Key)))
            {
                //Toaster.Hide();
                Debug.Log("option2");
                Option2();
            }
            else if (ContainsOneOf(mSpeechReco, Dictionary.GetPhoneticStrings(option1Key)))
            {
                //Toaster.Hide();
                Debug.Log("option1");
                Option1();
            }
            //else if (ContainsOneOf(mSpeechReco, Dictionary.GetPhoneticStrings("quit")))
            //{
            //    Toaster.Hide();
            //    Option1();
            //}
            //else
            //{
            //    Interaction.TextToSpeech.SayKey("notunderstandyesno", true);
            //    mError++;
            //    if (mError > 2)
            //    {
            //        QuitApp();
            //    }
            //    else
            //    {
            //        Interaction.TextToSpeech.Silence(1000, true);
            //        Interaction.TextToSpeech.SayKey(questionKey, true);
            //    }

            //    mSpeechReco = "";
            //}

        }


        private void OnSpeechReco(string iVoiceInput)
        {
            Debug.Log("reco vocale: " + iVoiceInput);
            Interaction.Mood.Set(MoodType.NEUTRAL);

            mSpeechReco = iVoiceInput;
            mListening = false;
        }

        private void PressedYes()
        {
            Toaster.Hide();
            Primitive.Speaker.FX.Play(FXSound.BEEP_1);
            Option2();
        }

        private void PressedNo()
        {
            Toaster.Hide();
            Primitive.Speaker.FX.Play(FXSound.BEEP_1);
            Option1();
        }

        private void Option2()
        {
            mRemoteControlBehaviour.CloseApp();
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Trigger(option2Trigger);
        }

        private void Option1()
        {
            mRemoteControlBehaviour.LaunchCall();
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Trigger(option1Trigger);
            //Interaction.TextToSpeech.SayKey("bye");
            //mQuit = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Toaster.Hide();

            //Interaction.TextToSpeech.Say("ok");
            Interaction.Mood.Set(MoodType.NEUTRAL);
            mSpeechReco = "";
            mListening = false;
        }

        private IEnumerator ActivateDisplay()
        {
            yield return mRemoteControlBehaviour.Call();
            mHasInitializedRemote = true;
            Interaction.SpeechToText.OnBestRecognition.Clear();
            Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechReco);
        }

        private bool ContainsOneOf(string iSpeech, string[] iListSpeech)
        {
            iSpeech = iSpeech.ToLower();
            for (int i = 0; i < iListSpeech.Length; ++i)
            {
                string[] words = iListSpeech[i].Split(' ');
                if (words.Length < 2)
                {
                    words = iSpeech.Split(' ');
                    foreach (string word in words)
                    {
                        if (word == iListSpeech[i].ToLower())
                        {
                            return true;
                        }
                    }
                }
                else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
                    return true;
            }
            return false;
        }

    }
}