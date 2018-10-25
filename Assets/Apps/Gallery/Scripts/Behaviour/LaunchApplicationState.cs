using BlueQuark;

using UnityEngine;

using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public class LaunchApplicationState : AStateMachineBehaviour
    {
        private readonly string STR_DEFAULT_SPRITE = "big-icon.png";

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // TODO: Check correct initialization when error control is available.

            // Define vocal listening
            Buddy.Vocal.EnableTrigger = true; // Active auto Okay Buddy
            Buddy.Vocal.ListenOnTrigger = true; // Active auto listen after Okay Buddy
            string[] grammars = { "app_grammar", "gallery" };
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters();
            Buddy.Vocal.DefaultInputParameters.Grammars = grammars;

            InitializeHeader();
            InitializeSlides();
            
            Trigger("TRIGGER_GALLERY_UPLOADED");
        }

        private void InitializeHeader()
        {
            Buddy.GUI.Header.DisplayParametersButton(false);
        }

        private void InitializeSlides()
        {
            //
            /// Initialize slide set
            PhotoManager photoManager = PhotoManager.GetInstance();
            photoManager.SetSlideSet(Buddy.GUI.Toaster.DisplaySlide());
            SlideSet slider = photoManager.GetSlideSet();

            //
            /// Create default sprite
            Texture2D spriteTexture = new Texture2D(980, 512);
            spriteTexture.LoadImage(File.ReadAllBytes(Buddy.Resources.GetSpritesFullPath(STR_DEFAULT_SPRITE)));
            spriteTexture.Apply();
            Sprite defaultSprite = Sprite.Create(spriteTexture, new UnityEngine.Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5F, 0.5F));

            //
            /// Set default sprite
            PictureToast defaultSlide = slider.SetDefaultSlide<PictureToast>();
            defaultSlide.With(defaultSprite);
            
            //
            /// Initializing slider with image sorted
            for (int i = 0; i < photoManager.GetCount(); i++)
            {
                PictureToast slide;
                if (photoManager.GetCount() - 1 == i) // Last photo is first displayed
                {
                    slide = slider.AddFirstDisplayedSlide<PictureToast>();
                }
                else
                {
                    slide = slider.AddSlide<PictureToast>();
                }

                //
                /// Setup slide with photo
                slide.With(photoManager.GetPhotoByIndex(i).ToSprite());
                photoManager.GetPhotoByIndex(i).SetSlide(ref slide);
            }
        }
        
        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}
