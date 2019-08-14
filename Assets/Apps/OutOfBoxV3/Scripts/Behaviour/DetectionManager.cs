using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.OutOfBoxV3
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class DetectionManager : MonoBehaviour
    {
        private Animator mAnimator;
        private bool mPause;
        private bool mReact;

        private void Start()
        {
            mAnimator = GetComponent<Animator>();
        }


        private void OnVocalTrigger(SpeechHotword obj)
        {
            if (mPause) {
                mPause = false;
                mReact = true;
                mAnimator.SetTrigger("TRANSITION");
            }
        }


        private void OnMouthClicked()
        {
            Buddy.Navigation.Stop();
            Buddy.Actuators.Wheels.Stop();
            Buddy.Actuators.Head.Stop();
            Buddy.Behaviour.Stop();
            Buddy.Behaviour.ResetMood();
            Buddy.Vocal.StopAndClear();
            mAnimator.SetTrigger("PAUSE");
            mReact = true;
            mPause = true;
            Buddy.Vocal.EnableTrigger = true;
        }

        private void OnSkinClicked()
        {
            //if(mReact)
                //TODO
                
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(2, 1, "othertouch", "TOUCH_FACE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
        }

        private void OnRightEyeClicked()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
            new EmotionalEvent(-3, 1, "eyepoke", "POKE_EYE", EmotionalEventType.INTERACTION, InternalMood.ANGRY));
        }

        private void OnLeftEyeClicked()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
            new EmotionalEvent(-3, 1, "eyepoke", "POKE_EYE", EmotionalEventType.INTERACTION, InternalMood.ANGRY));
        }


        private void OnTouchHeart()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(3, -2, "touchheart", "TOUCH_HEART", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
        }

        private void OnTouchRightShoulder()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(1, 0, "touchshoulder", "TOUCH_SHOULDER", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
        }

        private void OnTouchLeftShoulder()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(1, 0, "touchshoulder", "TOUCH_SHOULDER", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
        }

        private void OnTouchRightHead()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(2, -2, "touchhead", "TOUCH_HEAD", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
        }

        private void OnTouchLeftHead()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(2, -2, "touchhead", "TOUCH_HEAD", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
        }

        private void OnTouchBackHead()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(2, -2, "touchhead", "TOUCH_HEAD", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
        }

        /// <summary>
        /// Subscribe to the detectors callbacks
        /// </summary>
        public void LinkDetectorsEvents()
        {
            Buddy.Behaviour.Face.OnTouchLeftEye.Add(OnLeftEyeClicked);
            Buddy.Behaviour.Face.OnTouchRightEye.Add(OnRightEyeClicked);
            Buddy.Behaviour.Face.OnTouchMouth.Add(OnMouthClicked);
            Buddy.Behaviour.Face.OnTouchSkin.Add(OnSkinClicked);

            Buddy.Sensors.TouchSensors.BackHead.OnTouch.Add(OnTouchBackHead);
            Buddy.Sensors.TouchSensors.LeftHead.OnTouch.Add(OnTouchLeftHead);
            Buddy.Sensors.TouchSensors.RightHead.OnTouch.Add(OnTouchRightHead);
            Buddy.Sensors.TouchSensors.LeftShoulder.OnTouch.Add(OnTouchLeftShoulder);
            Buddy.Sensors.TouchSensors.RightShoulder.OnTouch.Add(OnTouchRightShoulder);
            Buddy.Sensors.TouchSensors.Heart.OnTouch.Add(OnTouchHeart);

            Buddy.Vocal.OnTrigger.Add(OnVocalTrigger);
        }

    }
}