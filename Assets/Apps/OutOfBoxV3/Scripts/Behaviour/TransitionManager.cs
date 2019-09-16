using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;

namespace BuddyApp.OutOfBoxV3
{
    public class TransitionManager : AStateMachineBehaviour
    {

        public override void Start()
        {
            base.Start();

            mBehaviour = GetComponent<OutOfBoxV3Behaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("TRANSITION"));
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            OutOfBoxUtilsVThree.DebugColor("TRANSITION MANAGER : " + mBehaviour.PhaseDropDown.captionText.text.ToUpper(), "blue");
            Buddy.Navigation.Stop();
            Buddy.Actuators.Wheels.Stop();
            Buddy.Actuators.Head.Stop();
            Buddy.Behaviour.Stop();
            Buddy.Behaviour.ResetMood();
            Buddy.Vocal.StopAndClear();
            
            Trigger(mBehaviour.PhaseDropDown.captionText.text.ToUpper());
        }
    }
}

