using BlueQuark;

using UnityEngine;

using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public class LaunchApplicationState : AStateMachineBehaviour
    {
        private readonly string STR_DEFAULT_SPRITE = "os_icon_photo";//"os_atlas_ui_photo_big";

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On State Enter...");

            // Define vocal listening
            Buddy.Vocal.EnableTrigger = true; // Active auto Okay Buddy
            Buddy.Vocal.ListenOnTrigger = true; // Active auto listen after Okay Buddy

            string[] grammars = { "grammar", "gallery" };
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters();
            Buddy.Vocal.DefaultInputParameters.Grammars = grammars;
            Buddy.Vocal.DefaultInputParameters.RecognitionThreshold = 6000; // More accurate recognition for "sure" / "share", etc
            
            InitializeHeader();
            InitializeSlides();
            
            Trigger("TRIGGER_GALLERY_UPLOADED");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");
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
            /// Set default sprite
            PictureToast defaultSlide = slider.SetDefaultSlide<PictureToast>();
            defaultSlide.With(Buddy.Resources.Get<Sprite>(STR_DEFAULT_SPRITE));

            if (null == Buddy.Resources.Get<Sprite>(STR_DEFAULT_SPRITE))
                ExtLog.E(ExtLogModule.APP, typeof(GalleryActivity), LogStatus.SUCCESS, LogInfo.LOADING, "DEFAULT SPRITE IS NULL");

            //
            /// Initializing slider with image sorted
            for (int i = 0; i < photoManager.GetCount(); ++i)
            {
                PictureToast slide;
                if (photoManager.GetFirstSlideIndex() == i) // Last photo is first displayed
                {
                    slide = slider.AddFirstDisplayedSlide<PictureToast>();
                }
                else
                {
                    slide = slider.AddSlide<PictureToast>();
                }

                //
                /// Setup slide with photo
                slide.With(photoManager.GetPhotoByIndex(i).GetPhotoFullpath());// ToSprite());
                photoManager.GetPhotoByIndex(i).SetSlide(ref slide);
            }
        }
    }
}
