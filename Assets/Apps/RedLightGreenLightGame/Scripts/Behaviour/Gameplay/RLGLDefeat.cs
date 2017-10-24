﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLDefeat : AStateMachineBehaviour
    {
        private RGBCam mCam;
        private int mLife = 1;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Interaction.TextToSpeech.Silence(1000);
            mCam = Primitive.RGBCam;
            StartCoroutine(Defeat());
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

        private IEnumerator Defeat()
        {
            Interaction.Mood.Set(MoodType.SAD);
            yield return SayKeyAndWait("youmoved");
            
            yield return SayKeyAndWait("lookphoto");

            Texture2D lTexture = mCam.FrameTexture2D;
            //Texture2D lTexture = LoadPNG("C:/Users/Walid/Pictures/buddy.png");
            //Utils.Texture2DToMat(lTexture);
            
            Toaster.Display<PictureToast>().With(Dictionary.GetString("lookphoto"), Sprite.Create(lTexture, new Rect(0, 0, lTexture.width, lTexture.height), new Vector2(0.5f, 0.5f)));
            
            yield return new WaitForSeconds(3);
            Interaction.Mood.Set(MoodType.HAPPY);
            Toaster.Hide();
            mLife--;
            if (mLife > 0)
            {
                yield return SayAndWait(Dictionary.GetRandomString("loselevel") + " " + mLife + " " + Dictionary.GetRandomString("life"));
                yield return new WaitForSeconds(3);
                Trigger("Repositionning");
            }
            else
                Trigger("GameOver");
        }

        public Texture2D LoadPNG(string filePath)
        {

            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            return tex;
        }

    }

   

}
