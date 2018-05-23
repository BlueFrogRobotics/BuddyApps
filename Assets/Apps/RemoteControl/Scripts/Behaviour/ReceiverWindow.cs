using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Buddy;

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
            textNoButton.text = BYOS.Instance.Dictionary.GetString("no");
            textYesButton.text = BYOS.Instance.Dictionary.GetString("yes");
            textTitle.text = BYOS.Instance.Dictionary.GetString("wanttotakecall"); 
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}