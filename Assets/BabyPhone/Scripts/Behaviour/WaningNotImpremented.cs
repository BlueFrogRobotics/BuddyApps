using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class WaningNotImpremented : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectViwer;

        void OnEnable()
        {
            objectViwer.SetActive(false);
        }

        void Update()
        {

        }

        public void PlayWarning()
        {
            objectViwer.SetActive(true);
        }

        public void Return()
        {
            objectViwer.SetActive(false);
        }

    }
}