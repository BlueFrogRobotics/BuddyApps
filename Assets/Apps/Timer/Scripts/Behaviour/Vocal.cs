using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.Timer
{

    public class Vocal : AStateMachineBehaviour
    {

        // Use this for initialization
        public override void Start()
        {

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER LISTEN test");
            Interaction.VocalManager.OnEndReco = GetAnswer;
            Interaction.VocalManager.OnError = NoAnswer;
            Interaction.VocalManager.StartInstantReco();

        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("EXIT LISTEN");
        }

        private void GetAnswer(string iAnswer)
        {
            Utils.LogI(LogContext.APP, "GOT AN ANSWER: " + iAnswer);
            TimerData.Instance.VocalRequest = iAnswer.ToLower();
            Trigger("ParseTime");
        }

        private void NoAnswer(STTError iError)
        {
            Utils.LogI(LogContext.APP, "VM error");
            Debug.Log("GOT NO ANSWER");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
