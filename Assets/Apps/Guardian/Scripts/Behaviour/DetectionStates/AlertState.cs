using UnityEngine;
using BlueQuark;

using System.Collections;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State that display the alert and send the notification
    /// </summary>
    public sealed class AlertState : AStateMachineBehaviour
    {
        private DetectionManager mDetectionManager;
        private AAlert mAlert;
        private bool mAlarm;

        private IEnumerator mAction;

        private RecipientsData mContacts;

        public override void Start()
        {
            mDetectionManager = GetComponent<DetectionManager>();

            //Buddy.Actuators.Speakers.Media.Play(
            //       Buddy.Resources.Get<AudioClip>("alarmbeep")
            //   );
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mAlarm = true;
            mContacts = Utils.UnserializeXML<RecipientsData>(Buddy.Resources.GetRawFullPath("contacts.xml"));

            mDetectionManager.CurrentTimer = 0.0f;
            mDetectionManager.Countdown = 0.0f;

            mDetectionManager.IsPasswordCorrect = false;

            mDetectionManager.IsDetectingFire = false;
            mDetectionManager.IsDetectingKidnapping = false;
            mDetectionManager.IsDetectingMovement = false;
            mDetectionManager.IsDetectingSound = false;

            Buddy.Actuators.Wheels.Stop();

            mAlert = GetAlert();

            Buddy.Behaviour.SetMood(Mood.SCARED);
            Buddy.Vocal.Say(mAlert.GetSpeechText());

            mAction = DisplayAlert();
            StartCoroutine(mAction);

            // Send notification to mybuddyapp
            //WebRTCListener.SendNotification(mAlert.GetMail().Subject, mAlert.GetMail().Body);

            string lMailAddress = mContacts.Recipients[GuardianData.Instance.ContactId].Mail;//GuardianData.Instance.Contact.Email;
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
                Buddy.GUI.Toaster.Hide();

            if (mDetectionManager.CurrentTimer > 10f && !mDetectionManager.IsPasswordCorrect && mAlarm && GuardianData.Instance.AlarmActivated) 
            {
				mDetectionManager.Volume = (int)(Buddy.Actuators.Speakers.Volume*100F); 
				Buddy.Actuators.Speakers.Volume = 0.70F;
                mDetectionManager.Countdown += Time.deltaTime;
                mAlarm = false;
                mDetectionManager.IsAlarmWorking = true;
                Buddy.Actuators.Speakers.Media.Repeat = true;
                Buddy.Actuators.Speakers.Media.Play(Buddy.Resources.Get<AudioClip>("alarmbeep"));
            }

            if (mDetectionManager.Countdown > 20f)
            {

				iAnimator.SetBool("Password", false);

                mDetectionManager.IsAlarmWorking = false;
                if (GuardianData.Instance.AlarmActivated)
                {
                    Buddy.Actuators.Speakers.Volume = (float)(mDetectionManager.Volume / 100F);
                    Buddy.Actuators.Speakers.Media.Repeat = false;
                    Buddy.Actuators.Speakers.Media.Stop();
                }
                Trigger("InitDetection");
            }
            
        }
        
        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }

        private IEnumerator DisplayAlert()
        {
            Debug.Log("display alert");
            //Buddy.GUI.Toaster.Display<IconToast>().With(mAlert.GetDisplayText(), mAlert.GetIcon(), new Color32(212, 0, 22, 255), true);
            Buddy.GUI.Header.DisplayLightTitle(mAlert.GetDisplayText());
            Buddy.GUI.Toaster.Display<IconToast>().With(mAlert.GetIcon(), Color.white, new Color32(212, 0, 22, 255), () => {

                Buddy.GUI.Toaster.Hide();

            });

            yield return new WaitForSeconds(5F);
        }

        private void SendMail(string iAddress)
        {
            
            BlueQuark.EMail lMail = mAlert.GetMail();
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
            Buddy.GUI.Notifier.Display<SimpleNotification>().With(Buddy.Resources.GetString("mailsent"), null);
        }

    }
}