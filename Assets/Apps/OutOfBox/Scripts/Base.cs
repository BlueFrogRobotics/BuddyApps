using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class Base : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Behaviour.ResetMood();
            switch(OutOfBoxData.Instance.Phase)
            {
                case OutOfBoxData.PhaseId.PhaseOne:
                    Trigger("Awakening");
                    break;
                case OutOfBoxData.PhaseId.PhaseTwo:
                    Trigger("Detect");
                    break;
                case OutOfBoxData.PhaseId.PhaseThree:
                    Trigger("SoundLoc");
                    break;
                case OutOfBoxData.PhaseId.PhaseFour:
                    Trigger("Movement");
                    break;
                case OutOfBoxData.PhaseId.PhaseFive:
                    Trigger("TouchAndCaress");
                    break;
                case OutOfBoxData.PhaseId.PhaseSix:
                    Trigger("Trigger");
                    break;
                default:
                    Trigger("Awakening");
                    break;
            }
        }
    }
}

