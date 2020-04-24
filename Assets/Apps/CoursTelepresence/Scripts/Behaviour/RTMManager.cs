using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;
using System;
using System.Globalization;

namespace BuddyApp.CoursTelepresence
{


    /// <summary>
    /// This class is in charge of the communication from the robot
    /// to the tablet through RTM communication
    /// </summary>
    public class RTMManager : MonoBehaviour
    {
        // Number of messages per second
        private const float SENSORS_BROADCAST_FREQUENCY = 4;

        private string mIdTablet;
        private bool mSensorsBroadcast;
        private bool mStaticSteering;
        private float mLastBroadcastTime;

        public CallRequest mCallRequest;

        // Use this for initialization
        void Start()
        {
            // TODO: get it from DB
            SetTabletId("tablette123456");

            Login();

            mStaticSteering = true;
            OnAskSteering = InformStaticSteering;
            OnActivateObstacle = SensorsBroadcast;

            mCallRequest = new CallRequest("", "");
            // Just to test
            //AnswerCallRequest(true);
            //RequestConnexion("myChannel", "Gregoire Pole");
        }

        // Update is called once per frame
        void Update()
        {
            if (mSensorsBroadcast && !mStaticSteering && (Time.time - mLastBroadcastTime > 1 / SENSORS_BROADCAST_FREQUENCY)) {
                mLastBroadcastTime = Time.time;

                SensorsBroadcastRTM();
            }
        }




        /////////////////////
        /// UI Mangament ////
        /////////////////////


        /// <summary>
        /// Swap from boutton
        /// </summary>
        /// <param name="iStatic">true if static, false if dynamic</param>
        public void SwapSteering()
        {
            mStaticSteering = !mStaticSteering;
            InformStaticSteering();
        }


        ////////////////////////
        /// SENDING COMMANDS ///
        //////////////////////// 




        /// <summary>
        /// Send a message to the tablet to connect
        /// </summary>
        /// <param name="iChannelId">Channel ID to connect to</param>
        /// <param name="iUserName">Name of the teacher</param>
        public void RequestConnexion(string iChannelId, string iUserName = "")
        {
            Debug.LogWarning("Requesting connexion with " + iChannelId + " " + iUserName);
            SendRTMMessage(Utils.SerializeJSON(new CallRequest(iChannelId, iUserName)));
        }

        /// <summary>
        /// Sending answer to an incoming call
        /// </summary>
        /// <param name="iAnswer">Positive or negative answer</param>
        public void AnswerCallRequest(bool iAnswer)
        {
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("callRequestAnswer", iAnswer.ToString())));
        }


        /// <summary>
        /// Sending Obstacle detection values every 0.5 seconds
        /// </summary>
        /// <param name="iActivate">Activate Sensors Broadcast</param>
        public void SensorsBroadcast(bool iActivate)
        {
            mSensorsBroadcast = iActivate;
        }


        /// <summary>
        /// Inform if the steering is static
        /// </summary>
        public void InformStaticSteering()
        {
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("informStaticSteering", mStaticSteering.ToString())));
        }

        /// <summary>
        /// Inform that the head is at the max right value angle
        /// </summary>
        /// <param name="iMax">Angle is at max value</param>
        public void HeadMaxRight(bool iMax)
        {
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("headMaxRight", iMax.ToString())));
        }


        /// <summary>
        /// Inform that the head is at the max left value angle
        /// </summary>
        /// <param name="iMax">Angle is at max value</param>
        public void HeadMaxLeft(bool iMax)
        {
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("headMaxLeft", iMax.ToString())));
        }


        /// <summary>
        /// Inform that the head is at the max up value angle
        /// </summary>
        /// <param name="iMax">Angle is at max value</param>
        public void HeadMaxUp(bool iMax)
        {
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("headMaxUp", iMax.ToString())));
        }


        /// <summary>
        /// Inform that the head is at the max down value angle
        /// </summary>
        /// <param name="iMax">Angle is at max value</param>
        public void HeadMaxDown(bool iMax)
        {
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("headMaxDown", iMax.ToString())));
        }


        /// <summary>
        /// Inform that BI is finished
        /// </summary>
        private void EndOfBI()
        {
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("moodBIEnded", true.ToString())));
        }


        /// <summary>
        /// Actually sends the sensors value through RTM
        /// </summary>
        private void SensorsBroadcastRTM()
        {
            float obstacleRight = Buddy.Sensors.UltrasonicSensors.Right.FilteredValue;
            // TODO add when available
            float obstacleCenter = 0F; // Buddy.Sensors.UltrasonicSensors.Center.FilteredValue;
            float obstacleLeft = Buddy.Sensors.UltrasonicSensors.Left.FilteredValue;


            SendRTMMessage(Utils.SerializeJSON(new ObstacleSensors(obstacleRight, obstacleCenter, obstacleLeft)));
        }




        //////////////////////////////
        /// Callbacks on reception ///
        //////////////////////////////


        public Action<bool> OncallRequestAnswer { get; set; }
        public Action<bool> OnFrontalListening { get; set; }
        public Action<bool> OnRaiseHand { get; set; }
        public Action<bool> OnActivateZoom { get; set; }
        public Action<int> OnHeadYes { get; set; }
        public Action<int> OnHeadNo { get; set; }
        public Action<int> OnHeadYesAbsolute { get; set; }
        public Action<int> OnHeadNoAbsolute { get; set; }
        public Action<Mood> OnMoodBI { get; set; }
        public Action<Mood> OnMood { get; set; }
        public Action<float> OnMicroThreshold { get; set; }
        public Action<string> OnDisplayMessage { get; set; }
        public Action<string> OnSpeechMessage { get; set; }
        public Action<CallRequest> OncallRequest { get; set; }
        public Action<WheelsMotion> OnWheelsMotion { get; set; }


        // These callback are managed internaly
        private Action OnAskSteering { get; set; }
        private Action<bool> OnActivateObstacle { get; set; }




        /////////////////////
        /// RTM functions ///
        /////////////////////


        public void SetTabletId(string iIdTablet)
        {
            Debug.Log("New tablet ID: " + iIdTablet);
            mIdTablet = iIdTablet;
        }

        private void InitRTM()
        {
            Buddy.WebServices.Agoraio.InitRTM();
            Buddy.WebServices.Agoraio.OnMessage = OnMessage;
        }

        private void Login()
        {
            InitRTM();
            Debug.Log("login");
            //Buddy.WebServices.Agoraio.Login(Buddy.Platform.RobotUID);
            Buddy.WebServices.Agoraio.Login("buddytest");
        }

        public void Logout()
        {
            Buddy.WebServices.Agoraio.Logout();
            Debug.Log("logout");
        }

        private void SendRTMMessage(string iMessage)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Debug.Log("message: " + iMessage);
            Debug.Log("Sent at " + timestamp);
            Buddy.WebServices.Agoraio.SendPeerMessage(mIdTablet, iMessage);
        }


        /// <summary>
        /// This is call when a message is received from the tablet
        /// </summary>
        /// <param name="iMessage"></param>
        private void OnMessage(string iMessage)
        {
            iMessage = iMessage.Replace(",tablette123456", "");
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Debug.Log("message received at " + timestamp);
            Debug.LogWarning("message received content " + iMessage);

            //TODO parse json message




            if (iMessage.Contains("userName")) {
                mCallRequest = Utils.UnserializeJSON<CallRequest>(iMessage);
                OncallRequest(mCallRequest);
            } else if (iMessage.Contains("speed")) {
                OnWheelsMotion(Utils.UnserializeJSON<WheelsMotion>(iMessage));

            } else {
                JsonMessage lMessage = Utils.UnserializeJSON<JsonMessage>(iMessage);

                bool lBoolValue;
                int lIntValue;
                float lFloatValue;

                switch (lMessage.propertyName) {
                    case "callRequestAnswer":
                        if (!Boolean.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            OncallRequestAnswer(lBoolValue);
                        }

                        break;

                    case "frontalListening":
                        if (!bool.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            OnFrontalListening(lBoolValue);
                        }

                        break;

                    case "mood":
                        Mood lMood;
                        if (!Enum.TryParse(lMessage.propertyValue, true, out lMood)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a mood");
                        } else {
                            // Set the mood
                            Buddy.Behaviour.SetMood(lMood);
                            // Triggers Callback (needs to hide video canvas)
                            OnMood(lMood);
                        }

                        break;

                    case "moodBI":
                        if (!Enum.TryParse(lMessage.propertyValue, true, out lMood)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a mood");
                        } else {
                            // Set the mood
                            BehaviourMovementPattern BIMotion = BehaviourMovementPattern.BODY_LOCAL_DISPLACEMENT;

                            if (mStaticSteering)
                                BIMotion = BehaviourMovementPattern.HEAD;

                            Buddy.Behaviour.Interpreter.RunRandom(lMood, BIMotion, Buddy.Behaviour.SetMood(lMood));
                            // Triggers Callback (needs to hide video canvas)
                            OnMoodBI(lMood);
                        }

                        break;

                    case "headYes":
                        if (!int.TryParse(lMessage.propertyValue, out lIntValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into an int");
                        } else {
                            OnHeadYes(lIntValue);
                        }

                        break;

                    case "headNo":
                        if (!int.TryParse(lMessage.propertyValue, out lIntValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            OnHeadNo(lIntValue);
                        }

                        break;

                    case "headYesheadYesAbsolute":
                        if (!int.TryParse(lMessage.propertyValue, out lIntValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into an int");
                        } else {
                            OnHeadYesAbsolute(lIntValue);
                        }

                        break;

                    case "headNoAbsolute":
                        if (!int.TryParse(lMessage.propertyValue, out lIntValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            OnHeadNoAbsolute(lIntValue);
                        }

                        break;

                    case "raiseHand":
                        if (!Boolean.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            OnRaiseHand(lBoolValue);
                        }

                        break;

                    case "speechAndDisplayMessage":
                        OnSpeechMessage(lMessage.propertyValue);
                        OnDisplayMessage(lMessage.propertyValue);
                        break;

                    case "speechMessage":
                        OnSpeechMessage(lMessage.propertyValue);
                        break;

                    case "displayMessage":
                        OnDisplayMessage(lMessage.propertyValue);
                        break;

                    case "microThreshold":
                        if (!float.TryParse(lMessage.propertyValue, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            OnMicroThreshold(lFloatValue);
                        }

                        break;

                    case "askStaticSteering":
                        OnAskSteering();

                        break;


                    case "activateObstacleDetection":
                        if (!bool.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            OnActivateObstacle(lBoolValue);
                        }

                        break;

                    case "activateZoom":
                        if (!bool.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            OnActivateZoom(lBoolValue);
                        }

                        break;

                    default:
                        Debug.LogWarning("UNKNOWN message type " + lMessage.propertyName);
                        break;
                }
            }
        }
    }
}
