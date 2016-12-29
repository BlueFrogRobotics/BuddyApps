using UnityEngine;
using System.Collections;
using System;

namespace BuddyApp.HideAndSeek
{
    public class TimerWindow : AWindow
    {

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

        }

        public override void Open()
        {
            animator.SetTrigger("Open_WTimer");
        }

        public override void Close()
        {
            animator.SetTrigger("Close_WTimer");
        }

        public override bool IsOff()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName("Window_Timer_Off");
        }
    }
}