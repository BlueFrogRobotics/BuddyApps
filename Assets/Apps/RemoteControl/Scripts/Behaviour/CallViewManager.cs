using BlueQuark;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.RemoteControl
{
    public class CallViewManager : MonoBehaviour
    {
        [Header("VOLUME SLIDER SETTINGS")]
        [SerializeField]
        private GameObject VolumeManager;

        [SerializeField]
        private Slider VolumeSlider;

        [SerializeField]
        private Text VolumeSliderText;

        // This function enable the Slider GameObject and add some code to it, to setting up the volume.
        public void DisplayVolumeSlider()
        {
            // If one reference is missing, nothing append
            if (!VolumeManager || !VolumeSlider || !VolumeSliderText)
                return;
            // If the volume gameobject is already active, nothing append
            if (VolumeManager.activeSelf == true)
                return;
            VolumeManager.SetActive(true);
            VolumeSlider.value = (int)(Buddy.Actuators.Speakers.Volume * 100F);
            VolumeSliderText.text = VolumeSlider.value.ToString();
            VolumeSlider.onValueChanged.AddListener((iVolume) =>
            {
                Buddy.Actuators.Speakers.Volume = iVolume / 100F;
                VolumeSliderText.text = iVolume.ToString();
            });
        }

        // This function disable the Slider GameObject and remove all code associate to it.
        // Useful if we want to hide the slider, during the call
        // Useless at call stopped, because the hide animation, already do the job
        public void HideVolumeSlider()
        {
            // If one reference is missing, nothing append
            if (!VolumeManager || !VolumeSlider || !VolumeSliderText)
                return;
            // If the volume gameobject is already inactive, nothing append
            if (VolumeManager.activeSelf == false)
                return;
            VolumeSlider.onValueChanged.RemoveAllListeners();
            VolumeManager.SetActive(false);
        }
    }
}
