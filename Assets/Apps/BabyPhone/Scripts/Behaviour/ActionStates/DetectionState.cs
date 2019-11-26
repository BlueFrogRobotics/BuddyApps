using BlueQuark;

using UnityEngine;

using System.Collections;

namespace BuddyApp.BabyPhone
{
	/// <summary>
	/// State that init that activate the detections chosen by hte user and pass to the next mode state
	/// </summary>
	public sealed class DetectionState : AStateMachineBehaviour
	{
		private DetectionManager mDetectionManager;
        private MediaManager mMediaManager;

        public override void Start()
		{
			mDetectionManager = GetComponent<DetectionManager>();
            mMediaManager = GetComponent<MediaManager>();
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mDetectionManager.IsDetectingMovement = BabyPhoneData.Instance.MovementDetection;
            mDetectionManager.IsDetectingSound = BabyPhoneData.Instance.SoundDetection;
            
            Buddy.GUI.Toaster.Hide();

            if (!mDetectionManager.HasLinkedDetector)
            {
                mDetectionManager.LinkDetectorsEvents();
            }
            if (!mMediaManager.enabled)
                mMediaManager.enabled = true;

            //StartCoroutine(DelayAlertAsync());
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        // For test purpose
        //private IEnumerator DelayAlertAsync()
        //{
        //    yield return new WaitForSeconds(4F);
        //    //if (mDetectionManager.HasLinkedDetector)
        //    //{
        //    //    mDetectionManager.AlertSimulation();
        //    //}
        //}
	}
}