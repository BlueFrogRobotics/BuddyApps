﻿using Buddy;
using Buddy.Features.Web;
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
            BYOS.Instance.ResourceManager.LoadAtlas("GuardianAtlas");
            BYOS.Instance.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
        }

        public override void OnStart(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
        {
            MailSender.OnMailSent += OnMailSent;
            mDetectionManager.LinkDetectorsEvents();
            //mDetectionManager.SoundDetector.StartMic();
        }

        public override void OnQuit()
        {
            MailSender.OnMailSent -= OnMailSent;
            mDetectionManager.SoundDetector.Stop();
            mDetectionManager.UnlinkDetectorsEvents();

            MailSender lMailSender = new MailSender("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL);

            string lMailAddress = GuardianData.Instance.ContactAddress;
            Debug.Log(lMailAddress);
            if (!lMailSender.CanSend || string.IsNullOrEmpty(lMailAddress) || string.IsNullOrEmpty(mDetectionManager.Logs))
                return;

            Mail lMail = new Mail("Guardian logs", mDetectionManager.Logs);
            lMail.AddTo(lMailAddress);
            lMailSender.Send(lMail);
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
            mWheels.StopWheels();

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
