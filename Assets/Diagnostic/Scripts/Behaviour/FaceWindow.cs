using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Buddy;

namespace BuddyApp.Diagnostic
{
    public class FaceWindow : MonoBehaviour
    {
        [SerializeField]
        private Dropdown dropdownMood;

        [SerializeField]
        private Dropdown dropdownEvent;

        private Face mFace;

        void Start()
        {
            mFace = BYOS.Instance.Face;
        }

        public void SetMood()
        {
            //mFace.SetMood();
        }

        public void SetEvent()
        {
            //mFace.SetEvent((FaceEvent)iEvent);
        }
    }
}
