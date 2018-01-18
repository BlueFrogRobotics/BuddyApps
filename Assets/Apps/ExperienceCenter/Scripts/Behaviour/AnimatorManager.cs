using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
		HighBattery = 0x11,
		Undefined = 0xFF
	}


	public class AnimatorManager : MonoBehaviour
	{
		
		[SerializeField]
		private Animator mMainAnimator;
		private IdleBehaviour mIdleBehaviour;
		private MoveForwardBehaviour mMoveBehaviour;

		private bool mSwitchOnce;
		public bool emergencyStop;
		private string mOldState;
		private TextToSpeech mTTS;
		public Dictionary <State, bool> stateDict;

		void Start ()
		{
			mOldState = "";
			mSwitchOnce = true;
			emergencyStop = false;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mMainAnimator = GameObject.Find ("AIBehaviour").GetComponent<Animator> ();
			mIdleBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IdleBehaviour> ();
			mMoveBehaviour = GameObject.Find ("AIBehaviour").GetComponent<MoveForwardBehaviour> ();

			InitStateDict ();
			ExperienceCenterData.Instance.ShouldSendCommand = false;
			ExperienceCenterData.Instance.Scenario = "Init";
			StartCoroutine (HandleParametersCommands ());
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
			}

			//if (TcpServer.clientConnected) {
			if (mMainAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Init EC State")) {
				if (mSwitchOnce) {
					mMainAnimator.SetTrigger ("Idle");
					ExperienceCenterData.Instance.Scenario = "Idle";
					Debug.Log ("[Animator] Switching to State: Idle");
					mSwitchOnce = false;
					stateDict [State.Idle] = true;
				}
			}
			if (mMainAnimator.GetCurrentAnimatorStateInfo (0).IsName (mOldState + " State")) {
				string state = GetTriggerString ();
				if (mOldState == "Idle") {
						
					if (!mIdleBehaviour.behaviourInit) {
						Debug.LogWarning ("Waiting for Head postion to be initialized !");
						return;
					}
				}
				if (state != "" && state != mOldState) {
					if (mSwitchOnce) {
						mMainAnimator.SetTrigger (state);
						ExperienceCenterData.Instance.Scenario = state;
						mSwitchOnce = false;
					}
				}
			}
//			} else {
//				if (mSwitchOnce) {
//					string stateStr = GetTriggerString ();
//					while (stateStr != "") {
//						if (stateStr == "Idle") {
//							stateDict [State.Idle] = false;
//							Debug.Log ("[Animator] Switching to State: Init EC");
//							mMainAnimator.SetTrigger ("ReInit");
//							stateStr = GetTriggerString ();
//						} else {
//							State state = GetTrigger ();
//							stateDict [state] = false;
//							Debug.Log ("[Animator] Switching to State: Idle");
//							stateDict [State.Idle] = true;
//							mMainAnimator.SetTrigger ("Idle");
//							stateStr = GetTriggerString ();
//						}
//					}
//		
//					mOldState = "";
//					Debug.Log ("Waiting for connection !");
//					mSwitchOnce = false;
//				}
//			}
		}

		public void ConnectionTrigger ()
		{
			mSwitchOnce = true;
		}

		private State GetTrigger ()
		{
			foreach (KeyValuePair<State, bool> pair in stateDict)
				if (pair.Value)
					return pair.Key;
			return State.Undefined;
		}

		private string GetTriggerString ()
		{
			State state = GetTrigger ();
			if (state == State.Undefined)
				return "";
			else
				return state.ToString ();
		}



		private void InitStateDict ()
		{
			stateDict = new Dictionary<State, bool> ();
			stateDict.Add (State.Idle, false);
			stateDict.Add (State.Welcome, false);
			stateDict.Add (State.Questions, false);
			stateDict.Add (State.ByeBye, false);
			stateDict.Add (State.MoveForward, false);
			stateDict.Add (State.IOT, false);
		}

		public void ActivateCmd (byte cmd)
		{
			if (stateDict [State.Idle]) {
				switch ((Command)cmd) {
				case Command.Welcome:
				case Command.Questions:
				case Command.ByeBye: 
					{
						UpdateStateDict (cmd, State.Idle); 
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
		    
			if (stateDict [State.Welcome]) {
				switch ((Command)cmd) {
				case Command.Stop: 
					{
						UpdateStateDict (cmd, State.Welcome); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, State.Welcome); 
						emergencyStop = true;
						break;
					}
				default:
					break;
				}
			}

			if (stateDict [State.ByeBye]) {
				switch ((Command)cmd) {
				case Command.MoveForward:
				case Command.Stop: 
					{
						UpdateStateDict (cmd, State.ByeBye); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, State.ByeBye); 
						emergencyStop = true;
						break;
					}
				default:
					break;
				}
			}

			if (stateDict [State.Questions]) {
				switch ((Command)cmd) {
				case Command.Stop: 
					{
						UpdateStateDict (cmd, State.Questions); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, State.Questions); 
						emergencyStop = true;
						break;
					}
				default:
					break;
				}
			}

			if (stateDict [State.MoveForward]) {
				switch ((Command)cmd) {
				case Command.IOT:
					{
						if (mMoveBehaviour.behaviourEnd)
							UpdateStateDict (cmd, State.MoveForward);
						else
							Debug.LogWarning ("Behaviour MoveForward is still running !");
						break;
					}
				case Command.Stop:
					{
						UpdateStateDict (cmd, State.MoveForward); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, State.MoveForward); 
						emergencyStop = true;
						break;
					}
				default:
					break;
				}
			}
				
			if (stateDict [State.IOT]) {
				switch ((Command)cmd) {
				case Command.Stop: 
					{
						UpdateStateDict (cmd, State.IOT); 
						break;
					}
				case Command.EmergencyStop:
					{
						UpdateStateDict (cmd, State.IOT); 
						emergencyStop = true;
						break;
					}
				default:
					break;
				}
			}
		}


		private void UpdateStateDict (byte b, State fromState)
		{
			Command cmd = (Command)b;
			State toState = (cmd == Command.Stop || cmd == Command.EmergencyStop) ? State.Idle : (State)b;	
			Debug.Log ("Running cmd: " + cmd);
			stateDict [fromState] = false;
			mOldState = fromState.ToString ();
			stateDict [toState] = true;
			Debug.Log ("[Animator] Switching to State: " + toState.ToString ());
			mSwitchOnce = true;
		}

		public State CheckState (byte stateReq)
		{
			switch ((StateReq)stateReq) {
			case StateReq.Scenario:
				{
					return GetTrigger ();
				}
			case StateReq.Language:
				{
					if (BYOS.Instance.Language.CurrentLang.ToString () == "FR")
						return State.French;
					else
						return State.English;
				}
			case StateReq.Battery:
				{
					float level = BYOS.Instance.Primitive.Battery.EnergyLevel;
					if (level <= 25)
						return State.LowBattery;
					else if (level > 25 && level <= 50)
						return State.MiddleBattery;
					else if (level > 50 && level <= 75)
						return State.GoodBattery;
					else
						return State.HighBattery;
				}
			default:
				return State.Undefined;
			}

		}

		// Fire commands enabled from application parameters layout
		private IEnumerator HandleParametersCommands ()
		{
			while (true) {
				if (ExperienceCenterData.Instance.ShouldSendCommand) {
					ActivateCmd ((byte)ExperienceCenterData.Instance.Command);
					ExperienceCenterData.Instance.ShouldSendCommand = false;
				}
				yield return new WaitForSeconds (1.0f);
			}
		}
	}
}

