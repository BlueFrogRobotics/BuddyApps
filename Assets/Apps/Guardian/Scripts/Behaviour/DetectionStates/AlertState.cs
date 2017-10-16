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
        private bool mAlarm;

        private IEnumerator mAction;

        public override void Start()
        {
            mDetectionManager = GetComponent<DetectionManager>();

            BYOS.Instance.Primitive.Speaker.FX.Load(
                   BYOS.Instance.Resources.Load<AudioClip>("alarmbeep"), 0
               );
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mAlarm = true;

            mDetectionManager.CurrentTimer = 0.0f;
            mDetectionManager.Countdown = 0.0f;

            mDetectionManager.IsPasswordCorrect = false;

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

			// Send notification to mybuddyapp
			WebRTCListener.SendNotification(mAlert.GetMail().Subject, mAlert.GetMail().Body);

			string lMailAddress = GuardianData.Instance.Contact.Email;
            if (!string.IsNullOrEmpty(lMailAddress) && GuardianData.Instance.SendMail)
                SendMail(lMailAddress);

            mDetectionManager.AddLog(mAlert.GetLog());
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDetectionManager.CurrentTimer += Time.deltaTime;
            if (mDetectionManager.Countdown != 0)
               mDetectionManager.Countdown += Time.deltaTime;

            if (iAnimator.GetBool("Password"))
                BYOS.Instance.Toaster.Hide();

            if (mDetectionManager.CurrentTimer > 15f && !mDetectionManager.IsPasswordCorrect && mAlarm) 
            {
				mDetectionManager.Volume = Primitive.Speaker.GetVolume();
				Primitive.Speaker.ChangeVolume(15);
                mDetectionManager.Countdown += Time.deltaTime;
                mAlarm = false;
                mDetectionManager.IsAlarmWorking = true;
                BYOS.Instance.Primitive.Speaker.FX.Loop = true;
                BYOS.Instance.Primitive.Speaker.FX.Play(0);
            }

            if (mDetectionManager.Countdown > 30f)
            {

				Primitive.Speaker.ChangeVolume(mDetectionManager.Volume);
				iAnimator.SetBool("Password", false);

                mDetectionManager.IsAlarmWorking = false;
                BYOS.Instance.Primitive.Speaker.FX.Loop = false;
                BYOS.Instance.Primitive.Speaker.FX.Stop();
                Trigger("InitDetection");
            }

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
        }

        private void SendMail(string iAddress)
        {
            
            EMail lMail = mAlert.GetMail();
            if (lMail == null)
                return;

            lMail.AddTo(iAddress);
            GetComponent<MediaManager>().Save(lMail);
        }

        private AAlert GetAlert()
        {
            switch (mDetectionManager.Detected) {
                case DetectionManager.Alert.MOVEMENT:
                    return new MovementAlert();

                case DetectionManager.Alert.SOUND:
                    return new SoundAlert();

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
            Notifier.Display<SimpleNot>().With(Dictionary.GetString("mailsent"), null);
        }

    }
}