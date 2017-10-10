using UnityEngine;
using Buddy;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State that init that activate the detections chosen by hte user and pass to the next mode state
    /// </summary>
    public class LandingState : AStateMachineBehaviour
    {

        public override void Start()
        {
            Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Header.DisplayParametersButton = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (GuardianData.Instance.FirstRun)
                Trigger("Parameter");
            else
                Trigger("NextStep");
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }


    }
}