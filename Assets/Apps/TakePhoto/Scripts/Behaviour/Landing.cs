using UnityEngine;
using BlueQuark;


namespace BuddyApp.TakePhoto
{
    public sealed class Landing : AStateMachineBehaviour
    {

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			// Inhibit head movement, as Looking for someone doesn't work
            //Buddy.Actuators.Head.SetPosition(15F, 0F);

            //Set the recognition threshold for the app
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters();
            Buddy.Vocal.DefaultInputParameters.RecognitionThreshold = 6000;
            //Trigger("LookForUser");

            Trigger("Photo");

        }

        //public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        //{
        //}

        //private void ActionStartApp()
        //{
        //    //Buddy.Vocal.SayKey("movehands");
        //}
    }
}