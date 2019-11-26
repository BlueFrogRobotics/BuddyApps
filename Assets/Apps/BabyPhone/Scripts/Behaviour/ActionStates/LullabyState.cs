using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System;

namespace BuddyApp.BabyPhone
{
	/// <summary>
	/// State that plays the lullaby
	/// </summary>
	public sealed class LullabyState : AStateMachineBehaviour
    {
        private Scrollbar mTimeScrollbar;
        private DateTime mStartTime;

        public override void Start()
        {
            mTimeScrollbar = GameObject.Find("AppGUI/Time_Progress").GetComponent<Scrollbar>();
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartTime = DateTime.Now;

            mTimeScrollbar.gameObject.SetActive(true);
            mTimeScrollbar.size = 0;
            mTimeScrollbar.value = 0;

            AudioClip music = Buddy.Resources.Get<AudioClip>("LullabyBaby.mp3");
            Buddy.Actuators.Speakers.Media.Play(music);

            StartCoroutine(WaitForMusicEnd());
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (BabyPhoneData.Instance.LullabyDuration > 0)
            {
                double lElapsedTime = (DateTime.Now - mStartTime).TotalSeconds;
                mTimeScrollbar.size = (float)(lElapsedTime / (BabyPhoneData.Instance.LullabyDuration * 60));
            }
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Footer.Hide();
            mTimeScrollbar.gameObject.SetActive(false);
            Buddy.Actuators.Speakers.Media.Stop();
            StopAllCoroutines();
        }

        private IEnumerator WaitForMusicEnd()
        {
            yield return new WaitForSeconds(BabyPhoneData.Instance.LullabyDuration * 60);
            Buddy.Actuators.Speakers.Media.Stop();

            Trigger("InitDetection");
        }
	}
}