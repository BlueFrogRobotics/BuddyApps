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

		private TextField mUrlAPI;
		private TextField mUserID;
		private Password mPassword;

		private Dropdown mLanguage;

        public override void Build()
        {	
			mSectionTitle = new Dictionary<string,Label>();
			mButtons = new Dictionary<string, LabeledButton>();
			mCheckBoxes = new Dictionary<string,OnOff>();

			// Add some blank space to place first elements right under the application title
			CreateWidget<Blank>();
			CreateWidget<Blank>();
			CreateWidget<Blank>();

			BuildConfigurationSection();
			CreateWidget<Blank>();
			BuildIOTSection();
			CreateWidget<Blank>();
			BuildLanguageSection();
			CreateWidget<Blank>();
			BuildActionSection();
			CreateWidget<Blank>();
			BuildStopSection();

			foreach(string label in mButtons.Keys)
			{
				mButtons[label].OuterLabel = "";
				mButtons[label].OnClickEvent(() => 
					{ 
						Debug.Log("Triggering action : " + label); 
						ExperienceCenterData.Instance.Command = label;
					});
			}
		}

        public override void LabelizeWidgets()
        {
			//TODO Use Buddy SDK dictionary to translate labels (FR/EN)
			mUrlAPI.Label = "REST API URL";
			mUrlAPI.EmptyText = "Enter targeted API Url";
			mUserID.Label = "USER ID";
			mUserID.EmptyText = "someone@somewhere.space";
			mPassword.Label = "USER PASSWORD";
			mPassword.EmptyText = "Enter your password";

			mLanguage.Label = "LANGUAGE";

			foreach(string label in mButtons.Keys)
				mButtons[label].InnerLabel = label;

			foreach(string title in mSectionTitle.Keys)
				mSectionTitle[title].Text = title;

			foreach (string label in mCheckBoxes.Keys)
				mCheckBoxes[label].Label = label;
        }

		private void BuildConfigurationSection()
		{
			AddSectionTitle("REST API Configuration");
			// REST API Url text field
			mUrlAPI = CreateWidget<TextField>();
			if (ExperienceCenterData.Instance.API_URL != "")
				mUrlAPI.FieldText = ExperienceCenterData.Instance.API_URL;
			mUrlAPI.OnEndEditEvent((string text) =>
				{
					ExperienceCenterData.Instance.API_URL = text;
				});

			// REST API User Id text field
			mUserID = CreateWidget<TextField>();
			if (ExperienceCenterData.Instance.UserID != "")
				mUserID.FieldText = ExperienceCenterData.Instance.UserID;
			mUserID.OnEndEditEvent((string text) =>
				{
					ExperienceCenterData.Instance.UserID = text;
				});

			// REST API User Password field
			mPassword = CreateWidget<Password>();
			if (ExperienceCenterData.Instance.Password != "")
				mPassword.FieldText = ExperienceCenterData.Instance.Password;
			mPassword.OnEndEditEvent((string text) =>
				{
					ExperienceCenterData.Instance.Password = text;
				});
		}

		private void BuildIOTSection()
		{
			AddSectionTitle("IOT");

			mCheckBoxes.Add("Light", CreateWidget<OnOff>());
			mCheckBoxes["Light"].IsActive = ExperienceCenterData.Instance.LightState;
			mCheckBoxes.Add("Store", CreateWidget<OnOff>());
			mCheckBoxes["Store"].IsActive = ExperienceCenterData.Instance.StoreState;
			mCheckBoxes.Add("Sonos", CreateWidget<OnOff>());
			mCheckBoxes["Sonos"].IsActive = ExperienceCenterData.Instance.SonosState;

			// Disable by default
			foreach (string device in mCheckBoxes.Keys)
			{
				mCheckBoxes[device].OnSwitchEvent((bool iVal) =>
					{
						Debug.Log(String.Format("Enable {0} : {1}",device,iVal));
						OnOffCallback(device,iVal);
					});
			}
		}

		private void BuildActionSection()
		{
			AddSectionTitle("COMMANDS");

			mButtons.Add("Welcome", CreateWidget<LabeledButton>());
			mButtons.Add("ByeBye", CreateWidget<LabeledButton>());
			mButtons.Add("MoveForward", CreateWidget<LabeledButton>());
			mButtons.Add("IOT", CreateWidget<LabeledButton>());
			mButtons.Add("StopMoving", CreateWidget<LabeledButton>());
			mButtons.Add("StartMoving", CreateWidget<LabeledButton>());
		}

		private void BuildLanguageSection()
		{
			mLanguage = CreateWidget<Dropdown>();
			mLanguage.AddOption("Français");
			mLanguage.AddOption("English");

			mLanguage.OnSelectEvent((string iLabel, object iAttachedObj, int iIndex) =>
				{
					Debug.Log("Selected language : " + iLabel);
					ExperienceCenterData.Instance.Language = iLabel;
				});
		}

		private void BuildStopSection()
		{
			AddSectionTitle("STOP COMMANDS");

			mButtons.Add("EmergencyStop", CreateWidget<LabeledButton>());
			mButtons.Add("Stop", CreateWidget<LabeledButton>());
		}

		private void AddSectionTitle(string label)
		{
			mSectionTitle.Add(label,CreateWidget<Label>());
			mSectionTitle[label].FontStyle = FontStyle.Bold;
		}

		private void OnOffCallback(string device, bool status)
		{
			ExperienceCenterData.Instance.ShouldTestIOT = true;
			if(device=="Light")
				ExperienceCenterData.Instance.LightState = status;
			else if(device=="Store")
				ExperienceCenterData.Instance.StoreState = status;
			else if(device=="Sonos")
				ExperienceCenterData.Instance.SonosState = status;
		}
    }
}