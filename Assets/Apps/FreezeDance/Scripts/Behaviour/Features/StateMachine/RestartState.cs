using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.FreezeDance
{
    public class RestartState : AStateMachineBehaviour
    {
        private MusicPlayer mMusicPlayer;
        //private bool mListening;
        //private float mTimer = 0.0f;
        //private bool mSwitchState = false;

        public override void Start()
        {
            mMusicPlayer = GetComponent<MusicPlayer>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Restart();
            //    Buddy.Vocal.SayKey("playagain");
            //    Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            //    mMusicPlayer.Restart();
            //    mListening = false;
            //    mSwitchState = false;
            //    mTimer = 0.0f;
            //    Buddy.Vocal.OnEndListening.Add(OnRecognition);
            //Toaster.Display<BinaryQuestionToast>().With(
            //    Buddy.Resources.GetString("playagain"),
            //    () => Restart(),
            //    () => QuitApp()
            //);
        }

        //public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        //{
        //    mTimer += Time.deltaTime;

        //    if (mTimer > 6.0f)
        //    {
        //        Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        //        mListening = false;
        //        mTimer = 0.0f;
        //    }

        //    if (Buddy.Vocal.IsBusy || mListening)
        //        return;

        //    if (!mListening && !mSwitchState)
        //    {
        //        Buddy.Behaviour.SetMood(Mood.LISTENING);
        //        Buddy.Vocal.Listen();
        //        mListening = true;
        //    }
        //}

        //public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        //{
        //    Buddy.Vocal.OnEndListening.Clear();
        //}

        private void Restart()
        {
            mMusicPlayer.Restart();
            Trigger("Start");
        }

        //private void OnRecognition(SpeechInput iSpeech)
        //{
        //    if (!string.IsNullOrEmpty(iSpeech.Utterance))
        //    {
        //        Buddy.Vocal.Listen();
        //        return;
        //    }
        //    if (Buddy.Resources.ContainsPhonetic(iSpeech.Utterance, "yes"))
        //    {
        //        Buddy.GUI.Toaster.Hide();
        //        Restart();
        //        mSwitchState = true;
        //    }
        //    else if (Buddy.Resources.ContainsPhonetic(iSpeech.Utterance, "no"))
        //    {
        //        Buddy.GUI.Toaster.Hide();
        //        QuitApp();
        //        mSwitchState = true;
        //    }
        //    mListening = false;
        //}
    }
}