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
        private LifeManager mLifeManager;
        //private int mLife = 2;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
            mLifeManager = GetComponent<LifeManager>();
        }

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
            mRLGLBehaviour.IsPlaying = false;

        }

        private IEnumerator Defeat()
        {
            Interaction.Mood.Set(MoodType.SAD);
            yield return SayKeyAndWait("youmoved");
            
            yield return SayKeyAndWait("lookphoto");
           //if(File.Exists(BYOS.Instance.Resources.GetPathToRaw("defeat")))
           // {
           //     Debug.Log("CA EXISTE LOL");
           // }
           //else
           // {
           //     Debug.Log("CA EXISTE PAS LOL");
           // } 
            //Texture2D lTexture = mRLGLBehaviour.PictureMoving;//mCam.FrameTexture2D;
            Texture2D lTexture = LoadPNG(BYOS.Instance.Resources.GetPathToRaw("defeat"));
            //Texture2D lTexture = LoadPNG("C:/Users/Walid/Pictures/buddy.png");
            //Utils.Texture2DToMat(lTexture);

            //if (lTexture != null)
            //{
            //    Debug.Log("DEFEAT EXIST");
            //}
            //else
            //    Debug.Log("DEFEAT NEXISTE PAS");

            Toaster.Display<PictureToast>().With(Dictionary.GetString("lookphoto"), Sprite.Create(lTexture, new Rect(0, 0, lTexture.width, lTexture.height), new Vector2(0.5f, 0.5f)));
            
            yield return new WaitForSeconds(3);
            Interaction.Mood.Set(MoodType.HAPPY);
            Toaster.Hide();
            mLifeManager.LoseLife();
            if (mLifeManager.Life > 0)
            {
                yield return SayAndWait(Dictionary.GetRandomString("loselevel") + " " + mLifeManager.Life + " " + Dictionary.GetRandomString("life"));
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