using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using System;

namespace BuddyApp.HideAndSeek
{
    public class ExplainationWindow : AWindow
    {

        [SerializeField]
        private Text message;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Init()
        {
            message.text = mDictionary.GetString("explainationText");
        }

        public override void Open()
        {
            animator.SetTrigger("Open_WExplaination");
        }

        public override void Close()
        {
            animator.SetTrigger("Close_WExplaination");
        }

        public override bool IsOff()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName("Window_Explaination_Off");
        }



    }
}