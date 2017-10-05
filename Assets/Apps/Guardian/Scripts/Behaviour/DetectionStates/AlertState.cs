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
        //private float mTimer;
        private bool mAlarm;
        //System.Media.SoundPlayer mplayer = new System.Media.SoundPlayer(@"C:\Users\macpc\Desktop\Random\alarmbeep.mp3");
        
       

        private IEnumerator mAction;

        public override void Start()
        {
            //AudioSource lAudioSource = GetComponent<AudioSource>();
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


            Debug.Log("debut Alerte");

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
            if (iAnimator.GetBool("Password") == false)
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
            mDetectionManager.CurrentTimer += Time.deltaTime;
            if (mDetectionManager.Countdown != 0)
               mDetectionManager.Countdown += Time.deltaTime;
            Debug.Log("TIME : " + mDetectionManager.CurrentTimer);
            Debug.Log("COUNTDOWN : " + mDetectionManager.Countdown);

            if (iAnimator.GetBool("Password"))
                BYOS.Instance.Toaster.Hide();

            if (mDetectionManager.CurrentTimer > 15f && !mDetectionManager.IsPasswordCorrect && mAlarm) 
            {
                mDetectionManager.Countdown += Time.deltaTime;
                Debug.Log("Coucou HIBOUUUUU");
                mAlarm = false;
                //mTimer = 14f;
                mDetectionManager.IsAlarmWorking = true;
                BYOS.Instance.Primitive.Speaker.FX.Loop = true;
                BYOS.Instance.Primitive.Speaker.FX.Play(0);
            }

            if (mDetectionManager.Countdown > 30f)
            {
                iAnimator.SetBool("Password", false);

                Debug.Log("CA PASSE ICI");
                mDetectionManager.IsAlarmWorking = false;
                BYOS.Instance.Primitive.Speaker.FX.Loop = false;
                BYOS.Instance.Primitive.Speaker.FX.Stop();
                Trigger("InitDetection");
            }


            //ptetre la
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
            Debug.Log("Ca fait 5S !!!!!");

            //Trigger("InitDetection"); // 
        }

        private void SendMail(string iAddress)
        {
            
            EMail lMail = mAlert.GetMail();
            Debug.Log("send mail pre");
            if (lMail == null)
                return;

            Debug.Log("send mail post");
            lMail.AddTo(iAddress);
            WebService.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnMailSent);
            BYOS.Instance.WebService.EMailSender.enabled = true;
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