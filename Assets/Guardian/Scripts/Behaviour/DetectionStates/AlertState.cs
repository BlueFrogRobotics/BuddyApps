using UnityEngine;
using Buddy;
using Buddy.UI;
using System.Collections;

namespace BuddyApp.Guardian
{
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
            Primitive.Motors.Wheels.StopWheels();

            mAlert = GetAlert();

            Interaction.Mood.Set(MoodType.SCARED);
            Interaction.TextToSpeech.Say(mAlert.GetSpeechText());

            mAction = DisplayAlert();
            StartCoroutine(mAction);

            string lMailAddress = GuardianData.Instance.ContactAddress;
            if (!string.IsNullOrEmpty(lMailAddress))
                SendMail(lMailAddress);

            mDetectionManager.AddLog(mAlert.GetLog());
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        private IEnumerator DisplayAlert()
        {
            BYOS.Instance.Toaster.Display<IconToast>().With(mAlert.GetDisplayText(), mAlert.GetIcon(), new Color32(212, 0, 22, 255), true);

            yield return new WaitForSeconds(5F);

            Trigger("InitDetection");
        }

        private void SendMail(string iAddress)
        {
            EMail lMail = mAlert.GetMail();
            if (lMail == null)
                return;

            lMail.AddTo(iAddress);
            WebService.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail);

            Debug.Log("Send mail to " + iAddress);
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

    }
}