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
            BYOS.Instance.Header.DisplayParameters = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            Debug.Log("coucou");

            if (GuardianData.Instance.FirstRun)
            {
                Debug.Log("coucou2");
                Trigger("Parameter");
            }
            else
            {
                Debug.Log("coucou3");
                Trigger("NextStep");
            }
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }


    }
}