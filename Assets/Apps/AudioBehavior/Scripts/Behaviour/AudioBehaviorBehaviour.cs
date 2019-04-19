using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.AudioBehavior
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class AudioBehaviorBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private AudioBehaviorData mAppData;
        private float mLastSoundLocTime;
        private int mSoundLoc;
        private int mCount;
        private int mLastAverageAmbiant;
        private int mAverageAmbiant;
        private float mLastTime;

        void Start()
        {
            mCount = 0;
            mAverageAmbiant = 0;

            /*
			* You can setup your App activity here.
			*/
            AudioBehaviorActivity.Init(null);

            Buddy.Vocal.EnableTrigger = true;
            Buddy.Sensors.Microphones.EnableEchoCancellation = false;
            Buddy.Sensors.Microphones.EnableSoundLocalization = true;
            //Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(
            //    Buddy.Sensors.Microphones.SoundLocalizationParameters.Algorithm,
            //    Buddy.Sensors.Microphones.SoundLocalizationParameters.Resolution,
            //    40);
            Buddy.Vocal.OnCompleteTrigger.Add(BuddyTrigged);
            Buddy.Actuators.Head.Yes.SetPosition(-9.9F);

        }

        private void BuddyTrigged(SpeechHotword iHotWord)
        {
            Buddy.Vocal.Stop();
            Debug.LogWarning("my angle " + mSoundLoc);
            Debug.LogWarning("os angle " + iHotWord.SoundLocalization);
            Debug.LogWarning("reco score " + iHotWord.RecognitionScore);
            Debug.LogWarning("time " + DateTime.Now.ToString());

            if (Time.time - mLastSoundLocTime < 1F)
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(-mSoundLoc, 80F, OnEndMove);
            else
                Debug.LogWarning("No motion bcs last sound loc too old: " + Time.time +" " + mLastSoundLocTime + " = " + (Time.time - mLastSoundLocTime));

        }

        private void OnEndMove()
        {
            Debug.LogWarning("motion end ");
            Buddy.Vocal.Listen(EndListening, SpeechRecognitionMode.GRAMMAR_ONLY);
        }

        private void EndListening(SpeechInput iObj)
        {
            Buddy.Sensors.Microphones.EnableSoundLocalization = false;
            Buddy.Sensors.Microphones.EnableEchoCancellation = true;
            Buddy.Vocal.Say("Je parle", EndSpeaking);
        }

        private void EndSpeaking(SpeechOutput iObj)
        {
            Buddy.Sensors.Microphones.EnableEchoCancellation = false;
            Buddy.Sensors.Microphones.EnableSoundLocalization = true;
        }

        private void Update()
        {
            if (mCount == 60) {
                mLastAverageAmbiant = mAverageAmbiant / 60;
                Debug.LogWarning("last ambiant average" + mLastAverageAmbiant);
                mAverageAmbiant = 0;
                mCount = 0;
                // TODO: change soundloc threshold with ambiant: 55 -> 40, 35 -> 30, 75 -> 55
                //lThresh = (mLastAverageAmbiant - 60) / 2 + 40;
            } else if (Time.time - mLastTime > 1F) {
                mLastTime = Time.time;
                mAverageAmbiant += Buddy.Sensors.Microphones.AmbiantSound;
                //Debug.LogWarning("last ambiant " + Buddy.Sensors.Microphones.AmbiantSound);
                mCount++;
            }


            if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION) {
                Debug.LogWarning("New sound loc "  + Buddy.Sensors.Microphones.SoundLocalization + "  " + DateTime.Now.ToString());
                mLastSoundLocTime = Time.time;
                mSoundLoc = Buddy.Sensors.Microphones.SoundLocalization;
            }
        }
    }
}