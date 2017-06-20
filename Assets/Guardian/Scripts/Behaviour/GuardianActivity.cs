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
            mDetectionManager = (DetectionManager)mObjects[0];
            BYOS.Instance.Resources.LoadAtlas("GuardianAtlas");
            mPrimitives.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
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
            mWebServices.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnMailSent);
            OnMailSent();
        }

        public override void OnClickLockedScreen()
        {
            mDetectionManager.IsDetectingFire = false;
            mDetectionManager.IsDetectingSound = false;
            mDetectionManager.IsDetectingKidnapping = false;
            mDetectionManager.IsDetectingMovement = false;

            mAnimator.GetBehaviour<WalkState>().StopWalkCoroutines();
            mAnimator.GetBehaviour<TurnState>().StopTurnCoroutines();

            mDetectionManager.Roomba.enabled = false;
            mPrimitives.Motors.Wheels.StopWheels();

            mAnimator.SetBool("Password", true);
        }

        public override void OnSuccessUnlockScreen()
        {
            mAnimator.ResetTrigger("InitDetection");
            mAnimator.ResetTrigger("FixedDetection");
            mAnimator.ResetTrigger("MobileDetection");
            mAnimator.ResetTrigger("Turn");
            mAnimator.ResetTrigger("Walk");
            mAnimator.ResetTrigger("Alert");

            mAnimator.SetBool("Password", false);
            mAnimator.Play("EnterMenu");
        }

        public override void OnFailureUnlockScreen()
        {
            mAnimator.ResetTrigger("InitDetection");
            mAnimator.ResetTrigger("FixedDetection");
            mAnimator.ResetTrigger("MobileDetection");
            mAnimator.ResetTrigger("Turn");
            mAnimator.ResetTrigger("Walk");
            mAnimator.ResetTrigger("Alert");

            BYOS.Instance.Toaster.UnlockToast();
            BYOS.Instance.Toaster.Hide();

            mAnimator.SetBool("Password", false);
            mAnimator.Play("Detection");
        }

        private void OnMailSent()
        {
            mNotifier.Display<SimpleNot>().With(mDictionary.GetString("mailsent"), null);
        }
    }
}
