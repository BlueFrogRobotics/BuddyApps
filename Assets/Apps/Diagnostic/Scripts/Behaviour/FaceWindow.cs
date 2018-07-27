using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public class FaceWindow : MonoBehaviour
    {
        [SerializeField]
        private Dropdown dropdownMood;

        [SerializeField]
        private Dropdown dropdownEvent;

        private Face mFace;

		private FacialExpression mMood;
		private FacialEvent mEvent;

		void Start()
        {
            mFace = Buddy.Behaviour.Face;
			mMood = FacialExpression.NEUTRAL;
			mEvent = FacialEvent.SMILE;
        }

        public void SetMood(int iMood)
        {
			mMood = ((FacialExpression) iMood);
        }

		public void SetMood()
		{
			Buddy.Behaviour.Face.SetFacialExpression(mMood);
		}

		public void SetEvent(int iEvent)
        {
           mEvent = (FacialEvent)iEvent;
        }

		public void SetEvent()
		{
            mFace.PlayEvent(mEvent);
			
		}

	}
}
