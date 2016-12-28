using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BuddyApp.HideAndSeek
{
    public class QuestionAddPlayerWindow : AWindow
    {
        [SerializeField]
        private Text message;

        [SerializeField]
        private Button buttonYes;

        [SerializeField]
        private Button buttonNo;

        [SerializeField]
        private Text buttonYesText;

        [SerializeField]
        private Text buttonNoText;

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
            message.text = mDictionary.GetString("askAddPlayer");
            buttonYesText.text = mDictionary.GetString("yes");
            buttonNoText.text = mDictionary.GetString("no");
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