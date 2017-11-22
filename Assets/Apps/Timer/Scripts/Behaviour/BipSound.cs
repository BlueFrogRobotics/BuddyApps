using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using Buddy.UI;

namespace BuddyApp.Timer
{

    public class BipSound : AStateMachineBehaviour
    {

        // Use this for initialization
        public override void Start()
        {
            BYOS.Instance.Primitive.Speaker.FX.Load(
                   BYOS.Instance.Resources.Load<AudioClip>("airhorn"), 0
               );
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            BYOS.Instance.Primitive.Speaker.FX.Play(0);
            Primitive.Speaker.ChangeVolume(15);
            BYOS.Instance.Primitive.Speaker.FX.Loop = true;
            BYOS.Instance.Primitive.Speaker.FX.Play(0);
            Interaction.Mood.Set(MoodType.SCARED);
            BYOS.Instance.Toaster.Display<BinaryQuestionToast>().With("Stop Alarm",StopAlarm,StopAlarm);
        }

        private void StopAlarm()
        {
            BYOS.Instance.Primitive.Speaker.FX.Loop = false;
            BYOS.Instance.Primitive.Speaker.FX.Stop();
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Trigger("Reset");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}