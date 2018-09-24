using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Mail;

using BlueQuark;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// Manager class that have reference to the differents stimuli and subscribes to their callbacks
	/// </summary>
	[RequireComponent(typeof(Navigation))]
	public sealed class DetectionManager : MonoBehaviour
	{

		public const float MAX_SOUND_THRESHOLD = 0.03F;
		public const float KIDNAPPING_THRESHOLD = 4.5F;
		public const float MAX_MOVEMENT_THRESHOLD = 5.0F;

		private Animator mAnimator;
		private KidnappingDetector mKidnappingDetection;
		private MotionDetector mMotionDetection;
		private NoiseDetector mNoiseDetection;
		private ThermalDetector mFireDetection;

        /// <summary>
        /// Speaker volume
        /// </summary>
        public int Volume { get; set; }

        public float CurrentTimer { get; set; }
        public float Countdown { get; set; }

        public string Logs { get; private set; }

		public bool PreviousScanLeft { get; set; }
        //public Navigation Roomba { get; set; }
		//public RoombaNavigation Roomba { get; private set; }

		public bool IsDetectingFire { get; set; }
		public bool IsDetectingMovement { get; set; }
		public bool IsDetectingKidnapping { get; set; }
		public bool IsDetectingSound { get; set; }
        public bool IsPasswordCorrect { get; set; }
        public bool IsAlarmWorking { get; set; }

		public Alert Detected { get; set; }

        /// <summary>
        /// True if the detectors callbacks have been set
        /// </summary>
        public bool HasLinkedDetector { get; private set; }

        
        //public MailAddress fromAddress = new MailAddress("notif.buddy@gmail.com", "notif");
        //MailAddress toAddress = new MailAddress("wa@bluefrogrobotics.com", "walid");
        //const string fromPassword = "autruchemagiquebuddy";
        //const string subject = "Subject";
        //const string body = "Body";

        
		/// <summary>
		/// Enum of the different alerts that Guardian app can send
		/// </summary>
		public enum Alert : int
		{
			MOVEMENT,
			SOUND,
			FIRE,
			KIDNAPPING
		}



    void Awake()
		{
            mAnimator = GetComponent<Animator>();
			GuardianActivity.Init(mAnimator, this);
        }

		void Start()
		{
			Volume = (int)(Buddy.Actuators.Speakers.Volume * 100F);
            Init();
            //EMail mMail = new EMail("sujet", "truc");
            //mMail.AddTo("wa@bluefrogrobotics.com");
            //Debug.Log("envoi du mail");
            //Buddy.WebServices.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, mMail);
            //SmtpClient smtp = new SmtpClient
            //{
            //    Host = "smtp.gmail.com",
            //    Port = 587,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            //};
            //using (var message = new MailMessage(fromAddress, toAddress)
            //{
            //    Subject = subject,
            //    Body = body
            //})
            //{
            //    smtp.SendAsync(message, null);
            //}
            //RecipientData recipient = new RecipientData();
            //recipient.Name = "rodolphe";
            //recipient.Mail = "rh@bluefrogrobotics.com";
            //RecipientsData contacts = new RecipientsData();
            //contacts.Recipients = new List<RecipientData>();
            //contacts.Recipients.Add(recipient);
            //Utils.SerializeXML<RecipientsData>(contacts, Buddy.Resources.GetRawFullPath("contacts.xml"));
        }


		/// <summary>
		/// Init the detectors and the roomba navigation
		/// </summary>
		public void Init()
		{
            HasLinkedDetector = false;

            mMotionDetection = Buddy.Perception.MotionDetector;
            mNoiseDetection = Buddy.Perception.NoiseDetector;
            mFireDetection = Buddy.Perception.ThermalDetector;
            mKidnappingDetection = Buddy.Perception.KidnappingDetector;

            //Roomba = BYOS.Instance.Navigation.Roomba;
            //Roomba.enabled = false;
        }

        private void Update()
        {
            //Debug.Log("mail busy: " + Buddy.WebServices.EMailSender.IsBusy);
        }

        /// <summary>
        /// Add a string to the log string
        /// </summary>
        /// <param name="iLog"></param>
        public void AddLog(string iLog)
		{
			Logs += iLog + "\n";
		}

		/// <summary>
		/// Subscribe to the detectors callbacks
		/// </summary>
		public void LinkDetectorsEvents()
		{
            Debug.Log("link detector events");
            MotionDetectorParameter lMotionParam = new MotionDetectorParameter();
            lMotionParam.SensibilityThreshold = GuardianData.Instance.MovementDetectionThreshold * MAX_MOVEMENT_THRESHOLD / 100;
            lMotionParam.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 640, 480);
            HasLinkedDetector = true;
            if (!IsDetectingMovement)
                mMotionDetection.OnDetect.AddP(OnMovementDetected, lMotionParam);
            //if (!IsDetectingSound)
            //{
                Debug.Log("on detecte son");
                mNoiseDetection.OnDetect.AddP(OnSoundDetected, 0.0F);
            //}
            //mFireDetection.OnDetect.AddP(OnThermalDetected, 50);
            //mKidnappingDetection.OnDetect.Add(OnKidnappingDetected, KIDNAPPING_THRESHOLD);
            //Buddy.Sensors.RGBCamera.Mode = RGBCameraMode.COLOR_320x240_30FPS_RGB;
        }

		/// <summary>
		/// Unsubscibe to the detectors callbacks
		/// </summary>
		public void UnlinkDetectorsEvents()
		{
            Debug.Log("unlink detector events");
            HasLinkedDetector = false;
            //mKidnappingDetection.OnDetect.Remove(OnKidnappingDetected);
            //mFireDetection.OnDetect.RemoveP(OnThermalDetected);
            //if (!IsDetectingSound)
                mNoiseDetection.OnDetect.RemoveP(OnSoundDetected);
            if (!IsDetectingMovement)
                mMotionDetection.OnDetect.RemoveP(OnMovementDetected);
        }

		/// <summary>
		/// Called when fire has been detected
		/// </summary>
		private bool OnThermalDetected(ObjectEntity[] iObject)
		{
			if (!IsDetectingFire)
				return true;

			Detected = Alert.FIRE;
			mAnimator.SetTrigger("Alert");
            return true;
		}

		/// <summary>
		/// Called when noise has been detected
		/// </summary>
		private bool OnSoundDetected(float iSound)
		{
			Debug.Log("============== Sound detected! detector");
			if (!IsDetectingSound)
				return true;
            //Debug.Log("iSound: " + iSound + " thresh: " + (1 - ((float)GuardianData.Instance.SoundDetectionThreshold / 100.0f)) * MAX_SOUND_THRESHOLD);
			if (iSound > (1 - ((float)GuardianData.Instance.SoundDetectionThreshold / 100.0f)) * MAX_SOUND_THRESHOLD) {
				Debug.Log("============== Threshold passed!");
				Detected = Alert.SOUND;
				mAnimator.SetTrigger("Alert");
			}
			return true;
		}

		/// <summary>
		/// Called when movement has been detected
		/// </summary>
		private bool OnMovementDetected(MotionEntity[] iMotions)
		{
            if (!IsDetectingMovement || iMotions.Length<3)
				return true;

			Detected = Alert.MOVEMENT;
			mAnimator.SetTrigger("Alert");
            
            return true;
		}

		/// <summary>
		/// Called when buddy is being kidnapped
		/// </summary>
		private void OnKidnappingDetected()
		{
			if (!IsDetectingKidnapping)
				return;

			Detected = Alert.KIDNAPPING;
			mAnimator.SetTrigger("Alert");
            //return true;
		}

        private void OnMediaSaved()
        {

        }

        public void OnMailSent()
        {
            Debug.Log("le mail a ete fabuleusement envoye");
        }
    }
}
