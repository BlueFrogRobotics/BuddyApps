using Buddy.UI;
using Buddy.Command;
using Buddy;
using System;
using UnityEngine;

namespace BuddyApp.Guardian
{
	public class GuardianLayout : AWindowLayout
	{
		private LabeledButton mHeadOrientation;
		private OnOff mMovementDetection;
		//private LabeledButton mMovementDebug;
		private GaugeOnOff mSoundDetection;
		private LabeledButton mSoundDebug;
		private OnOff mFireDetection;
		private LabeledButton mFireDebug;
		private OnOff mMobileDetection;
		private OnOff mKidnappingDetection;
		private OnOff mSendMail;
		private Dropdown mContacts;

		public override void Build()
		{
			Title = BYOS.Instance.Dictionary.GetString("detectionparameters");
			CreateWidgets();

			mMovementDetection.IsActive = GuardianData.Instance.MovementDetection;
			//mMovementDetection.DisplayPercentage = true;
			//mMovementDetection.Slider.wholeNumbers = true;
			//mMovementDetection.Slider.maxValue = 100;
			//mMovementDetection.Slider.value = GuardianData.Instance.MovementDetectionThreshold;

			mSoundDetection.IsActive = GuardianData.Instance.SoundDetection;
			mSoundDetection.DisplayPercentage = true;
			mSoundDetection.Slider.wholeNumbers = true;
			mSoundDetection.Slider.maxValue = 100;
			mSoundDetection.Slider.value = GuardianData.Instance.SoundDetectionThreshold;

			mFireDetection.IsActive = GuardianData.Instance.FireDetection;
			mMobileDetection.IsActive = GuardianData.Instance.MobileDetection;
			mKidnappingDetection.IsActive = GuardianData.Instance.KidnappingDetection;
			mSendMail.IsActive = GuardianData.Instance.SendMail;

			RegisterEvents();
			//Debug.Log("==============pre for each contact: " + GuardianData.Instance.Contact.FirstName + " " + GuardianData.Instance.Contact.LastName);

			//foreach (GuardianData.Contacts lContact in Enum.GetValues(typeof(GuardianData.Contacts)))
			//	mContacts.AddOption(lContact.ToString(), lContact);

			// When we use AddOption, the callback onselect is call!!!
			// So we need to save the contact and set it again -_-'
			// Their should be a cleaner way to do this but...

			UserAccount selected = new UserAccount();
			selected = GuardianData.Instance.Contact;

			foreach (UserAccount lUser in BYOS.Instance.DataBase.GetUsers())
				mContacts.AddOption(lUser.FirstName + " " + lUser.LastName, lUser);

			GuardianData.Instance.Contact = selected;

			//Debug.Log("============== post for each contact: " + GuardianData.Instance.Contact.FirstName + " " + GuardianData.Instance.Contact.LastName);

			mContacts.SetDefault(GuardianData.Instance.Contact.FirstName + " " + GuardianData.Instance.Contact.LastName);

			mContacts.enabled = mSendMail.IsActive;

			mHeadOrientation.enabled = !mMobileDetection.IsActive;
        }

		private void CreateWidgets()
		{
			//mMovementDebug = CreateWidget<LabeledButton>();
			mMovementDetection = CreateWidget<OnOff>();
			mKidnappingDetection = CreateWidget<OnOff>();
			mSoundDetection = CreateWidget<GaugeOnOff>();
			mSoundDebug = CreateWidget<LabeledButton>();
			mFireDetection = CreateWidget<OnOff>();
			mFireDebug = CreateWidget<LabeledButton>();
			mMobileDetection = CreateWidget<OnOff>();
			mHeadOrientation = CreateWidget<LabeledButton>();
			mSendMail = CreateWidget<OnOff>();
			mContacts = CreateWidget<Dropdown>();
		}

		public override void LabelizeWidgets()
		{
			mHeadOrientation.OuterLabel = BYOS.Instance.Dictionary.GetString("headorientation");
			mHeadOrientation.InnerLabel = BYOS.Instance.Dictionary.GetString("changeheadorientation");
			//mMovementDebug.OuterLabel = BYOS.Instance.Dictionary.GetString("movementsensibility");
			//mMovementDebug.InnerLabel = BYOS.Instance.Dictionary.GetString("sensibilitysettings");
			mFireDebug.OuterLabel = BYOS.Instance.Dictionary.GetString("testfiredetection");
			mFireDebug.InnerLabel = BYOS.Instance.Dictionary.GetString("thermicview");
			mSoundDebug.OuterLabel = BYOS.Instance.Dictionary.GetString("noisesensibility");
			mSoundDebug.InnerLabel = BYOS.Instance.Dictionary.GetString("sensibilitysettings");
			mMovementDetection.Label = BYOS.Instance.Dictionary.GetString("movementdetection");
			mFireDetection.Label = BYOS.Instance.Dictionary.GetString("firedetection");
			mMobileDetection.Label = BYOS.Instance.Dictionary.GetString("mobile");
			mKidnappingDetection.Label = BYOS.Instance.Dictionary.GetString("kidnappingdetection");
			mSoundDetection.Label = BYOS.Instance.Dictionary.GetString("sounddetection");
			mSendMail.Label = BYOS.Instance.Dictionary.GetString("mailnotif", LoadContext.APP);
			mContacts.Label = BYOS.Instance.Dictionary.GetString("whotocontact");
		}

		private void RegisterEvents()
		{
			mHeadOrientation.OnClickEvent(() => { GuardianData.Instance.HeadOrientation = true; });
			//mMovementDebug.OnClickEvent(() => { GuardianData.Instance.MovementDebug = true; });
			mSoundDebug.OnClickEvent(() => { GuardianData.Instance.SoundDebug = true; });
			mFireDebug.OnClickEvent(() => { GuardianData.Instance.FireDebug = true; });

			//mMovementDetection.OnUpdateEvent((int iVal) => {
			//	GuardianData.Instance.MovementDetectionThreshold = iVal;
			//});

			mSoundDetection.OnSwitchEvent((bool iVal) => {
				GuardianData.Instance.SoundDetection = iVal;
			});

			mSoundDetection.OnUpdateEvent((int iVal) => {
				GuardianData.Instance.SoundDetectionThreshold = iVal;
			});

			mFireDetection.OnSwitchEvent((bool iVal) => {
				GuardianData.Instance.FireDetection = iVal;
			});

			mMovementDetection.OnSwitchEvent((bool iVal) => {
				GuardianData.Instance.MovementDetection = iVal;
			});

			mMobileDetection.OnSwitchEvent((bool iVal) => {
				GuardianData.Instance.MobileDetection = iVal;
				mHeadOrientation.gameObject.SetActive(!iVal);
			});

			mKidnappingDetection.OnSwitchEvent((bool iVal) => {
				GuardianData.Instance.KidnappingDetection = iVal;
			});

			mSendMail.OnSwitchEvent((bool iVal) => {
				GuardianData.Instance.SendMail = iVal;
				mContacts.gameObject.SetActive(iVal);
			});


			mContacts.OnSelectEvent((string iLabel, object iAttachedObj, int iIndex) => {
				GuardianData.Instance.Contact = (UserAccount)iAttachedObj;
				//mContacts.SetDefault(GuardianData.Instance.Contact.FirstName + " " + GuardianData.Instance.Contact.LastName);
			});

		}

		public override void Update()
		{
			//if (!IsDisplayed)
			Debug.Log("Desactive");
		}
	}
}