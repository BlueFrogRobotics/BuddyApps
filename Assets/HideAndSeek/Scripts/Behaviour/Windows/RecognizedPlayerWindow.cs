using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BuddyApp.HideAndSeek
{
    public class RecognizedPlayerWindow : AWindow
    {

        [SerializeField]
        private Text message;

        [SerializeField]
        private RawImage imageToDisplay;

        [SerializeField]
        private GameObject picsContainer;

        [SerializeField]
        private Texture2D textUnknownPerson;

        public RawImage ImageToDisplay { get { return imageToDisplay; } }

        public Text Message { get { return message; } }


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
            //message.text = mDictionary.GetString("askPlayerName");
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

        public void SetUnkownPerson()
        {
            //picsContainer.SetActive(true);
            imageToDisplay.texture = textUnknownPerson;
        }

        //public void DisableImage()
        //{
        //    picsContainer.SetActive(false);
        //}
    }
}