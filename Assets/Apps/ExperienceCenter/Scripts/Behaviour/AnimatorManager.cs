using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

using Buddy;

namespace BuddyApp.ExperienceCenter
{
	[Flags]
	public enum Mode
	{
		CommandReq = 0x90,
		CommandAck = 0x80,
		StateReq = 0x70,
		StateAck = 0x60,
		ServerBusy = 0x50
	}

	[Flags]
	public enum Command
	{
		Welcome = 0x01,
		Questions = 0x02,
		ByeBye = 0x03,
		MoveForward = 0x04,
		IOT = 0x05,
		English = 0x06,
		French = 0x07,
		Stop = 0x08,
		EmergencyStop = 0x09
	}

	[Flags]
	public enum StateReq
	{
		Scenario = 0x01,
		Language = 0x02,
		Battery = 0x03
	}

	[Flags]
	public enum State
	{
		Idle = 0x00,
		Welcome = 0x01,
		Questions = 0x02,
		ByeBye = 0x03,
		MoveForward = 0x04,
		IOT = 0x05,
		English = 0x06,
		French = 0x07,
		LowBattery = 0x08,
		MiddleBattery = 0x09,
		GoodBattery = 0x10,
		HighBattery= 0x11
	}


	public class AnimatorManager : MonoBehaviour
	{
		
		[SerializeField]
		private Animator mMainAnimator;
		private IdleBehaviour mIdleBehaviour;

		private bool mSwitchOnce;
		public bool emergencyStop;
		private string mOldState;
		private TextToSpeech mTTS;
		public Dictionary <string, bool> stateDict;

		void Start ()
		{
			mOldState = "";
			mSwitchOnce = true;
			emergencyStop = false;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mMainAnimator = GameObject.Find ("AIBehaviour").GetComponent<Animator> ();
			mIdleBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IdleBehaviour> ();
			InitStateDict ();

		}

		void Update ()
		{
			
			if (emergencyStop) {
				BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
				if (mTTS.HasFinishedTalking) {
					Debug.LogWarning ("[EMERGENCY STOP] Run ! ");
					emergencyStop = false;
					ExperienceCenterActivity.QuitApp ();
					return;
			}

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
					if (mOldState == "Idle") {
						
						if (!mIdleBehaviour.behaviourInit) {
							Debug.LogWarning ("Waiting for Head postion to be initialized !");
							return;
						}
					}
					if (state != "" && state != mOldState) {
						if (mSwitchOnce) {
							mMainAnimator.SetTrigger (state);
							mSwitchOnce = false;
						}
					}
				}
			} else {
				if (mSwitchOnce) {
					string state = GetTrigger ();
					while (state != "") {
						if (state == "Idle") {
							stateDict [state] = false;
							Debug.Log ("[Animator] Switch to State: Init EC");
							mMainAnimator.SetTrigger ("ReInit");
							state = GetTrigger ();
						} else {
							stateDict [state] = false;
							Debug.Log ("[Animator] Switch to State: Idle");
							stateDict ["Idle"] = true;
							mMainAnimator.SetTrigger ("Idle");
							state = GetTrigger ();
						}
					}
		
					mOldState = "";
					Debug.Log ("Waiting for connection !");
					mSwitchOnce = false;
				}
			}
		}

		public void ConnectionTrigger ()
		{
			mSwitchOnce = true;
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
						UpdateStateDict (cmd, "Idle"); 
						break;
					}
				case Command.EmergencyStop:
					{
						emergencyStop = true;
						break;
					}
				default:
					break;
				}
			}
		    
			if (stateDict ["Welcome"]) {
				switch ((Command)cmd) {
				case Command.Stop: 
					{
						UpdateStateDict (cmd, "Welcome"); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, "Welcome"); 
						emergencyStop = true;
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
						UpdateStateDict (cmd, "ByeBye"); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, "ByeBye"); 
						emergencyStop = true;
						break;
					}
				default:
					break;
				}
			}

			if (stateDict ["Questions"]) {
				switch ((Command)cmd) {
				case Command.Stop: 
					{
						UpdateStateDict (cmd, "Questions"); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, "Questions"); 
						emergencyStop = true;
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
						UpdateStateDict (cmd, "MoveForward"); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, "MoveForward"); 
						emergencyStop = true;
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
						UpdateStateDict (cmd, "IOT"); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, "IOT"); 
						emergencyStop = true;
						break;
					}
				default:
					break;
				}
			}
		}


		private void UpdateStateDict (byte b, string state)
		{
			Command cmd = (Command)b;
			State st = (cmd == Command.Stop || cmd == Command.EmergencyStop) ? State.Idle : (State)b;	
			Debug.Log ("Running cmd: " + cmd);
			stateDict [state] = false;
			mOldState = state;
			stateDict [st.ToString ()] = true;
			Debug.Log ("[Animator] Switch to State: " + st.ToString ());
			mSwitchOnce = true;
		}
			
	}
}

