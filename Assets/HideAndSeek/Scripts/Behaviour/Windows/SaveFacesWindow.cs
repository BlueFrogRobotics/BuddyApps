using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BuddyApp.HideAndSeek
{
    public class SaveFacesWindow : AWindow
    {

        [SerializeField]
        private Text message;

        [SerializeField]
        private RawImage imageToDisplay;

        [SerializeField]
        private Scrollbar scrollLoading;

        [SerializeField]
        private Button buttonGo;

        [SerializeField]
        private Text buttonGoText;

        public Button ButtonGo { get { return buttonGo; } }

        public Scrollbar ScrollLoading { get { return scrollLoading; } }

        public RawImage ImageToDisplay { get { return imageToDisplay; } }

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
            message.text = mDictionary.GetString("askStartReco");
            buttonGoText.text = mDictionary.GetString("go");
        }

        public override void Open()
        {
            animator.SetTrigger("Open_WReco");
        }

        public override void Close()
        {
            animator.SetTrigger("Close_WReco");
        }

        public override bool IsOff()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName("Window_Reco_Off");
        }
    }
}