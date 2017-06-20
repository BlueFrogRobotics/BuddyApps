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
            mDetectionManager = (DetectionManager)Objects[0];
            BYOS.Instance.Resources.LoadAtlas("GuardianAtlas");
            Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
        }

        public override void OnStart(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
        {
            mDetectionManager.LinkDetectorsEvents();
            //mDetectionManager.SoundDetector.StartMic();
        }

        public override void OnQuit()
        {
            mDetectionManager.SoundDetector.Stop();
            mDetectionManager.UnlinkDetectorsEvents();

            string lMailAddress = GuardianData.Instance.ContactAddress;
            Debug.Log(lMailAddress);
            if (string.IsNullOrEmpty(lMailAddress) || string.IsNullOrEmpty(mDetectionManager.Logs))
                return;

            EMail lMail = new EMail("Guardian logs", mDetectionManager.Logs);
            lMail.AddTo(lMailAddress);
            WebService.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnMailSent);
            OnMailSent();
        }

        public override void OnClickLockedScreen()
        {
            mDetectionManager.IsDetectingFire = false;
            mDetectionManager.IsDetectingSound = false;
            mDetectionManager.IsDetectingKidnapping = false;
            mDetectionManager.IsDetectingMovement = false;

            Animator.GetBehaviour<WalkState>().StopWalkCoroutines();
            Animator.GetBehaviour<TurnState>().StopTurnCoroutines();

            mDetectionManager.Roomba.enabled = false;
            Primitive.Motors.Wheels.StopWheels();

            Animator.SetBool("Password", true);
        }

        public override void OnSuccessUnlockScreen()
        {
            Animator.ResetTrigger("InitDetection");
            Animator.ResetTrigger("FixedDetection");
            Animator.ResetTrigger("MobileDetection");
            Animator.ResetTrigger("Turn");
            Animator.ResetTrigger("Walk");
            Animator.ResetTrigger("Alert");

            Animator.SetBool("Password", false);
            Animator.Play("EnterMenu");
        }

        public override void OnFailureUnlockScreen()
        {
            Animator.ResetTrigger("InitDetection");
            Animator.ResetTrigger("FixedDetection");
            Animator.ResetTrigger("MobileDetection");
            Animator.ResetTrigger("Turn");
            Animator.ResetTrigger("Walk");
            Animator.ResetTrigger("Alert");

            BYOS.Instance.Toaster.UnlockToast();
            BYOS.Instance.Toaster.Hide();

            Animator.SetBool("Password", false);
            Animator.Play("Detection");
        }

        private void OnMailSent()
        {
            Notifier.Display<SimpleNot>().With(Dictionary.GetString("mailsent"), null);
        }
    }
}
