using Buddy;
using Buddy.UI;
using System.Collections;
using UnityEngine;

namespace BuddyApp.Guardian
{
    public class GuardianActivity : AAppActivity
    {
        public static DetectionManager sDetectionManager;/// TODO: apres la release de core, remettre cette variable en private non static 

        public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
        {
            Resources.LoadAtlas("GuardianAtlas");
            Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
        }

        public override void OnStart(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
        {
            ///TODO: decommenter la ligne ci dessous apres la release de core
            //sDetectionManager = (DetectionManager)Objects[0];
        }

        public override void OnQuit()
        {
            sDetectionManager.UnlinkDetectorsEvents();

            string lMailAddress = GuardianData.Instance.ContactAddress;
            Debug.Log(lMailAddress);
            if (string.IsNullOrEmpty(lMailAddress) || string.IsNullOrEmpty(sDetectionManager.Logs))
                return;

            EMail lMail = new EMail("Guardian logs", sDetectionManager.Logs);
            lMail.AddTo(lMailAddress);
            WebService.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnMailSent);
            OnMailSent();
        }

        public override void OnClickLockedScreen()
        {
            sDetectionManager.IsDetectingFire = false;
            sDetectionManager.IsDetectingSound = false;
            sDetectionManager.IsDetectingKidnapping = false;
            sDetectionManager.IsDetectingMovement = false;

            Animator.GetBehaviour<WalkState>().StopWalkCoroutines();
            Animator.GetBehaviour<TurnState>().StopTurnCoroutines();

            sDetectionManager.Roomba.enabled = false;
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

        public static void StartManager()
        {
            //GuardianActivity lActivity = (GuardianActivity)BYOS.Instance.AppManager.CurrentApp.AppActivity;
        }

        private void InitManager()
        {
            sDetectionManager = (DetectionManager)Objects[0];
            sDetectionManager.Init();
            sDetectionManager.LinkDetectorsEvents();
        }

        private void OnMailSent()
        {
            Notifier.Display<SimpleNot>().With(Dictionary.GetString("mailsent"), null);
            Debug.Log("mail sent");
        }
    }
}
