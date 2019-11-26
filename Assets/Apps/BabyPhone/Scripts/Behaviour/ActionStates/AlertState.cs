using UnityEngine;
using BlueQuark;

using System;
using System.Collections;
using System.Globalization;

namespace BuddyApp.BabyPhone
{
    /// <summary>
    /// State that send the notification
    /// </summary>
    public sealed class AlertState : AStateMachineBehaviour
    {
        private DetectionManager mDetectionManager;

        private RecipientsData mContacts;

        public override void Start()
        {
            mDetectionManager = GetComponent<DetectionManager>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mContacts = Utils.UnserializeXML<RecipientsData>(Buddy.Resources.GetRawFullPath("contacts.xml"));

            mDetectionManager.Countdown = 0.0f;
            mDetectionManager.IsDetectingMovement = false;
            mDetectionManager.IsDetectingSound = false;

            Buddy.Actuators.Wheels.Stop();

            Buddy.Behaviour.SetMood(Mood.SAD);

            //StartCoroutine(DisplayAlertAsync());

            // Send notification to mybuddyapp
            //WebRTCListener.SendNotification(mAlert.GetMail().Subject, mAlert.GetMail().Body);

            if (mContacts.Recipients.Count > 0) {
                string lMailAddress = mContacts.Recipients[BabyPhoneData.Instance.ContactId].Mail;
                if (!string.IsNullOrEmpty(lMailAddress) && BabyPhoneData.Instance.SendMail)
                    SendMail(lMailAddress);
            }
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDetectionManager.Countdown += Time.deltaTime;

            if (mDetectionManager.Countdown > 20f)
            {
                if (BabyPhoneData.Instance.ReplayOnDetection)
                    Trigger("StartLullaby");
                else
                    Trigger("InitDetection");
            }
            
        }
        
        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }

        // For test purpose
        //private IEnumerator DisplayAlertAsync()
        //{
        //    yield return new WaitForSeconds(2F);
        //    //GetComponent<MediaManager>().VideoSaved("");
        //}

        private void SendMail(string iAddress)
        {
            string key = mDetectionManager.Detected == DetectionManager.Alert.MOVEMENT ? "movementalertmessage" : "soundalertmessage";

            string babyName = BabyPhoneData.Instance.BabyName;
            if (string.IsNullOrEmpty(babyName))
                babyName = Buddy.Resources.GetString("baby");
            string emailContent = string.Format(Buddy.Resources.GetString(key), babyName);

            string emailSubject = Buddy.Resources.GetString("alert");

            BlueQuark.EMail lMail = new EMail(emailSubject, emailContent);
            if (lMail == null)
                return;

            lMail.AddTo(iAddress);
            GetComponent<MediaManager>().Save(lMail);
        }

        private void OnMailSent()
        {
            Buddy.GUI.Notifier.Display<SimpleNotification>().With(Buddy.Resources.GetString("mailsent"), null);
        }

    }
}