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

		private MoodType mMood;
		private FaceEvent mEvent;

		void Start()
        {
            mFace = BYOS.Instance.Interaction.Face;
			mMood = MoodType.NEUTRAL;
			mEvent = FaceEvent.SMILE;
        }

        public void SetMood(int iMood)
        {
			mMood = ((MoodType) iMood);
        }

		public void SetMood()
		{
			BYOS.Instance.Interaction.Mood.Set(mMood);
		}

		public void SetEvent(int iEvent)
        {
           mEvent = (FaceEvent)iEvent;
        }

		public void SetEvent()
		{
			mFace.SetEvent(mEvent);
		}

	}
}
