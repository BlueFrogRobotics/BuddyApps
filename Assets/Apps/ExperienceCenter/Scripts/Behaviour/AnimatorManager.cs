using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using BlueQuark;

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
        ByeBye = 0x02,
        MoveForward = 0x03,
        IOT = 0x04,
        Wander = 0x05,
        English = 0x06,
        French = 0x07,
        Stop = 0x08,
        EmergencyStop = 0x09,
        VocalTrigger = 0x0A,
        Unlock = 0x0B
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
        ByeBye = 0x02,
        MoveForward = 0x03,
        IOT = 0x04,
        Wander = 0x05,
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
        private CollisionDetector mCollisionDetector;
        //private TextToSpeech mTTS;
        //private SpeechToText mSpeechToText;

        private bool mSwitchOnce;
        private bool mSwitchIdleOnce;
        private bool mTrigger;
        private bool mBML;

        private string mOldState;

        private float mBatteryLevel;

        public bool emergencyStop;

        public Dictionary<State, bool> stateDict;

        void Start()
        {
            mOldState = "";
            mSwitchOnce = true;
            mSwitchIdleOnce = true;
            emergencyStop = false;
            //mTTS = BYOS.Instance.Interaction.TextToSpeech;
            //mSpeechToText = BYOS.Instance.Interaction.SpeechToText;
            mMainAnimator = GameObject.Find("AIBehaviour").GetComponent<Animator>();
            mIdleBehaviour = GameObject.Find("AIBehaviour").GetComponent<IdleBehaviour>();
            mMoveBehaviour = GameObject.Find("AIBehaviour").GetComponent<MoveForwardBehaviour>();
            mCollisionDetector = GameObject.Find("AIBehaviour").GetComponent<CollisionDetector>();

            InitStateDict();
            ExperienceCenterData.Instance.ShouldSendCommand = false;
            ExperienceCenterData.Instance.Scenario = "Init";
            StartCoroutine(HandleParametersCommands());

            //mSpeechToText.OnErrorEnum.Clear();
            //mSpeechToText.OnErrorEnum.Add(AvoidLock);
            //BYOS.Instance.Interaction.VocalManager.EnableDefaultErrorHandling = false;
            //BYOS.Instance.Interaction.VocalManager.OnError = AvoidLock;
            Buddy.Vocal.OnTrigger.Add(OnTrigger);
            Buddy.Vocal.OnListeningStatus.Add(AvoidLock);
            Buddy.Behaviour.Face.OnTouchMouth.Add(MouthClicked);

            StartCoroutine(ForceNeutralMood());
        }

        void Update()
        {
            mBatteryLevel = Buddy.Sensors.Battery.ChargingState;

            if (mTrigger != ExperienceCenterData.Instance.VoiceTrigger) {
                mTrigger = ExperienceCenterData.Instance.VoiceTrigger;
                Debug.LogFormat("[EXCENTER][ANIMMANAGER] Voice Trigger = {0}", mTrigger);
            }

            if (ExperienceCenterData.Instance.RunTrigger) {
                //mSpeechToText.Request();
                Buddy.Vocal.Listen("experiencecenter", SpeechRecognitionMode.GRAMMAR_ONLY);
                Debug.Log("[EXCENTER][ANIMMANAGER] Run Trigger ");
                ExperienceCenterData.Instance.RunTrigger = false;
            }

            if (mBML != ExperienceCenterData.Instance.EnableBML) {
                mBML = ExperienceCenterData.Instance.EnableBML;
                Debug.LogFormat("[EXCENTER][ANIMMANAGER] BML active = {0}", mBML);
            }
            if (emergencyStop) {
                //BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
                Buddy.Vocal.EnableTrigger = false;
                if (!Buddy.Vocal.IsSpeaking) {
                    Debug.LogWarning("[EXCENTER][ANIMMANAGER] Run EMERGENCY STOP ! ");
                    emergencyStop = false;
                    ExperienceCenterActivity.QuitApp();
                    return;
                }
            }

            //if (TcpServer.clientConnected) {
            if (mMainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Init EC State")) {
                if (mSwitchOnce) {
                    mMainAnimator.SetTrigger("Idle");
                    ExperienceCenterData.Instance.Scenario = "Idle";
                    Debug.Log("[EXCENTER][ANIMMANAGER] Switching to State: Idle");
                    mSwitchOnce = false;
                    stateDict[State.Idle] = true;
                }
            }
            if (mMainAnimator.GetCurrentAnimatorStateInfo(0).IsName(mOldState + " State")) {
                string state = GetTriggerString();
                if (mOldState == "Idle") {

                    if (!mIdleBehaviour.headPoseInit) {
                        if (mSwitchIdleOnce) {
                            Debug.Log("[EXCENTER][ANIMMANAGER] Waiting for Head postion to be initialized !");
                            mIdleBehaviour.StopBehaviour();
                            mSwitchIdleOnce = false;
                        }
                        return;
                    }
                }
                if (state != "" && state != mOldState) {
                    if (!mSwitchIdleOnce)
                        mSwitchIdleOnce = true;
                    if (mSwitchOnce) {
                        mMainAnimator.SetTrigger(state);
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

        public void ConnectionTrigger()
        {
            mSwitchOnce = true;
        }

        private State GetTrigger()
        {
            foreach (KeyValuePair<State, bool> pair in stateDict)
                if (pair.Value)
                    return pair.Key;
            return State.Undefined;
        }

        private string GetTriggerString()
        {
            State state = GetTrigger();
            if (state == State.Undefined)
                return "";
            else
                return state.ToString();
        }



        private void InitStateDict()
        {
            stateDict = new Dictionary<State, bool>();
            stateDict.Add(State.Idle, false);
            stateDict.Add(State.Welcome, false);
            stateDict.Add(State.ByeBye, false);
            stateDict.Add(State.MoveForward, false);
            stateDict.Add(State.IOT, false);
            stateDict.Add(State.Wander, false);
        }

        public void ActivateCmd(byte cmd)
        {
            if (stateDict[State.Idle]) {
                switch ((Command)cmd) {
                    case Command.Welcome:
                    case Command.ByeBye:
                        //case Command.Wander:	
                        {
                            UpdateStateDict(cmd, State.Idle);
                            break;
                        }
                    case Command.EmergencyStop: {
                            emergencyStop = true;
                            break;
                        }
                    case Command.VocalTrigger: {
                            if (ExperienceCenterData.Instance.VoiceTrigger) {
                                ExperienceCenterData.Instance.RunTrigger = true;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

            if (stateDict[State.Welcome]) {
                switch ((Command)cmd) {
                    case Command.Stop: {
                            UpdateStateDict(cmd, State.Welcome);
                            break;
                        }
                    case Command.EmergencyStop: {
                            UpdateStateDict(cmd, State.Welcome);
                            emergencyStop = true;
                            break;
                        }
                    default:
                        break;
                }
            }

            if (stateDict[State.ByeBye]) {
                switch ((Command)cmd) {
                    case Command.MoveForward:
                    case Command.Stop: {
                            UpdateStateDict(cmd, State.ByeBye);
                            break;
                        }
                    case Command.EmergencyStop: {
                            UpdateStateDict(cmd, State.ByeBye);
                            emergencyStop = true;
                            break;
                        }
                    case Command.VocalTrigger: {
                            if (ExperienceCenterData.Instance.VoiceTrigger) {
                                ExperienceCenterData.Instance.RunTrigger = true;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

            if (stateDict[State.MoveForward]) {
                switch ((Command)cmd) {
                    case Command.IOT: {
                            if (mMoveBehaviour.behaviourEnd)
                                UpdateStateDict(cmd, State.MoveForward);
                            else
                                Debug.LogWarning("[EXCENTER][ANIMMANAGER] Behaviour MoveForward is still running !");
                            break;
                        }
                    case Command.Stop: {
                            UpdateStateDict(cmd, State.MoveForward);
                            break;
                        }
                    case Command.EmergencyStop: {
                            UpdateStateDict(cmd, State.MoveForward);
                            emergencyStop = true;
                            break;
                        }
                    case Command.VocalTrigger: {
                            if (ExperienceCenterData.Instance.VoiceTrigger) {
                                ExperienceCenterData.Instance.RunTrigger = true;
                            }
                            break;
                        }
                    case Command.Unlock: {
                            if (mCollisionDetector.behaviourInit) {
                                Debug.LogWarning("[EXCENTER][ANIMMANAGER] Simulated obstacle !");
                                mCollisionDetector.enableToMove = false;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

            if (stateDict[State.IOT]) {
                switch ((Command)cmd) {
                    case Command.Stop: {
                            UpdateStateDict(cmd, State.IOT);
                            break;
                        }
                    case Command.EmergencyStop: {
                            UpdateStateDict(cmd, State.IOT);
                            emergencyStop = true;
                            break;
                        }
                    case Command.Unlock: {
                            if (mCollisionDetector.behaviourInit) {
                                Debug.LogWarning("[EXCENTER][ANIMMANAGER] Simulated obstacle !");
                                mCollisionDetector.enableToMove = false;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }

            //			if (stateDict [State.Wander]) {
            //				switch ((Command)cmd) {
            //				case Command.Stop: 
            //					{
            //						UpdateStateDict (cmd, State.Wander); 
            //						break;
            //					}
            //				case Command.EmergencyStop:
            //					{
            //						UpdateStateDict (cmd, State.Wander); 
            //						emergencyStop = true;
            //						break;
            //					}
            //				default:
            //					break;
            //				}
            //			}
        }


        private void UpdateStateDict(byte b, State fromState)
        {
            Command cmd = (Command)b;
            State toState = (cmd == Command.Stop || cmd == Command.EmergencyStop) ? State.Idle : (State)b;
            Debug.Log("[EXCENTER][ANIMMANAGER] Running cmd: " + cmd);
            stateDict[fromState] = false;
            mOldState = fromState.ToString();
            stateDict[toState] = true;
            Debug.Log("[EXCENTER][ANIMMANAGER] Switching to State: " + toState.ToString());
            mSwitchOnce = true;
        }

        public State CheckState(byte stateReq)
        {
            switch ((StateReq)stateReq) {
                case StateReq.Scenario: {
                        return GetTrigger();
                    }
                case StateReq.Language: {
                        if (Buddy.Platform.Language.SystemInputLanguage.ISO6391Code==ISO6391Code.FR)
                            return State.French;
                        else
                            return State.English;
                    }
                case StateReq.Battery: {
                        Debug.LogFormat("[EXCENTER][ANIMMANAGER] Battery level: {0}", mBatteryLevel);
                        if (mBatteryLevel <= 25)
                            return State.LowBattery;
                        else if (mBatteryLevel > 25 && mBatteryLevel <= 50)
                            return State.MiddleBattery;
                        else if (mBatteryLevel > 50 && mBatteryLevel <= 75)
                            return State.GoodBattery;
                        else
                            return State.HighBattery;
                    }
                default:
                    return State.Undefined;
            }

        }

        // Fire commands enabled from application parameters layout
        private IEnumerator HandleParametersCommands()
        {
            while (true) {
                if (ExperienceCenterData.Instance.ShouldSendCommand) {
                    ActivateCmd((byte)ExperienceCenterData.Instance.Command);
                    ExperienceCenterData.Instance.ShouldSendCommand = false;
                }
                yield return new WaitForSeconds(1.0f);
            }
        }


        private void AvoidLock(SpeechInputStatus iEvent)
        {
            if (iEvent.IsError)
            {
                Buddy.Actuators.Wheels.Locked = false;
                Debug.LogWarningFormat("[EXCENTER][ANIMMANAGER] ERROR STT: {0}", iEvent.ToString());
            }
        }

        public void MouthClicked()
        {
            string currentTrigger = GetTriggerString();
            if (currentTrigger == "Idle" || currentTrigger == "ByeBye" || currentTrigger == "MoveForward")
                ExperienceCenterData.Instance.RunTrigger = true;
        }

        public void OnTrigger(int iLevel)
        {
            string currentTrigger = GetTriggerString();
            if (currentTrigger == "Idle" || currentTrigger == "ByeBye" || currentTrigger == "MoveForward")
                ExperienceCenterData.Instance.RunTrigger = true;
        }

        private IEnumerator ForceNeutralMood()
        {
            Mood buddyMood = Buddy.Behaviour.Mood;
            //VocalManager lVocalManager = BYOS.Instance.Interaction.VocalManager;
            while (true) {
                if (!Buddy.Vocal.IsListening/*mSpeechToText.HasFinished */&& buddyMood != Mood.NEUTRAL)
                    Buddy.Behaviour.Mood = Mood.NEUTRAL;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}

