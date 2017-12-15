using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;



namespace BuddyApp.ExperienceCenter
{
	public class AnimatorManager : MonoBehaviour
	{
		[Flags]
		public enum Command
		{
			Welcome = 0x01,
			Questions = 0x02,
			ByeBye = 0x03,
			MoveForward = 0x04,
			IOT = 0x05,
			Anglais = 0x06,
			Francais = 0x07,
			Stop = 0x08,
			StopMoving = 0x09,
			StartMoving = 0x10,
			EmergencyStop = 0x11
		}

		[Flags]
		public enum State
		{
			LowBattery = 0x01,
			MiddleBattery = 0x02,
			HighBattery = 0x03
		}

		public enum Mode
		{
			CommandRequest = 0x90,
			CommandResponse = 0x80,
			StateRequest = 0x70,
			StateResponse = 0x60
		}

		[SerializeField]
		private Animator mMainAnimator;

		public Dictionary <string, bool> stateDict;

		public AnimatorManager ()
		{
		}

		void Awake ()
		{
			mMainAnimator = GameObject.Find ("AIBehaviour").GetComponent<Animator> ();
			InitStateDict ();
		}

		private void InitStateDict ()
		{
			stateDict = new Dictionary<string, bool> ();
			stateDict.Add ("Idle", false);
			stateDict.Add ("Welcome", false);
			stateDict.Add ("Questions", false);
			stateDict.Add ("ByeBye", false);
			stateDict.Add ("MoveForward", false);
			stateDict.Add ("IOT", false);
			stateDict.Add ("Walk", false);
		}

		public void ActivateCmd (byte cmd)
		{
			if (stateDict["Idle"]) {
				Debug.Log ("Running cmd: " + (Command)cmd);
				stateDict["Idle"] = false;
				stateDict[((Command)cmd).ToString ()] = true;
			} else
				Debug.Log ("Not in Idle State");
		}

		public string GetTrigger()
		{
			foreach (KeyValuePair<string, bool> state in stateDict)
				if (state.Value) 
					return state.Key;
			return "";
		}


	}
}

