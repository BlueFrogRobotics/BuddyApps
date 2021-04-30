using BlueQuark;

using UnityEngine;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

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

        private void DisplaySettingsToaster()
        {
            //Debug.LogWarning("Enter settings");
            // Store previous email adress to share
            string lPreviousValue = string.Empty;
            // Enqueue en ParameterToast request that will be display after all previous queued toasts
            Buddy.GUI.Dialoger.Display<ParameterToast>().With((iBuilder) => { // This callback will be called on Toast display
                TText lText = iBuilder.CreateWidget<TText>(); // Create a new text widget
                lText.SetLabel(Buddy.Resources.GetString("emailtoshare")); // Set the content of the widget
                /*TToggle lToggle = iBuilder.CreateWidget<TToggle>(); // Create a new toggle
                lToggle.SetLabel("my toggle"); // You can labeled it
                lToggle.ToggleValue = true; // The default value
                lToggle.OnToggle.Add((iVal) => { // Callback on each modification
                    Debug.Log("Toggle : " + iVal);
                });*/
                TTextField lField = iBuilder.CreateWidget<TTextField>(); // Create an input field

                lPreviousValue = GalleryData.Instance.mailshare;

                // if the adresse mail is already saved in the user params
                if (string.IsNullOrEmpty(GalleryData.Instance.mailshare))
                {
                    lField.SetPlaceHolder(Buddy.Resources.GetString("defaultemail"));
                }
                else
                {
                    lField.SetPlaceHolder(GalleryData.Instance.mailshare);
                }
                
                lField.OnChangeValue.Add((iVal) => { // Callback on each modification
                    //Debug.Log("Text field : " + iVal);
                    GalleryData.Instance.mailshare = iVal;
                });
            }, () => { // Callback on the left lateral button click
                //Debug.Log("Cancel");

                // set to previous value
                GalleryData.Instance.mailshare = lPreviousValue;
                Buddy.GUI.Dialoger.Hide(); // You must hide manually the Toaster
            }, "Cancel",
            () => { // Callback on the right lateral button click
                 //Debug.Log("OK");
                Buddy.GUI.Dialoger.Hide(); // You must hide manually the Toaster
             }, "OK");

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.STOPPING, "On State Exit...");
        }
        
        private void InitializeHeader()
        {
            // Show settings button
            Buddy.GUI.Header.DisplayParametersButton(true);
            Buddy.GUI.Header.OnClickParameters.Add(DisplaySettingsToaster);
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
