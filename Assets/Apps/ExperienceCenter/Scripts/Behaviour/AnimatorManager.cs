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
		Idle = 0x00,
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
		StateResponse = 0x60,
        ServerBusy = 0x50
	}

	public class AnimatorManager : MonoBehaviour
	{
		
		[SerializeField]
		private Animator mMainAnimator;
		private bool mSwitchOnce;
		private string mOldState;
		public Dictionary <string, bool> stateDict;

		void Start ()
		{
			mOldState = "";
			mSwitchOnce = true;
			mMainAnimator = GameObject.Find ("AIBehaviour").GetComponent<Animator> ();
			InitStateDict ();
		}

		void Update ()
		{
			if (TcpServer.clientConnected) {
				if (mMainAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Init EC State")) {
					if (mSwitchOnce) {
						mMainAnimator.SetTrigger ("Idle");
						Debug.Log ("[Animator] Switch to State: Idle");
						mSwitchOnce = false;
						stateDict ["Idle"] = true;
					}
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
		    
			if (stateDict ["Welcome"] ) {
				switch ((Command)cmd) {
				case Command.Stop: 
					{
						UpdateStateDict (Command.Idle, "Welcome"); 
						break;
					}
				default:
					break;
				}
			}

			if (stateDict ["ByeBye"]) {
				switch ((Command)cmd) {
				case Command.MoveForward:
				case Command.Stop: 
					{
						UpdateStateDict (Command.Idle, "ByeBye"); 
						break;
					}
				default:
					break;
				}
			}

			if (stateDict ["Questions"] ) {
				switch ((Command)cmd) {
				case Command.Stop: 
					{
						UpdateStateDict (Command.Idle, "Questions"); 
						break;
					}
				default:
					break;
				}
			}

			if (stateDict ["MoveForward"]) {
				switch ((Command)cmd) {
				case Command.IOT:
				case Command.Stop: 
					{
						UpdateStateDict (Command.Idle, "MoveForward"); 
						break;
					}
				default:
					break;
				}
			}
				
			if (stateDict ["IOT"]) {
				switch ((Command)cmd) {
				case Command.Stop: 
					{
						UpdateStateDict (Command.Idle, "IOT"); 
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

