using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BuddyApp.HideAndSeek
{
    public class SearchFacesWindow : AWindow
    {

        [SerializeField]
        private RawImage camView;

        public RawImage CamView { get { return camView; } }

        private CanvasGroup mCanvasGroup;

        // Use this for initialization
        void Start()
        {
            mCanvasGroup = GetComponent<CanvasGroup>();
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
            mCanvasGroup.alpha = 1;
            //animator.SetTrigger("Open_WTimer");
        }

        public override void Close()
        {
            mCanvasGroup.alpha = 0;
            //animator.SetTrigger("Close_WTimer");
        }

        public override bool IsOff()
        {
            return false;// animator.GetCurrentAnimatorStateInfo(0).IsName("Window_Timer_Off");
        }
    }
}