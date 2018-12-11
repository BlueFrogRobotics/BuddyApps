using BlueQuark;

using UnityEngine;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// Activity of the guardian application.
    /// Manages the password disabler lockscreen
    /// </summary>
    public sealed class GuardianActivity : AAppActivity
    {
        private DetectionManager mDetectionManager;

        public override void OnAwake()
        {
            Buddy.GUI.Header.OnClickParameters.Add(OnClickParameters);  
            Buddy.GUI.Screen.OnClickToUnlock.Add(OnClickLockedScreen);
            Buddy.GUI.Screen.OnSuccessUnlock.Add(OnSuccessUnlockScreen);
            Buddy.GUI.Screen.OnFailUnlock.Add(OnFailureUnlockScreen);
            Buddy.GUI.Screen.OnCancelUnlock.Add(OnCancelUnlockScreen);
            Buddy.GUI.Screen.OnTimeoutUnlock.Add(OnTimeoutUnlockScreen);
            mDetectionManager = (DetectionManager)Objects[0];
        }

        private void OnClickParameters()
        {
            Animator.Play("Parameters");
            Buddy.GUI.Header.DisplayParametersButton(false);
        }

        private void OnCancelUnlockScreen()
        {
            OnHideLockNumpad();
        }

        private void OnTimeoutUnlockScreen(float iTimeout)
        {
            OnHideLockNumpad();
        }

        private void OnHideLockNumpad() {

            if (mDetectionManager.CurrentTimer == 0.0f)
            {
                mDetectionManager.IsDetectingMovement = GuardianData.Instance.MovementDetection;
                mDetectionManager.IsDetectingSound = GuardianData.Instance.SoundDetection;
                mDetectionManager.IsDetectingFire = GuardianData.Instance.FireDetection;
                mDetectionManager.IsDetectingKidnapping = GuardianData.Instance.KidnappingDetection;

                Animator.SetBool("Password", false);
                Animator.Play("Detection");
            }
        }

        public override void OnStart()
        {
           
        }

        public override void OnQuit()
        {
            string lMailAddress = GuardianData.Instance.Contact.Email;
            if (string.IsNullOrEmpty(lMailAddress) || string.IsNullOrEmpty(mDetectionManager.Logs))
                return;

            ///TODO: Send log by mail
            //if (GuardianData.Instance.SendMail)
            //{
            //    EMail lMail = new EMail("Guardian logs", mDetectionManager.Logs);
            //    lMail.AddTo(lMailAddress);
            //    WebService.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnMailSent);
            //    BYOS.Instance.WebService.EMailSender.enabled = true;
            //    OnMailSent();
            //}
        }

		private void OnClickLockedScreen()
		{
            if (!Animator.GetBool("Password"))
            {

                mDetectionManager.IsDetectingFire = false;
                mDetectionManager.IsDetectingSound = false;
                mDetectionManager.IsDetectingKidnapping = false;
                mDetectionManager.IsDetectingMovement = false;

                Animator.GetBehaviour<WalkState>().StopWalkCoroutines();
                Animator.GetBehaviour<TurnState>().StopTurnCoroutines();

                Buddy.Actuators.Wheels.Stop();

                Animator.SetBool("Password", true);
            }
        }

		private void OnSuccessUnlockScreen()
		{
			Buddy.Actuators.Speakers.Volume = mDetectionManager.Volume / 100F;
			mDetectionManager.CurrentTimer = 0F;
            mDetectionManager.Countdown = 0F;

            mDetectionManager.IsPasswordCorrect = true;
            mDetectionManager.IsAlarmWorking = false;

            Buddy.Actuators.Speakers.Media.Repeat = false;

            Animator.ResetTrigger("InitDetection");
			Animator.ResetTrigger("FixedDetection");
			Animator.ResetTrigger("MobileDetection");
			Animator.ResetTrigger("Turn");
			Animator.ResetTrigger("Walk");
			Animator.ResetTrigger("Alert");
            mDetectionManager.UnlinkDetectorsEvents();

            Animator.SetBool("Password", false);
            QuitApp();
		}

        private void OnFailureUnlockScreen()
        {
            Buddy.Actuators.Speakers.Media.Repeat = false;

            Animator.ResetTrigger("InitDetection");
            Animator.ResetTrigger("FixedDetection");
            Animator.ResetTrigger("MobileDetection");
            Animator.ResetTrigger("Turn");
            Animator.ResetTrigger("Walk");
            Animator.ResetTrigger("Alert");

            if (mDetectionManager.IsAlarmWorking) {
                Buddy.Actuators.Speakers.Media.Repeat = true;
                Buddy.Actuators.Speakers.Media.Play(0);
                Animator.Play("Alert");

            } else if (mDetectionManager.CurrentTimer > 0.0f && mDetectionManager.CurrentTimer < 15F) {
                Animator.Play("Alert");
            }
        }


		private void OnMailSent()
		{
            Buddy.GUI.Notifier.Display<SimpleNotification>().With(Buddy.Resources.GetString("mailsent"), null);
            
			Debug.Log("mail sent");
		}
	}
}
