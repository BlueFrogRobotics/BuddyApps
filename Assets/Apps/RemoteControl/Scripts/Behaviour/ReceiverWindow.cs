using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.RemoteControl
{
    public class ReceiverWindow : MonoBehaviour
    {
        [SerializeField]
        private Text textTitle;

        [SerializeField]
        private Text textYesButton;

        [SerializeField]
        private Text textNoButton;

        // Use this for initialization
        void Start()
        {
            textNoButton.text = "no";// Buddy.Resources["no"];
            textYesButton.text = "yes";// Buddy.Resources["yes"];
            textTitle.text = "Take the call ?";// Buddy.Resources["wanttotakecall"]; 
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}