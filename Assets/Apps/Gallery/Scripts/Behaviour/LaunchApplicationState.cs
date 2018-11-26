using BlueQuark;

using UnityEngine;

using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public class LaunchApplicationState : AStateMachineBehaviour
    {
        private readonly string STR_DEFAULT_SPRITE = "os_icon_photo_big";

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On State Enter...");

            // Define vocal listening
            Buddy.Vocal.EnableTrigger = true; // Active auto Okay Buddy
            Buddy.Vocal.ListenOnTrigger = true; // Active auto listen after Okay Buddy

            string[] grammars = { "grammar", "gallery" };
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters {
                Grammars = grammars,
                RecognitionThreshold = 6000 // More accurate recognition for "sure" / "share", etc
            };

            InitializeHeader();
            InitializeSlides();
            
            Trigger("TRIGGER_GALLERY_UPLOADED");
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
            PhotoManager lPhotoManager = PhotoManager.GetInstance();
            lPhotoManager.SetSlideSet(Buddy.GUI.Toaster.DisplaySlide());
            SlideSet lSlider = lPhotoManager.GetSlideSet();

            //
            /// Set default sprite
            lSlider.SetDefaultSlide<PictureToast>().With(Buddy.Resources.Get<Sprite>(STR_DEFAULT_SPRITE));

            if (null == Buddy.Resources.Get<Sprite>(STR_DEFAULT_SPRITE))
                ExtLog.E(ExtLogModule.APP, typeof(GalleryActivity), LogStatus.SUCCESS, LogInfo.LOADING, "DEFAULT SPRITE IS NULL");

            //
            /// Initializing slider with image sorted
            for (int i = 0; i < lPhotoManager.GetCount(); ++i)
            {
                PictureToast lSlide;
                if (lPhotoManager.GetFirstSlideIndex() == i) // Last photo is first displayed
                {
                    lSlide = lSlider.AddFirstDisplayedSlide<PictureToast>();
                }
                else
                {
                    lSlide = lSlider.AddSlide<PictureToast>();
                }

                //
                /// Setup slide with photo
                lSlide.With(lPhotoManager.GetPhotoByIndex(i).GetPhotoFullpath());// ToSprite());
                lPhotoManager.GetPhotoByIndex(i).SetSlide(ref lSlide);
            }
        }
    }
}
