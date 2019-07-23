using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;

namespace BuddyApp.OutOfBox
{
    public class Base : AStateMachineBehaviour
    {

        public override void Start()
        {
            base.Start();

            mBehaviour = GetComponent<OutOfBoxBehaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("Base"));
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Behaviour.ResetMood();
            Trigger(mBehaviour.PhaseDropDown.captionText.text);
        }
    }
}

