using BlueQuark;
using System.Collections;
using UnityEngine;

namespace BuddyApp.Guardian
{
    public sealed class GuardianActivity : AAppActivity
    {
        private DetectionManager mDetectionManager;

        public override void OnAwake()
        {
            Debug.Log("on awake activity");
            //Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
            Debug.Log("on loading activity");
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
			//return false;
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
            Debug.Log("on start activity");
        }

        public override void OnQuit()
        {
            //mDetectionManager.UnlinkDetectorsEvents();

            string lMailAddress = GuardianData.Instance.Contact.Email;
            Debug.Log(lMailAddress);
            if (string.IsNullOrEmpty(lMailAddress) || string.IsNullOrEmpty(mDetectionManager.Logs))
                return;

            // Send log by mail
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

                //mDetectionManager.Roomba.enabled = false;
                Buddy.Actuators.Wheels.Stop();

                Animator.SetBool("Password", true);
            }
        }

		private void OnSuccessUnlockScreen()
		{
            Debug.Log("on success unlock screen");
			Buddy.Actuators.Speakers.Volume = mDetectionManager.Volume;
            Debug.Log("unlock 1");
			mDetectionManager.CurrentTimer = 0f;
            mDetectionManager.Countdown = 0f;
            Debug.Log("unlock 2");

            mDetectionManager.IsPasswordCorrect = true;
            mDetectionManager.IsAlarmWorking = false;

            Debug.Log("unlock 3");
            Buddy.Actuators.Speakers.Media.Repeat = false;

            Animator.ResetTrigger("InitDetection");
			Animator.ResetTrigger("FixedDetection");
			Animator.ResetTrigger("MobileDetection");
			Animator.ResetTrigger("Turn");
			Animator.ResetTrigger("Walk");
			Animator.ResetTrigger("Alert");
            Debug.Log("unlock 4");
            mDetectionManager.UnlinkDetectorsEvents();
            Debug.Log("unlock 5");

            Animator.SetBool("Password", false);
            QuitApp();
			//Animator.Play("EnterMenu");
            Debug.Log("unlock 6");
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

            } else if (mDetectionManager.CurrentTimer > 0.0f && mDetectionManager.CurrentTimer < 15f) {
                Animator.Play("Alert");
            }
        }

        public static void StartManager()
		{
			//GuardianActivity lActivity = (GuardianActivity)BYOS.Instance.AppManager.CurrentApp.AppActivity;
		}

		//private void InitManager()
		//{
		//	sDetectionManager = (DetectionManager)Objects[0];
		//	sDetectionManager.Init();
		//	sDetectionManager.LinkDetectorsEvents();
		//}

		private void OnMailSent()
		{
            Buddy.GUI.Notifier.Display<SimpleNotification>().With(Buddy.Resources.GetString("mailsent"), null);
            
			Debug.Log("mail sent");
		}
	}
}
