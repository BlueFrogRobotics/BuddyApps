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
    public class LoadingState : AStateMachineBehaviour
    {
        private float mTimer;
        private bool mHasSwitchedState;
        private TextToSpeech mTTS;


        public override void Start()
        {

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTTS = BYOS.Instance.Interaction.TextToSpeech;
            mTimer = 0.0f;
            mHasSwitchedState = false;
            BYOS.Instance.WebService.EMailSender.enabled = true;
            mTTS.Silence(2000);
            
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            Debug.Log("has finished talking: " + mTTS.HasFinishedTalking + " is speaking: " + mTTS.IsSpeaking);
            if(mTimer>95.0f && !mHasSwitchedState)
            {
                mHasSwitchedState = true;
                Trigger("NextStep");
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            ResetTrigger("NextStep");
        }

        
    }
}