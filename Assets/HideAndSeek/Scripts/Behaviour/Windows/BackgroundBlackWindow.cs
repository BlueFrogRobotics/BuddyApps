using UnityEngine;
using System.Collections;

namespace BuddyApp.HideAndSeek
{
    public class BackgroundBlackWindow : AWindow
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
            animator.SetTrigger("Open_BG");
        }

        public override void Close()
        {
            animator.SetTrigger("Close_BG");
        }

        public override bool IsOff()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName("Background_Black_Off");
        }
    }
}