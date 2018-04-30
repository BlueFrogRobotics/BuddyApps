using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BuddyApp.Jukebox
{
    public class ActivePause : MonoBehaviour
    {
        [SerializeField]
        private GameObject pause;

        [SerializeField]
        private GameObject play;
         
         
        public void PauseMusicAndIcon()
        {
            if (pause.activeSelf) {
                pause.SetActive(false);
                play.SetActive(true);
            } else if (play.activeSelf) {
                play.SetActive(false);
                pause.SetActive(true);
            }
        }
    }
}