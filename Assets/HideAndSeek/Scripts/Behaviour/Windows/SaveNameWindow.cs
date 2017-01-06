﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BuddyApp.HideAndSeek
{
    public class SaveNameWindow : AWindow
    {

        [SerializeField]
        private Text message;

        [SerializeField]
        private RawImage imageToDisplay;

        [SerializeField]
        private Button buttonYes;

        [SerializeField]
        private Button buttonNo;

        [SerializeField]
        private Text buttonYesText;

        [SerializeField]
        private Text buttonNoText;

        [SerializeField]
        private InputField inputName;

        public Button ButtonYes { get { return buttonYes; } }

        public Button ButtonNo { get { return buttonNo; } }

        public RawImage ImageToDisplay { get { return imageToDisplay; } }

        public InputField InputName { get { return inputName; } }

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
            message.text = mDictionary.GetString("askPlayerName");
            buttonYesText.text = mDictionary.GetString("yes");
            buttonNoText.text = mDictionary.GetString("no");
        }

        public override void Open()
        {
            animator.SetTrigger("Open_WRecoName");
        }

        public override void Close()
        {
            animator.SetTrigger("Close_WRecoName");
        }

        public override bool IsOff()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName("Window_RecoName_Off");
        }
    }
}