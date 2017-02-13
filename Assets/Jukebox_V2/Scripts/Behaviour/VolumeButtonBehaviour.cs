using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public class VolumeButtonBehaviour : MonoBehaviour
    {
        [SerializeField]
        private GameObject sliderToTurnOnOff;

        public void ActivateButton()
        {
            sliderToTurnOnOff.SetActive(!sliderToTurnOnOff.activeSelf);
        }
    }
}