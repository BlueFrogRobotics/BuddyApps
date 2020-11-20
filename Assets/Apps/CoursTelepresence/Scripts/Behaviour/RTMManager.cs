using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;
using System;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace BuddyApp.CoursTelepresence
{
    /// <summary>
    /// This class is in charge of the communication from the robot
    /// to the tablet through RTM communication
    /// </summary>
    public class RTMManager : MonoBehaviour
    {
        [SerializeField]
        private RawImage rawVideo;

        // Number of messages per second
        private const float SENSORS_BROADCAST_FREQUENCY = 4;

        private string mIdTablet;
        private string mBuddyId;
        private string mToken;
        private bool mSensorsBroadcast;
        public bool mStaticSteering;
        private float mLastBroadcastTime;

        public CallRequest mCallRequest;
        private int mPingId;
        private float mPingTime;
        private bool mMovingYes;
        private bool mMovingNo;

        private float mLastCommandTime;
        private float mLastYesCommandTime;
        private const string GET_TOKEN_URL = "https://teamnet-bfr.ey.r.appspot.com/rtmToken?account=";

        public bool IsInitialised { get; internal set; }

        public int IndexTablet { get; set; }


        // Use this for initialization
        void Start()
        {
            float lFloatValue = -0.8f;
            float test = Mathf.Lerp(-10, 37, (lFloatValue + 1.0F) / 2F);
            Debug.LogWarning("test!!!: " + test);

            mBuddyId = Buddy.Platform.RobotUID;
            Debug.LogError("Buddy id: " + mBuddyId);
            mMovingYes = false;
            mMovingNo = false;
            //Buddy.Actuators.Wheels.Locked = true;
            IsInitialised = false;
            Login();
            mPingId = 0;
            mStaticSteering = true;
            OnAskSteering = InformStaticSteering;
            OnActivateObstacle = SensorsBroadcast;

            mCallRequest = new CallRequest("", "");
            Debug.LogError("robot id: " + Buddy.Platform.RobotUID);
        }

        public void SetHeadADroite()
        {
            string msg= "{ \"propertyName\": \"headYesAbsolute\", \"propertyValue\": \"-0.7\" } ";
            OnMessage(msg);
            string msg2 = "{ \"propertyName\": \"headNoAbsolute\", \"propertyValue\": \"0.5\" } ";
            OnMessage(msg2);
        }

        public void SetHeadAGauche()
        {
            string msg = "{ \"propertyName\": \"headYesAbsolute\", \"propertyValue\": \"0.7\" } ";
            OnMessage(msg);
            string msg2 = "{ \"propertyName\": \"headNoAbsolute\", \"propertyValue\": \"-0.5\" } ";
            OnMessage(msg2);
        }

        // Update is called once per frame
        void Update()
        {
            if (mSensorsBroadcast && !mStaticSteering && (Time.time - mLastBroadcastTime > 1 / SENSORS_BROADCAST_FREQUENCY))
            {
                mLastBroadcastTime = Time.time;
                SensorsBroadcastRTM();
            }

            if (Time.time - mPingTime > 5.0F)
                Ping();

            if (Buddy.Actuators.Head.Yes.IsBusy)
                mMovingYes = true;
            else if (mMovingYes)
            {
                // Robot just stopped
                mMovingYes = false;
                SendYesAngle();
            }

            if (Buddy.Actuators.Head.No.IsBusy)
                mMovingNo = true;
            else if (mMovingNo)
            {
                // Robot just stopped
                mMovingNo = false;
                SendNoAngle();
            }
        }

        /////////////////////
        /// UI Mangament ////
        /////////////////////


        /// <summary>
        /// Swap from boutton
        /// </summary>
        /// <param name="iStatic">true if static, false if dynamic</param>
        public void SwapSteering(Image iImage)
        {
            mStaticSteering = !mStaticSteering;
            Buddy.Actuators.Wheels.Locked = mStaticSteering;
            if (mStaticSteering) {
                iImage.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_Toggle_ON");
                iImage.color = new Color(0F, 0.78F, 0.78F, 1F);
            } else {
                Buddy.Actuators.Head.No.ResetPosition();
                iImage.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_Toggle_OFF");
                iImage.color = new Color(0.2F, 0.2F, 0.2F, 1F);
            }
            InformStaticSteering();
        }

        public void SwapSteering(bool iValue)
        {
            mStaticSteering = iValue;
            if(!mStaticSteering)
                Buddy.Actuators.Head.No.ResetPosition();
            Buddy.Actuators.Wheels.Locked = mStaticSteering;
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
        /// Ask if a tablet is available
        /// </summary>
        /// <param name="iMax">Angle is at max value</param>
        public void AskAvailable(string iId)
        {
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("askAvailable", true.ToString())), iId);
        }

        public void SendImage()
        {
            Debug.LogError("avant send picture");
            Buddy.WebServices.Agoraio.SendPicture(mIdTablet, Buddy.Resources.AppSpritesPath + "big-icon.png");
            Debug.LogError("apres send picture");
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

        public void SendNoAngle()
        {
            float lValue = 0F;
            Debug.LogWarning("angle no: " + Buddy.Actuators.Head.No.Angle);
            if (Buddy.Actuators.Head.No.Angle < 0)
                lValue = -Buddy.Actuators.Head.No.Angle / Buddy.Actuators.Head.No.AngleMin;
            else
                lValue = Buddy.Actuators.Head.No.Angle / Buddy.Actuators.Head.No.AngleMax;


            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("informNoAngle", lValue.ToString())));
            Debug.LogWarning("inform no: " + lValue.ToString());
        }

        public void SendYesAngle()
        {
            float lValue = 0F;
            Debug.LogWarning("angle yes: "+Buddy.Actuators.Head.Yes.Angle);
            if (Buddy.Actuators.Head.Yes.Angle > 0)
                lValue = Buddy.Actuators.Head.Yes.Angle / Buddy.Actuators.Head.Yes.AngleMax;
            else
                lValue = Buddy.Actuators.Head.Yes.Angle / Buddy.Actuators.Head.Yes.AngleMin;

            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("informYesAngle", lValue.ToString())));
            Debug.LogWarning("inform yes: " + lValue.ToString());
        }




        /// <summary>
        /// Sending Ping
        /// </summary>
        private void Ping()
        {

            mPingId++;
            mPingId = mPingId % 10000;
            mPingTime = Time.time;
            CoursTelepresenceData.Instance.Ping = mPingId.ToString();
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("ping", mPingId.ToString())));
        }


        public void Ping(string iIdTablet, int iIdPing)
        {
            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("ping", iIdPing.ToString())), iIdTablet);
        }
        
        //////////////////////////////
        /// Callbacks on reception ///
        //////////////////////////////


        public Action<bool> OncallRequestAnswer { get; set; }
        public Action<bool> OnFrontalListening { get; set; }
        public Action<bool> OnRaiseHand { get; set; }
        public Action<bool> OnActivateZoom { get; set; }
        public Action<float> OnHeadYes { get; set; }
        public Action<float> OnHeadNo { get; set; }
        public Action<float> OnHeadYesAbsolute { get; set; }
        public Action<float> OnHeadNoAbsolute { get; set; }
        public Action<int> OnPing { get; set; }
        public Action<int> OnPingWithId { get; set; }
        public Action<Mood> OnMoodBI { get; set; }
        public Action<Mood> OnMood { get; set; }
        public Action<float> OnMicroThreshold { get; set; }
        public Action<string> OnDisplayMessage { get; set; }
        public Action<string> OnSpeechMessage { get; set; }
        public Action<string> OnPictureReceived { get; set; }
        public Action<CallRequest> OncallRequest { get; set; }
        public Action<WheelsMotion> OnWheelsMotion { get; set; }
        public Action<bool> OnTakePhoto { get; set; }


        // These callback are managed internaly
        private Action OnAskSteering { get; set; }
        private Action<bool> OnActivateObstacle { get; set; }


        /////////////////////
        /// RTM functions ///
        /////////////////////


        public void SetTabletId(string iIdTablet)
        {
            Debug.LogWarning("New tablet ID: " + iIdTablet);
            mIdTablet = iIdTablet;
        }

        private void InitRTM()
        {
            Debug.LogError("INIT - RTMMANAGER");
            Buddy.WebServices.Agoraio.InitRTM();
            Buddy.WebServices.Agoraio.OnMessage = OnMessage;
            Debug.LogError("INIT fin - RTMMANAGER");
        }

        private void Login()
        {
            Debug.LogError("LOGIN - RTMMANAGER");
            StartCoroutine(LoginAsync());
            Debug.LogError("LOGIN fin - RTMMANAGER");
        }

        public void Logout()
        {
            Debug.LogError("LOGOUT - RTMMANAGER");
            Buddy.WebServices.Agoraio.Logout();
            Debug.LogError("LOGOUT fin - RTMMANAGER");
        }

        private void SendRTMMessage(string iMessage)
        {
            //Debug.LogError("SENDRTMMANAGER - RTMMANAGER : message: " + iMessage);
            //Debug.LogError("Sent to " + mIdTablet);
            if (string.IsNullOrEmpty(mIdTablet)) {
                Debug.LogError(" SENDRTMMANAGER - RTMMANAGER :  Can't send a message, no tablet ID");
                return;
            }
            Buddy.WebServices.Agoraio.SendPeerMessage(mIdTablet, iMessage);
            //Debug.LogError("SENDRTMMANAGER - RTMMANAGER fin");
        }

        private void SendRTMMessage(string iMessage, string iIdTablet)
        {
            Debug.LogError("SENDRTMMANAGER 2 - RTMMANAGER : message: " + iMessage);
            Debug.LogError("Sent to " + iIdTablet);
            if (string.IsNullOrEmpty(iIdTablet))
            {
                Debug.LogError("SENDRTMMANAGER 2 - RTMMANAGER : Can't send a message, no tablet ID");
                return;
            }
            Buddy.WebServices.Agoraio.SendPeerMessage(iIdTablet, iMessage);
            Debug.LogError("SENDRTMMANAGER 2 - RTMMANAGER fin");
        }

        /// <summary>
        /// This is call when a message is received from the tablet
        /// </summary>
        /// <param name="iMessage"></param>
        private void OnMessage(string iMessage)
        {
            //iMessage = iMessage.Replace("," + mIdTablet, "");
            Debug.LogWarning("message received content " + iMessage);

            if (iMessage.Contains("userName") && !iMessage.Contains("[METARTM]")) {
                mCallRequest = Utils.UnserializeJSON<CallRequest>(iMessage);
                OncallRequest(mCallRequest);
            } else if (iMessage.Contains("speed") && !iMessage.Contains("[METARTM]")) {
                if (mStaticSteering)
                    Debug.LogWarning("Can't move wheels while static steering is on!");
                else
                {
                    WheelsMessage lMessage = Utils.UnserializeJSON<WheelsMessage>(iMessage);
                    float lSpeed = 0F;
                    float lAngular = 0F;
                    if (!float.TryParse(lMessage.speed.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lSpeed))
                    {
                        Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a float");
                    }
                    if (!float.TryParse(lMessage.angularVelocity.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lAngular))
                    {
                        Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a float");
                    }
                    WheelsMotion lMotion = new WheelsMotion(lSpeed, -lAngular); 
                    if(OnWheelsMotion!=null)
                        OnWheelsMotion(lMotion);
                }
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
                            if(OncallRequestAnswer!=null)
                                OncallRequestAnswer(lBoolValue);
                        }
                        break;

                    case "ping":
                        if (!int.TryParse(lMessage.propertyValue, NumberStyles.Any, CultureInfo.InvariantCulture, out lIntValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into an int");
                        } else {
                            SendRTMMessage(Utils.SerializeJSON(
                                new JsonMessage("pingAck", lMessage.propertyValue)));
                        }
                        break;

                    case "askAvailable":
                        if (!Boolean.TryParse(lMessage.propertyValue, out lBoolValue))
                        {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        }
                        else
                        {
                            bool lAvailable = true;
                            if (CoursTelepresenceData.Instance.CurrentState == CoursTelepresenceData.States.INCOMMING_CALL_STATE || CoursTelepresenceData.Instance.CurrentState == CoursTelepresenceData.States.CALL_STATE || CoursTelepresenceData.Instance.CurrentState == CoursTelepresenceData.States.CALLING_STATE)
                            {
                                lAvailable = false;
                            }
                            else if(CoursTelepresenceData.Instance.CurrentState == CoursTelepresenceData.States.IDLE_STATE)
                            {
                                lAvailable = true;
                            }
                            SendRTMMessage(Utils.SerializeJSON(
                                new JsonMessage("informAvailable", lAvailable.ToString())));
                        }

                        break;

                    case "pingAck":
                        if (!int.TryParse(lMessage.propertyValue, NumberStyles.Any, CultureInfo.InvariantCulture, out lIntValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into an int");
                        } else {
                            if (lIntValue == mPingId && OnPing!=null)
                                OnPing((int)((Time.time - mPingTime) * 1000));
                            if(OnPingWithId!=null)
                                OnPingWithId(lIntValue);
                        }

                        break;

                    case "frontalListening":
                        if (!bool.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            if(OnFrontalListening!=null)
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
                            VideoSurface lVideoSurface = rawVideo.GetComponent<VideoSurface>();
                            if (lVideoSurface != null)
                            {
                                lVideoSurface.SetEnable(false);
                                Destroy(rawVideo.GetComponent<VideoSurface>());
                            }
                            if(OnMood!=null)
                                OnMood(lMood);
                        }

                        break;

                    case "moodBI":
                        BehaviourMovementPattern lPattern;
                        if (mStaticSteering)
                            lPattern = BehaviourMovementPattern.HEAD | BehaviourMovementPattern.EYES;
                        else
                            lPattern = BehaviourMovementPattern.COMPLETE_FREEDOM;
                        if (!Enum.TryParse(lMessage.propertyValue, true, out lMood)) {
                            if (lMessage.propertyValue == "CRY")
                            {
                                lMood = Mood.SAD;
                                Buddy.Behaviour.Interpreter.RunRandom(lMood.ToString().ToLower());//, lPattern);
                                if(OnMoodBI != null)
                                    OnMoodBI(lMood);
                            }
                            else if (lMessage.propertyValue == "SLEEP")
                            {
                                lMood = Mood.TIRED;
                                Buddy.Behaviour.Interpreter.RunRandom(lMood.ToString().ToLower());//, lPattern);
                                if (OnMoodBI != null)
                                    OnMoodBI(lMood);
                            }
                            else
                                Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a mood");
                        } else {
                            Debug.LogWarning("moodBI wheels locked " + Buddy.Actuators.Wheels.Locked);
                            Buddy.Behaviour.Interpreter.RunRandom(lMood.ToString().ToLower());
                            if (OnMoodBI != null)
                                OnMoodBI(lMood);
                        }
                        break;

                    case "headYes":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into an int");
                        } else {
                            if (OnHeadYes != null) {
                                OnHeadYes(Mathf.Pow(lFloatValue, 5) * 40F);
                                mLastYesCommandTime = Time.time;
                            }
                        }

                        break;

                    case "headNo":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            if (OnHeadNo!=null)
                            {
                                mLastCommandTime = Time.time;
                                OnHeadNo(Mathf.Pow(lFloatValue, 5) * -80F);
                            }
                        } 
                        break;

                    case "headYesAbsolute":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into an int");
                        } else if(OnHeadYesAbsolute!=null){
                            //if (lFloatValue > 0)
                            OnHeadYesAbsolute(Mathf.Lerp(Buddy.Actuators.Head.Yes.AngleMin, Buddy.Actuators.Head.Yes.AngleMax, (lFloatValue + 1.0F) / 2F));
                            //else
                            //    OnHeadYesAbsolute(lFloatValue * Buddy.Actuators.Head.Yes.AngleMin);
                        }
                        break;

                    case "headNoAbsolute":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else if(OnHeadNoAbsolute!=null){
                            OnHeadNoAbsolute(Mathf.Lerp(-30F, 30F/*Buddy.Actuators.Head.No.AngleMin, Buddy.Actuators.Head.No.AngleMax*/, (lFloatValue + 1.0F) / 2F));
                        }

                        break;

                    case "raiseHand":
                        if (!Boolean.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else if(OnRaiseHand!=null){
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
                        if (lMessage.propertyValue.Contains("[METARTM]")) {
                            Debug.LogWarning("Meta RTM detected");
                            OnMessage(lMessage.propertyValue.Replace("[METARTM]", ""));
                        } else
                            OnDisplayMessage(lMessage.propertyValue);
                        break;

                    case "microThreshold":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
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
                    case "requestPhoto":
                        if (!bool.TryParse(lMessage.propertyValue, out lBoolValue))
                        {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        }
                        else
                        {
                            OnTakePhoto(lBoolValue);
                        }
                        break;

                    case "rawPhotoProfile":
                        Debug.LogError("photo profiler : " + lMessage.propertyValue);
                        if (string.IsNullOrEmpty(lMessage.propertyValue)) {
                            Debug.LogWarning("picture is empty");
                        } else {
                            OnPictureReceived(lMessage.propertyValue);
                        }
                        break;

                    default:
                        Debug.LogWarning("UNKNOWN message type " + lMessage.propertyName);
                        break;
                }
            }
        }

        private IEnumerator LoginAsync()
        {
            Debug.LogError("avant init rtm ");
            InitRTM();
            Debug.LogError("apres init rtm");
            //yield return new WaitForSeconds(3);
            //Debug.LogError("apres wait rtm");
            //Buddy.WebServices.Agoraio.GetToken(GET_TOKEN_URL + mBuddyId);
            yield return GetToken(mBuddyId);
            //mToken = "0067b13f4916a6b43e0b23958b18926d596IACzTupl8hAKEHxO6sZSZ5MODRbiXmO44s8McpjgNNKGKHE51f0AAAAAEAB2Yn0/v9h2XwEA6ANPlXVf";
            Debug.LogError("avant login rtm");
            Buddy.WebServices.Agoraio.Login(mBuddyId, mToken);
            Debug.LogError("apres login rtm");
            IsInitialised = true;
        }

        private IEnumerator RenewTokenAsync()
        {
            yield return GetToken(mBuddyId);
            Buddy.WebServices.Agoraio.RenewToken(mToken);
        }

        private IEnumerator GetToken(string lId)
        {
            float lTime = Time.time;
            
            string request = GET_TOKEN_URL+ lId;
            Debug.LogError("debut get token: "+request);
            using (UnityWebRequest www = UnityWebRequest.Get(request))
            {
                www.useHttpContinue = false;
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Debug.LogError("Request error " + www.error + " " + www.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("fin request");
                    string lRecoJson = www.downloadHandler.text;
                    Debug.LogError("recu: "+lRecoJson);
                    Newtonsoft.Json.Linq.JObject lJsonNode = Utils.UnserializeJSONtoObject(lRecoJson);
                    Debug.LogError("token: " + lJsonNode["key"]);
                    mToken = (string)lJsonNode["key"];
                }
            }
            Debug.LogError("bon ben fini");
            Debug.LogError("temps: " + (Time.time - lTime));
        }

        void OnApplicationQuit()
        {
            Logout();
        }
    }
}
