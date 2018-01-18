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
		private TextField mTableDistance;
		private TextField mIOTDistance;

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
			mTableDistance.Label = "Table Distance";
			mTableDistance.EmptyText = "-";
			mIOTDistance.Label = "IOT Distance";
			mIOTDistance.EmptyText = "-";

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
			mButtons.Add ("Questions", CreateWidget<LabeledButton> ());
			CreateWidget<Blank> ();
			mButtons.Add ("ByeBye", CreateWidget<LabeledButton> ());
			mButtons.Add ("MoveForward", CreateWidget<LabeledButton> ());
			mButtons.Add ("IOT", CreateWidget<LabeledButton> ());

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
			AddSectionTitle ("Movement");
		
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