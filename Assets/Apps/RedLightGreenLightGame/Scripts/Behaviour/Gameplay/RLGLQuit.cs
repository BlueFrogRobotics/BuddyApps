using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLQuit : AStateMachineBehaviour
    {
        private RGBCam mCam;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mCam = Primitive.RGBCam;
            StartCoroutine(Quit());

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        private IEnumerator Quit()
        {
            Texture2D lTexture = mCam.FrameTexture2D;
            yield return SayKeyAndWait("showphotos");
            Interaction.Mood.Set(MoodType.HAPPY);
            Toaster.Display<PictureToast>().With(Dictionary.GetString("lookphoto"), Sprite.Create(mCam.FrameTexture2D, new Rect(0, 0, lTexture.width, lTexture.height), new Vector2(0.5f, 0.5f)));
            yield return new WaitForSeconds(2);
            QuitApp();
        }

    }

}
