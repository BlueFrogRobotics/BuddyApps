using BlueQuark;

using UnityEngine;


namespace BuddyApp.Guardian
{
	public sealed class StartTimerState : AStateMachineBehaviour
	{
		int mTimer;

		private bool mStartTimer;

		public override void Start()
		{
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			Buddy.GUI.Header.DisplayParametersButton(true);

			if (GuardianData.Instance.FirstRun) {
				Buddy.Vocal.SayKey("firststartdetectiontimer");
				GuardianData.Instance.FirstRun = false;
				mTimer = 10;
            } else {
                Buddy.Vocal.SayKey("startdetectiontimer");
				mTimer = 5;
			}

			mStartTimer = false;
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (Buddy.Vocal.IsSpeaking || mStartTimer)
				return;

            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("startdetectiontimer"));
            Buddy.GUI.Toaster.Display<CountdownToast>().With(mTimer, new Color(0, 0, 0, 180), 
            (iCountdown) =>
            {
                iCountdown.Playing = !iCountdown.Playing;
                Cancel();
            },
            (iCountdown) =>
            {
                if(iCountdown.IsDone)
                {
                    Buddy.GUI.Toaster.Hide();
                    InitDetection();
                }
            }
            );

			mStartTimer = true;
		}

		private void InitDetection()
		{
			Buddy.GUI.Header.DisplayParametersButton(false);
			Trigger("InitDetection");
		}

		private void Cancel()
		{
            Buddy.GUI.Header.DisplayParametersButton(false);
            Trigger("Cancel");
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Header.HideTitle();
            mStartTimer = false;
		}
	}
}