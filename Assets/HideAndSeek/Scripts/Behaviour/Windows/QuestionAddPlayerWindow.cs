using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BuddyApp.HideAndSeek
{
    public class QuestionAddPlayerWindow : AWindow
    {

        [SerializeField]
        private Button buttonYes;

        [SerializeField]
        private Button buttonNo;

        public Button ButtonYes { get { return buttonYes; } }

        public Button ButtonNo { get { return buttonNo; } }

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
            animator.SetTrigger("Open_WQuestion");
        }

        public override void Close()
        {
            animator.SetTrigger("Close_WQuestion");
        }

        public override bool IsOff()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName("Window_Question_Off");
        }


    }
}