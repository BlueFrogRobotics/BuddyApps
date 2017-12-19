using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace BuddyApp.ExperienceCenter
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
	public enum BuddyState
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

	public class AnimatorManager : MonoBehaviour
	{
		
		[SerializeField]
		private Animator mMainAnimator;
		private string mOldState = "";
		private bool mSwitchOnce = true;
		public bool clientConnected = false;
		public Dictionary <string, bool> stateDict;

		public AnimatorManager ()
		{
		}

		void Start ()
		{
			mMainAnimator = GameObject.Find ("AIBehaviour").GetComponent<Animator> ();
			InitStateDict ();
		}

		void Update ()
		{
			if (clientConnected) {
				if (mMainAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Init EC State")) {
					if (mSwitchOnce) {
						mMainAnimator.SetTrigger ("Idle");
						Debug.Log ("[Animator] Switch to State: Idle");
						mSwitchOnce = false;
					}
					stateDict ["Idle"] = true;
				}
				if (mMainAnimator.GetCurrentAnimatorStateInfo (0).IsName (mOldState + " State")) {
					string state = GetTrigger ();
					if (state != "" && state != mOldState) {
						if (mSwitchOnce) {
							mMainAnimator.SetTrigger (state);
							mSwitchOnce = false;
						}

					}
				}
			}
		}

		private string GetTrigger ()
		{
			foreach (KeyValuePair<string, bool> state in stateDict)
				if (state.Value)
					return state.Key;
			return "";
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
			if (stateDict ["Idle"]) {
				switch ((Command)cmd) {
				case Command.Welcome:
				case Command.Questions:
				case Command.ByeBye: 
					{
						UpdateStateDict ((Command)cmd, "Idle"); 
						break;
					}
				default:
					break;
				}
			}
		    
			if (stateDict ["ByeBye"]) {
				switch ((Command)cmd) {
				case Command.MoveForward:
					{
						UpdateStateDict ((Command)cmd, "ByeBye"); 
						break;
					}
				default:
					break;
				}
			}

			if (stateDict ["MoveForward"]) {
				switch ((Command)cmd) {
				case Command.IOT:
					{
						UpdateStateDict ((Command)cmd, "MoveForward"); 
						break;
					}
				default:
					break;
				}
			}
		}


		private void UpdateStateDict (Command cmd, string state )
		{
			Debug.Log ("Running cmd: " + cmd);
			stateDict [state] = false;
			mOldState = state;
			stateDict [cmd.ToString ()] = true;
			Debug.Log ("[Animator] Switch to State: " + cmd.ToString ());
			mSwitchOnce = true;
		}
	}
}

