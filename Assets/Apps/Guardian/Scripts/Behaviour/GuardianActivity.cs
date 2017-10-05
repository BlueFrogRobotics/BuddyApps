using Buddy;
using Buddy.UI;
using System.Collections;
using UnityEngine;

namespace BuddyApp.Guardian
{
    public class GuardianActivity : AAppActivity
    {
        private DetectionManager mDetectionManager;

        public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
        {
            Resources.LoadAtlas("GuardianAtlas");
            //Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
            Debug.Log("on loading activity");
            BYOS.Instance.Header.OnClickParameters = OnClickParameters;
            BYOS.Instance.Primitive.TouchScreen.OnClickToUnlock(OnClickLockedScreen);
            BYOS.Instance.Primitive.TouchScreen.OnSuccessUnlock(OnSuccessUnlockScreen);
            BYOS.Instance.Primitive.TouchScreen.OnFailUnlock(OnFailureUnlockScreen);
        }

        public override void OnAwake()
        {
            Debug.Log("on awake activity");
            mDetectionManager = (DetectionManager)Objects[0];
        }


        private bool OnClickParameters()
        {
            Animator.Play("Parameters");
            return false;
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

		public override void OnClickLockedScreen()
		{
            if (!Animator.GetBool("Password"))
            {

                mDetectionManager.IsDetectingFire = false;
                mDetectionManager.IsDetectingSound = false;
                mDetectionManager.IsDetectingKidnapping = false;
                mDetectionManager.IsDetectingMovement = false;

                Animator.GetBehaviour<WalkState>().StopWalkCoroutines();
                Animator.GetBehaviour<TurnState>().StopTurnCoroutines();

                mDetectionManager.Roomba.enabled = false;
                Primitive.Motors.Wheels.Stop();

                Animator.SetBool("Password", true);
            }
        }

        private void OnSuccessUnlockScreen()
        {
            Animator.ResetTrigger("InitDetection");
            Animator.ResetTrigger("FixedDetection");
            Animator.ResetTrigger("MobileDetection");
            Animator.ResetTrigger("Turn");
            Animator.ResetTrigger("Walk");
            Animator.ResetTrigger("Alert");
            mDetectionManager.UnlinkDetectorsEvents();

            Animator.SetBool("Password", false);
            Animator.Play("EnterMenu");
        }

        private void OnFailureUnlockScreen()
        {
=======
		}

		public override void OnSuccessUnlockScreen()
		{
            mDetectionManager.CurrentTimer = 0f;
            mDetectionManager.Countdown = 0f;

            mDetectionManager.IsPasswordCorrect = true;
            mDetectionManager.IsAlarmWorking = false;

            BYOS.Instance.Primitive.Speaker.FX.Loop = false;

            Animator.ResetTrigger("InitDetection");
			Animator.ResetTrigger("FixedDetection");
			Animator.ResetTrigger("MobileDetection");
			Animator.ResetTrigger("Turn");
			Animator.ResetTrigger("Walk");
			Animator.ResetTrigger("Alert");
            mDetectionManager.UnlinkDetectorsEvents();

            Animator.SetBool("Password", false);
			Animator.Play("EnterMenu");
		}

        public override void OnFailureUnlockScreen()
        {
            BYOS.Instance.Primitive.Speaker.FX.Loop = false;

>>>>>>> [GUARDIAN] Almost done alarm Parameter
            Animator.ResetTrigger("InitDetection");
            Animator.ResetTrigger("FixedDetection");
            Animator.ResetTrigger("MobileDetection");
            Animator.ResetTrigger("Turn");
            Animator.ResetTrigger("Walk");
            Animator.ResetTrigger("Alert");

<<<<<<< HEAD
            BYOS.Instance.Toaster.UnlockToast();
            BYOS.Instance.Toaster.Hide();

            Animator.SetBool("Password", false);
            Animator.Play("Detection");
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
            Notifier.Display<SimpleNot>().With(Dictionary.GetString("mailsent"), null);
            Debug.Log("mail sent");
        }
    }
=======
            //BYOS.Instance.Toaster.UnlockToast();
            //BYOS.Instance.Toaster.Hide();

            Animator.SetBool("Password", false);
            if (mDetectionManager.IsAlarmWorking) {
                Debug.Log("Case 1");
                BYOS.Instance.Primitive.Speaker.FX.Loop = true;
                BYOS.Instance.Primitive.Speaker.FX.Play(0);
                Animator.Play("Alert");

            } else if (mDetectionManager.CurrentTimer > 0.0f && mDetectionManager.CurrentTimer < 15f) {
                Debug.Log("Case 2");

                Animator.Play("Alert");
            } else {
                Debug.Log("Case 3");

                Animator.Play("Detection");
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
			Notifier.Display<SimpleNot>().With(Dictionary.GetString("mailsent"), null);
			Debug.Log("mail sent");
		}
	}
>>>>>>> [GUARDIAN] Almost done alarm Parameter
}
