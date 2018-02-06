using Buddy.UI;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BuddyApp.ExperienceCenter
{
	public class ExperienceCenterLayout : AWindowLayout
	{
		private Dictionary<string,Label> mSectionTitle;
		private Dictionary<string,LabeledButton> mButtons;
		private Dictionary<string,OnOff> mCheckBoxes;
		private TextField mScenario;

		private OnOff mBaseMovementCheckBox;
		private OnOff mHeadMovementCheckBox;
		private TextField mStopDistance;
		private TextField mNoiseTime;
		private TextField mTableDistance;
		private TextField mIOTDistance;
		private TextField mWelcomeTimeOut;
		private TextField mNoHingeAngle;
		private TextField mNoHingeSpeed;
		private TextField mHeadPoseTolerance;
		private TextField mMaxDistance;
		private TextField mMinDistance;
		private TextField mMinSpeed;
		private TextField mMaxSpeed;
		private OnOff mVoiceTriggerCheckBox;
		private OnOff mRunTriggerCheckBox;
		private OnOff mBMLCheckBox;

		private TextField mUrlAPI;
		private TextField mUserID;
		private Password mPassword;

		private TextField mIPAddress;
		private TextField mStatusTcp;

		public override void Build ()
		{	
			mSectionTitle = new Dictionary<string,Label> ();
			mButtons = new Dictionary<string, LabeledButton> ();
			mCheckBoxes = new Dictionary<string,OnOff> ();

			// Add some blank space to place first elements right under the application title
			CreateWidget<Blank> ();
			CreateWidget<Blank> ();
			CreateWidget<Blank> ();

			BuildStopSection ();
			CreateWidget<Blank> ();
			BuildActionSection ();
			CreateWidget<Blank> ();
			BuildIOTSection ();
			CreateWidget<Blank> ();
			BuildMovementSection ();
			CreateWidget<Blank> ();
			BuildTcpSection ();
			CreateWidget<Blank> ();
			BuildConfigurationSection ();

			foreach (string label in mButtons.Keys) {
				mButtons [label].OuterLabel = "";
				mButtons [label].OnClickEvent (() => { 
					Debug.Log ("Triggering action : " + label); 
					ExperienceCenterData.Instance.Command = (ExperienceCenter.Command)Enum.Parse (typeof(ExperienceCenter.Command), label);
					ExperienceCenterData.Instance.ShouldSendCommand = true;
				});
			}
		}

		public override void LabelizeWidgets ()
		{
			//TODO Use Buddy SDK dictionary to translate labels (FR/EN)
			mUrlAPI.Label = "REST API URL";
			mUrlAPI.EmptyText = "Enter targeted API Url";
			mUserID.Label = "USER ID";
			mUserID.EmptyText = "someone@somewhere.space";
			mPassword.Label = "USER PASSWORD";
			mPassword.EmptyText = "Enter your password";

			mBaseMovementCheckBox.Label = "Base Movement";
			mHeadMovementCheckBox.Label = "Head Movement";
			mStopDistance.Label = "Stop Distance";
			mStopDistance.EmptyText = "-";
			mNoiseTime.Label = "Noise Time";
			mNoiseTime.EmptyText = "-";
			mTableDistance.Label = "Table Distance";
			mTableDistance.EmptyText = "-";
			mIOTDistance.Label = "IOT Distance";
			mIOTDistance.EmptyText = "-";
			mWelcomeTimeOut.Label = "Welcome Time-out";
			mWelcomeTimeOut.EmptyText = "-";
			mNoHingeAngle.Label = "No Hinge Angle";
			mNoHingeAngle.EmptyText = "-";
			mNoHingeSpeed.Label = "No Hinge Speed";
			mNoHingeSpeed.EmptyText = "-";
			mHeadPoseTolerance.Label = "Head Tolerance";
			mHeadPoseTolerance.EmptyText = "-";
			mMaxDistance.Label = "Max Distance";
			mMaxDistance.EmptyText = "-";
			mMinDistance.Label = "Min Distance";
			mMinDistance.EmptyText = "-";
			mMaxSpeed.Label = "Max Speed";
			mMaxSpeed.EmptyText = "-";
			mMinSpeed.Label = "Min Speed";
			mMinSpeed.EmptyText = "-";

			mVoiceTriggerCheckBox.Label = "Voice Trigger";
			mRunTriggerCheckBox.Label = "Run Trigger";
			mBMLCheckBox.Label = "BML";

			foreach (string label in mButtons.Keys)
				mButtons [label].InnerLabel = label;

			foreach (string title in mSectionTitle.Keys)
				mSectionTitle [title].Text = title;

			foreach (string label in mCheckBoxes.Keys)
				mCheckBoxes [label].Label = label;

			mIPAddress.Label = "Address";
			mIPAddress.EmptyText = "-";
			mStatusTcp.Label = "Status";
			mStatusTcp.EmptyText = "-";

			mScenario.Label = "Scenario";
			mScenario.EmptyText = "-";
		}

		private void BuildConfigurationSection ()
		{
			AddSectionTitle ("REST API Configuration");
			// REST API Url text field
			mUrlAPI = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.API_URL != "")
				mUrlAPI.FieldText = ExperienceCenterData.Instance.API_URL;
			mUrlAPI.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.API_URL = text;
			});

			// REST API User Id text field
			mUserID = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.UserID != "")
				mUserID.FieldText = ExperienceCenterData.Instance.UserID;
			mUserID.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.UserID = text;
			});

			// REST API User Password field
			mPassword = CreateWidget<Password> ();
			if (ExperienceCenterData.Instance.Password != "")
				mPassword.FieldText = ExperienceCenterData.Instance.Password;
			mPassword.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.Password = text;
			});
		}

		private void BuildTcpSection ()
		{
			AddSectionTitle ("TCP Server");

			mIPAddress = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.IPAddress != "")
				mIPAddress.FieldText = ExperienceCenterData.Instance.IPAddress;
			mIPAddress.OnEndEditEvent ((string text) => {
				mIPAddress.FieldText = ExperienceCenterData.Instance.IPAddress;
			});

			mStatusTcp = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.StatusTcp != "")
				mStatusTcp.FieldText = ExperienceCenterData.Instance.StatusTcp;
			mStatusTcp.OnEndEditEvent ((string text) => {
				mStatusTcp.FieldText = ExperienceCenterData.Instance.StatusTcp;
			});
			
		}

		private void BuildIOTSection ()
		{
			AddSectionTitle ("IOT");

			mCheckBoxes.Add ("Light", CreateWidget<OnOff> ());
			mCheckBoxes ["Light"].IsActive = ExperienceCenterData.Instance.IsLightOn;
			mCheckBoxes.Add ("Store", CreateWidget<OnOff> ());
			mCheckBoxes ["Store"].IsActive = ExperienceCenterData.Instance.IsStoreDeployed;
			mCheckBoxes.Add ("Sonos", CreateWidget<OnOff> ());
			mCheckBoxes ["Sonos"].IsActive = ExperienceCenterData.Instance.IsMusicOn;

			// Disable by default
			foreach (string device in mCheckBoxes.Keys) {
				mCheckBoxes [device].OnSwitchEvent ((bool iVal) => {
					Debug.Log (String.Format ("Enable {0} : {1}", device, iVal));
					OnOffCallback (device, iVal);
				});
			}
		}

		private void BuildActionSection ()
		{
			AddSectionTitle ("COMMANDS");

			mButtons.Add ("Welcome", CreateWidget<LabeledButton> ());
			CreateWidget<Blank> ();
			mButtons.Add ("ByeBye", CreateWidget<LabeledButton> ());
			mButtons.Add ("MoveForward", CreateWidget<LabeledButton> ());
			mButtons.Add ("IOT", CreateWidget<LabeledButton> ());
			CreateWidget<Blank> ();
			mButtons.Add ("Wander", CreateWidget<LabeledButton> ());
			CreateWidget<Blank> ();
			mScenario = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.Scenario != "")
				mScenario.FieldText = ExperienceCenterData.Instance.Scenario;
			mScenario.OnEndEditEvent ((string text) => {
				mScenario.FieldText = ExperienceCenterData.Instance.Scenario;
			});
		}


		private void BuildMovementSection ()
		{
			AddSectionTitle ("Movement & Debug");
		
			mBaseMovementCheckBox = CreateWidget<OnOff> ();
			mBaseMovementCheckBox.IsActive = ExperienceCenterData.Instance.EnableBaseMovement;

			mBaseMovementCheckBox.OnSwitchEvent ((bool iVal) => {
				Debug.Log (String.Format ("Enable {0} : {1}", "Base Movement", iVal));
				ExperienceCenterData.Instance.EnableBaseMovement = iVal;
			});

			mHeadMovementCheckBox = CreateWidget<OnOff> ();
			mHeadMovementCheckBox.IsActive = ExperienceCenterData.Instance.EnableHeadMovement;

			mHeadMovementCheckBox.OnSwitchEvent ((bool iVal) => {
				Debug.Log (String.Format ("Enable {0} : {1}", "Head Movement", iVal));
				ExperienceCenterData.Instance.EnableHeadMovement = iVal;
			});

			mStopDistance = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.StopDistance != 0.0f)
				mStopDistance.FieldText = Convert.ToString (ExperienceCenterData.Instance.StopDistance);
			mStopDistance.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.StopDistance = (float)Convert.ToDouble (text);
			});

			mNoiseTime = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.NoiseTime!= 0.0f)
				mNoiseTime.FieldText = Convert.ToString (ExperienceCenterData.Instance.NoiseTime);
			mNoiseTime.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.NoiseTime = (float)Convert.ToDouble (text);
			});

			mTableDistance = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.TableDistance != 0.0f)
				mTableDistance.FieldText = Convert.ToString (ExperienceCenterData.Instance.TableDistance);
			mTableDistance.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.TableDistance = (float)Convert.ToDouble (text);
			});

			mIOTDistance = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.IOTDistance != 0.0f)
				mIOTDistance.FieldText = Convert.ToString (ExperienceCenterData.Instance.IOTDistance);
			mIOTDistance.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.IOTDistance = (float)Convert.ToDouble (text);
			});

			mHeadPoseTolerance = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.HeadPoseTolerance != 0.0f)
				mHeadPoseTolerance.FieldText = Convert.ToString (ExperienceCenterData.Instance.HeadPoseTolerance);
			mHeadPoseTolerance.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.HeadPoseTolerance = (float)Convert.ToDouble (text);
			});

			mWelcomeTimeOut = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.WelcomeTimeOut != 0.0f)
				mWelcomeTimeOut.FieldText = Convert.ToString (ExperienceCenterData.Instance.WelcomeTimeOut);
			mWelcomeTimeOut.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.WelcomeTimeOut = (float)Convert.ToDouble (text);
			});

			mNoHingeAngle = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.NoHingeAngle != 0.0f)
				mNoHingeAngle.FieldText = Convert.ToString (ExperienceCenterData.Instance.NoHingeAngle);
			mNoHingeAngle.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.NoHingeAngle = (float)Convert.ToDouble (text);
			});

			mNoHingeSpeed = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.NoHingeSpeed != 0.0f)
				mNoHingeSpeed.FieldText = Convert.ToString (ExperienceCenterData.Instance.NoHingeSpeed);
			mNoHingeSpeed.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.NoHingeSpeed = (float)Convert.ToDouble (text);
			});

			mMaxDistance = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.MaxDistance != 0.0f)
				mMaxDistance.FieldText = Convert.ToString (ExperienceCenterData.Instance.MaxDistance);
			mMaxDistance.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.MaxDistance = (float)Convert.ToDouble (text);
			});

			mMinDistance = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.MinDistance != 0.0f)
				mMinDistance.FieldText = Convert.ToString (ExperienceCenterData.Instance.MinDistance);
			mMinDistance.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.MinDistance = (float)Convert.ToDouble (text);
			});

			mMaxSpeed = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.MaxSpeed != 0.0f)
				mMaxSpeed.FieldText = Convert.ToString (ExperienceCenterData.Instance.MaxSpeed);
			mMaxSpeed.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.MaxSpeed = (float)Convert.ToDouble (text);
			});

			mMinSpeed = CreateWidget<TextField> ();
			if (ExperienceCenterData.Instance.MinSpeed != 0.0f)
				mMinSpeed.FieldText = Convert.ToString (ExperienceCenterData.Instance.MinSpeed);
			mMinSpeed.OnEndEditEvent ((string text) => {
				ExperienceCenterData.Instance.MinSpeed = (float)Convert.ToDouble (text);
			});

			mVoiceTriggerCheckBox = CreateWidget<OnOff> ();
			mVoiceTriggerCheckBox.IsActive = ExperienceCenterData.Instance.VoiceTrigger;

			mVoiceTriggerCheckBox.OnSwitchEvent ((bool iVal) => {
				Debug.Log (String.Format ("Enable {0} : {1}", "Voice Trigger", iVal));
				ExperienceCenterData.Instance.VoiceTrigger = iVal;
			});

			mBMLCheckBox = CreateWidget<OnOff> ();
			mBMLCheckBox.IsActive = ExperienceCenterData.Instance.EnableBML;

			mBMLCheckBox.OnSwitchEvent ((bool iVal) => {
				Debug.Log (String.Format ("Enable {0} : {1}", "BML", iVal));
				ExperienceCenterData.Instance.EnableBML = iVal;
			});

			mRunTriggerCheckBox = CreateWidget<OnOff> ();
			mRunTriggerCheckBox.IsActive = ExperienceCenterData.Instance.RunTrigger;

			mRunTriggerCheckBox.OnSwitchEvent ((bool iVal) => {
				Debug.Log (String.Format ("Enable {0} : {1}", "Run Trigger", iVal));
				ExperienceCenterData.Instance.RunTrigger = iVal;
			});
		}

		private void BuildStopSection ()
		{
			AddSectionTitle ("STOP COMMANDS");

			mButtons.Add ("EmergencyStop", CreateWidget<LabeledButton> ());
			mButtons.Add ("Stop", CreateWidget<LabeledButton> ());
		}

		private void AddSectionTitle (string label)
		{
			mSectionTitle.Add (label, CreateWidget<Label> ());
			mSectionTitle [label].FontStyle = FontStyle.Bold;
		}

		private void OnOffCallback (string device, bool status)
		{
			ExperienceCenterData.Instance.ShouldTestIOT = true;
			if (device == "Light")
				ExperienceCenterData.Instance.LightState = status;
			else if (device == "Store")
				ExperienceCenterData.Instance.StoreState = status;
			else if (device == "Sonos")
				ExperienceCenterData.Instance.SonosState = status;
		}
			
	}
}