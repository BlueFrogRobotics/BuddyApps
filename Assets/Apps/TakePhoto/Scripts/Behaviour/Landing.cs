using UnityEngine;
using BlueQuark;


namespace BuddyApp.TakePhoto
{
    public sealed class Landing : AStateMachineBehaviour
    {
        private float mTimer;

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0F;
            ActionStartApp();
            //Set the recognition threshold for the app
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters();
            Buddy.Vocal.DefaultInputParameters.RecognitionThreshold = 6000;

        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            //We also set a timer if we have a bug with the Buddy.Vocal.IsBusy because we still work on the vocal and we want to avoid being blocked here.
            if(!Buddy.Vocal.IsBusy || mTimer > 6F)
                Trigger("Photo");
        }

        private void ActionStartApp()
        {
            //Buddy.Vocal.SayKey("movehands");
            Buddy.Actuators.Head.SetPosition(15F, 0F);
        }
    }
}