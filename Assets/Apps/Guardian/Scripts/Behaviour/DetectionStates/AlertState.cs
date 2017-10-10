using UnityEngine;
using Buddy;
using Buddy.UI;
using System.Collections;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State that display the alert and send the notification
    /// </summary>
    public class AlertState : AStateMachineBehaviour
    {
        private DetectionManager mDetectionManager;
        private AAlert mAlert;

        private IEnumerator mAction;

        public override void Start()
        {
            mDetectionManager = GetComponent<DetectionManager>();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDetectionManager.IsDetectingFire = false;
            mDetectionManager.IsDetectingKidnapping = false;
            mDetectionManager.IsDetectingMovement = false;
            mDetectionManager.IsDetectingSound = false;

            mDetectionManager.Roomba.enabled = false;
            Primitive.Motors.Wheels.Stop();

            mAlert = GetAlert();

            Interaction.Mood.Set(MoodType.SCARED);
            Interaction.TextToSpeech.Say(mAlert.GetSpeechText());

            mAction = DisplayAlert();
            StartCoroutine(mAction);
			//WebService.EMailSender.enabled = true;

			// Send notification to mybuddyapp
			WebRTCListener.SendNotification(mAlert.GetMail().Subject, mAlert.GetMail().Body);

			string lMailAddress = GuardianData.Instance.Contact.Email;
            if (!string.IsNullOrEmpty(lMailAddress) && GuardianData.Instance.SendMail)
                SendMail(lMailAddress);

            mDetectionManager.AddLog(mAlert.GetLog());
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (iAnimator.GetBool("Password"))
                BYOS.Instance.Toaster.Hide();
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        private IEnumerator DisplayAlert()
        {
            Debug.Log("display alert");
            BYOS.Instance.Toaster.Display<IconToast>().With(mAlert.GetDisplayText(), mAlert.GetIcon(), new Color32(212, 0, 22, 255), true);

            yield return new WaitForSeconds(5F);

            Trigger("InitDetection");
        }

        private void SendMail(string iAddress)
        {
            
            EMail lMail = mAlert.GetMail();
            Debug.Log("send mail pre");
            if (lMail == null)
                return;

            Debug.Log("send mail post");
            lMail.AddTo(iAddress);
            GetComponent<MediaManager>().Save(lMail);
            //WebService.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnMailSent);
            //BYOS.Instance.WebService.EMailSender.enabled = true;
            Debug.Log("send mail encore apres");
        }

        private AAlert GetAlert()
        {
            switch (mDetectionManager.Detected) {
                case DetectionManager.Alert.MOVEMENT:
                    return new MovementAlert(mDetectionManager.SaveVideo);

                case DetectionManager.Alert.SOUND:
                    return new SoundAlert(mDetectionManager.SaveAudio);

                case DetectionManager.Alert.FIRE:
                    return new FireAlert();

                case DetectionManager.Alert.KIDNAPPING:
                    return new KidnappingAlert();

                default:
                    return null;
            }
        }

        private void OnMailSent()
        {
            Debug.Log("le mail a ete fabuleusement envoye");
            Notifier.Display<SimpleNot>().With(Dictionary.GetString("mailsent"), null);
        }

    }
}