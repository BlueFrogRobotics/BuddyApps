using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.OutOfBoxV3
{
    public class Awakening : AStateMachineBehaviour
    {
        private bool mWokeUp;

        public override void Start()
        {
            base.Start();

            mBehaviour = GetComponent<OutOfBoxV3Behaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("TRANSITION"));
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mWokeUp = false;

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

            Buddy.Vocal.OnTrigger.AddP(WakeUp);
            OutOfBoxUtils.DebugColor("AWAKENING : ", "blue");
            Buddy.Actuators.Head.No.ResetPosition();
            Buddy.Actuators.Head.Yes.SetPosition(-9F, 45F , (iPos) =>
            {
                // Asleep
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.FALL_ASLEEP, false);
            });
            
        }

        private bool WakeUp(SpeechHotword HotWord = null)
        {
            mWokeUp = true;
            Buddy.Actuators.Head.Yes.SetPosition(5F, 45F);

            Buddy.Behaviour.Face.PlayEvent(FacialEvent.AWAKE, null, (iFacialEvent) =>
            {
                Buddy.Vocal.SayKey("awakegrumpy", (iSpeechOutput) =>
                {
                    // TODO Play BI grumpy
                    Buddy.Cognitive.InternalState.AddCumulative(new EmotionalEvent(-6, -6, "stoppednap", "STOPPED_NAP", EmotionalEventType.UNFULFILLED_DESIRE, InternalMood.GRUMPY));                    
                });
            });
            return false;
        }


        private void OnSkinClicked()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(2, 1, "othertouch", "TOUCH_FACE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
            RobotTouched();
        }

        private void OnMouthClicked()
        {
            RobotTouched();
        }

        private void OnRightEyeClicked()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
            new EmotionalEvent(-3, 1, "eyepoke", "POKE_EYE", EmotionalEventType.INTERACTION, InternalMood.ANGRY));
            RobotTouched();
        }

        private void OnLeftEyeClicked()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
            new EmotionalEvent(-3, 1, "eyepoke", "POKE_EYE", EmotionalEventType.INTERACTION, InternalMood.ANGRY));
            RobotTouched();
        }

        private void OnTouchHeart()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(3, -2, "touchheart", "TOUCH_HEART", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
            RobotTouched();
        }

        private void OnTouchRightShoulder()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(1, 0, "touchshoulder", "TOUCH_SHOULDER", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
            RobotTouched();
        }

        private void OnTouchLeftShoulder()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(1, 0, "touchshoulder", "TOUCH_SHOULDER", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
            RobotTouched();
        }

        private void OnTouchRightHead()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(2, -2, "touchhead", "TOUCH_HEAD", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
            RobotTouched();
        }

        private void OnTouchLeftHead()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(2, -2, "touchhead", "TOUCH_HEAD", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
            RobotTouched();
        }

        private void OnTouchBackHead()
        {
            Buddy.Cognitive.InternalState.AddCumulative(
                new EmotionalEvent(2, -2, "touchhead", "TOUCH_HEAD", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
            RobotTouched();
        }

        private void RobotTouched()
        {
            if(!mWokeUp)
                WakeUp();
            else if(Buddy.Cognitive.InternalState.Pleasure > 0)
                Buddy.Vocal.SayKey("awakefeelbetter", (iSpeechOutput) => {
                    mBehaviour.PhaseDropDown.value = 1;
                });
        }
    }
}

