using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;
using System;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace BuddyApp.TeleBuddyQuatreDeux
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
        public string IdConnectionTablet { get { return mIdTablet; } }

        public bool PingReceived { get; set; }


        // Use this for initialization
        void Start()
        {
            PingReceived = false;
            mBuddyId = /*Buddy.Platform.RobotUID*/ Buddy.IO.MobileData.IMEI();
            mMovingYes = false;
            mMovingNo = false;
            Buddy.Actuators.Wheels.Locked = true;
            IsInitialised = false;
            //Login();
            mPingId = 0;
            mStaticSteering = true;
            OnAskSteering = InformStaticSteering;
            OnActivateObstacle = SensorsBroadcast;

            mCallRequest = new CallRequest("", "");
            //Debug.LogError("robot id: " + Buddy.Platform.RobotUID);
        }

        // Update is called once per frame
        void Update()
        {
            if (mSensorsBroadcast && !mStaticSteering && (Time.time - mLastBroadcastTime > 1 / SENSORS_BROADCAST_FREQUENCY)) {
                mLastBroadcastTime = Time.time;
                SensorsBroadcastRTM();
            }

            if (Time.time - mPingTime > 5.0F)
                Ping();

            if (Buddy.Actuators.Head.Yes.IsBusy)
                mMovingYes = true;
            else if (mMovingYes) {
                // Robot just stopped
                mMovingYes = false;
                SendYesAngle();
            }

            if (Buddy.Actuators.Head.No.IsBusy)
                mMovingNo = true;
            else if (mMovingNo) {
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
            if (!mStaticSteering)
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

        private void SendNoAngle()
        {
            float lValue = 0F;
            Debug.LogWarning("angle no: " + Buddy.Actuators.Head.No.Angle);
            if (Buddy.Actuators.Head.No.Angle > 0)
                lValue = -Mathf.Abs(Buddy.Actuators.Head.No.Angle / Buddy.Actuators.Head.No.AngleMax);
            else
                lValue = Mathf.Abs(Buddy.Actuators.Head.No.Angle / Buddy.Actuators.Head.No.AngleMin);


            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("informNoAngle", lValue.ToString())));
            Debug.LogWarning("inform no: " + lValue.ToString());
        }

        private void SendYesAngle()
        {
            float lValue = 0F;
            Debug.LogWarning("angle yes: " + Buddy.Actuators.Head.Yes.Angle);
            if (Buddy.Actuators.Head.Yes.Angle > 0)
                lValue = Mathf.Abs(Buddy.Actuators.Head.Yes.Angle / Buddy.Actuators.Head.Yes.AngleMax);
            else
                lValue = -Mathf.Abs(Buddy.Actuators.Head.Yes.Angle / Buddy.Actuators.Head.Yes.AngleMin);//YesHeadHinge.MAX_DOWN_ANGLE;

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
            TeleBuddyQuatreDeuxData.Instance.Ping = mPingId.ToString();
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
            Debug.LogWarning("before crypt tablet ID: " + iIdTablet);
            string lIdTablet = TeleBuddyQuatreDeuxBehaviour.EncodeToSHA256(TeleBuddyQuatreDeuxBehaviour.EncodeToMD5(iIdTablet));
            Debug.LogWarning("New tablet ID: " + lIdTablet);
            mIdTablet = lIdTablet;
        }

        private void InitRTM()
        {
            Debug.Log("INIT - RTMMANAGER");
            Buddy.WebServices.Agoraio.InitRTM(/*TeleBuddyQuatreDeuxBehaviour.APP_ID*/ DBManager.Instance.ListUserStudent[TeleBuddyQuatreDeuxData.Instance.IndexTablet].AppID);//TODO WALID: attendre que la requete zoho soit terminé avant etremplacer par l'app id recu //TODO MC : tout est fait dans connectingstate ButtonClick()
            Buddy.WebServices.Agoraio.OnMessage = OnMessage;

            Debug.Log("INIT fin - RTMMANAGER");
        }

        public void Login()
        {
            Debug.Log("LOGIN - RTMMANAGER");
            StartCoroutine(LoginAsync());
            Debug.Log("LOGIN fin - RTMMANAGER");
        }

        public void Logout()
        {
            Debug.Log("LOGOUT - RTMMANAGER");
            Buddy.WebServices.Agoraio.Logout();
            Debug.Log("LOGOUT fin - RTMMANAGER");
        }

        private void SendRTMMessage(string iMessage)
        {
            if (!iMessage.Contains("ping") && !iMessage.Contains("pingAck"))
                Debug.Log("SENDRTMMANAGER - RTMMANAGER : message: " + iMessage + " idtablet: " + mIdTablet);
            //Debug.LogError("Sent to " + mIdTablet);
            if (string.IsNullOrEmpty(mIdTablet)) {
                Debug.Log(" SENDRTMMANAGER - RTMMANAGER :  Can't send a message, no tablet ID");
                return;
            }
            Buddy.WebServices.Agoraio.SendPeerMessage(mIdTablet, iMessage);
            //Debug.LogError("SENDRTMMANAGER - RTMMANAGER fin");
        }


        private void SendRTMMessage(string iMessage, string iIdTablet)
        {

            Debug.Log("SENDRTMMANAGER 2 - RTMMANAGER : message: " + iMessage);
            Debug.Log("Sent to " + iIdTablet);
            if (string.IsNullOrEmpty(iIdTablet)) {
                Debug.Log("SENDRTMMANAGER 2 - RTMMANAGER : Can't send a message, no tablet ID");
                return;
            }
            Buddy.WebServices.Agoraio.SendPeerMessage(iIdTablet, iMessage);
            Debug.Log("SENDRTMMANAGER 2 - RTMMANAGER fin");
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
                else {
                    WheelsMessage lMessage = Utils.UnserializeJSON<WheelsMessage>(iMessage);
                    float lSpeed = 0F;
                    float lAngular = 0F;
                    if (!float.TryParse(lMessage.speed.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lSpeed)) {
                        Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a float");
                    }
                    if (!float.TryParse(lMessage.angularVelocity.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lAngular)) {
                        Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a float");
                    }

                    Debug.LogWarning("Motion received " + lSpeed + " " + lAngular);

                    WheelsMotion lMotion = new WheelsMotion(lSpeed, -lAngular);
                    if (OnWheelsMotion != null)
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
                            if (OncallRequestAnswer != null)
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
                        if (!Boolean.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            bool lAvailable = true;
                            if (TeleBuddyQuatreDeuxData.Instance.CurrentState == TeleBuddyQuatreDeuxData.States.INCOMMING_CALL_STATE || TeleBuddyQuatreDeuxData.Instance.CurrentState == TeleBuddyQuatreDeuxData.States.CALL_STATE || TeleBuddyQuatreDeuxData.Instance.CurrentState == TeleBuddyQuatreDeuxData.States.CALLING_STATE || TeleBuddyQuatreDeuxData.Instance.CurrentState == TeleBuddyQuatreDeuxData.States.CONNECTING_STATE) {
                                lAvailable = false;
                            } else if (TeleBuddyQuatreDeuxData.Instance.CurrentState == TeleBuddyQuatreDeuxData.States.IDLE_STATE) {
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
                            if (lIntValue == mPingId && OnPing != null) {
                                Debug.Log("################### PING ACK CALLED");
                                OnPing((int)((Time.time - mPingTime) * 1000));
                                PingReceived = true;
                            }
                            if (OnPingWithId != null)
                                OnPingWithId(lIntValue);
                        }

                        break;

                    case "frontalListening":
                        if (!bool.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            if (OnFrontalListening != null)
                                OnFrontalListening(lBoolValue);
                        }
                        break;

                    case "mood":
                        Mood lMood;
                        if (!Enum.TryParse(lMessage.propertyValue, true, out lMood)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a mood");
                        } else {
                            // Set the mood
                            Debug.Log("***************************** DEBUG MOOD OVER PROFIL");
                            Buddy.Behaviour.SetMood(lMood);
                            Debug.Log("***************************** DEBUG MOOD OVER PROFIL 2");
                            // Triggers Callback (needs to hide video canvas)
                            //VideoSurface lVideoSurface = rawVideo.GetComponent<VideoSurface>();
                            //Debug.LogError("***************************** DEBUG MOOD OVER PROFIL 3");
                            //if (lVideoSurface != null)
                            //{
                            //    Debug.LogError("***************************** DEBUG MOOD OVER PROFIL 4 ");
                            //    lVideoSurface.SetEnable(false);
                            //    Debug.LogError("***************************** DEBUG MOOD OVER PROFIL 5");
                            //    Destroy(rawVideo.GetComponent<VideoSurface>());
                            //    Debug.LogError("***************************** DEBUG MOOD OVER PROFIL 6");
                            //}
                            Debug.Log("***************************** DEBUG MOOD OVER PROFIL 7");
                            if (OnMood != null) {
                                Debug.Log("***************************** DEBUG MOOD OVER PROFIL 8");
                                OnMood(lMood);

                            }
                        }

                        break;

                    case "moodBI":
                        BehaviourMovementPattern lPattern;
                        if (mStaticSteering)
                            lPattern = BehaviourMovementPattern.HEAD | BehaviourMovementPattern.EYES;
                        else
                            lPattern = BehaviourMovementPattern.COMPLETE_FREEDOM;
                        if (!Enum.TryParse(lMessage.propertyValue, true, out lMood)) {
                            if (lMessage.propertyValue == "CRY") {
                                lMood = Mood.SAD;
                                Buddy.Behaviour.Interpreter.RunRandom(lMood.ToString().ToLower());//, lPattern);
                                if (OnMoodBI != null)
                                    OnMoodBI(lMood);
                            } else if (lMessage.propertyValue == "SLEEP") {
                                lMood = Mood.TIRED;
                                Buddy.Behaviour.Interpreter.RunRandom(lMood.ToString().ToLower());//, lPattern);
                                if (OnMoodBI != null)
                                    OnMoodBI(lMood);
                            } else
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
                            if (OnHeadNo != null) {
                                mLastCommandTime = Time.time;
                                OnHeadNo(Mathf.Pow(lFloatValue, 5) * -80F);
                            }
                        }
                        break;

                    case "headYesAbsolute":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into an int");
                        } else if (OnHeadYesAbsolute != null) {
                            Debug.LogWarning("Angle Yes received, ask to go " + lFloatValue);

                            OnHeadYesAbsolute(lFloatValue); //Mathf.Lerp(Buddy.Actuators.Head.Yes.AngleMin, Buddy.Actuators.Head.Yes.AngleMax, (lFloatValue + 1.0F) / 2F));
                            //if (lFloatValue > 0)
                            //    OnHeadYesAbsolute(lFloatValue * Buddy.Actuators.Head.Yes.AngleMax);
                            //else
                            //    OnHeadYesAbsolute(lFloatValue * Buddy.Actuators.Head.Yes.AngleMin);

                        }
                        break;


                    case "headNoAbsolute":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else if (OnHeadNoAbsolute != null) {
                            Debug.LogWarning("Angle No received, ask to go " + lFloatValue);

                            OnHeadNoAbsolute(lFloatValue); //- Mathf.Lerp(Buddy.Actuators.Head.No.AngleMin, Buddy.Actuators.Head.No.AngleMax, (lFloatValue + 1.0F) / 2F));
                            //OnHeadNoAbsolute(lFloatValue * Buddy.Actuators.Head.No.AngleMin);
                        }

                        break;

                    case "raiseHand":
                        if (!Boolean.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else if (OnRaiseHand != null) {
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
                        if (!bool.TryParse(lMessage.propertyValue, out lBoolValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            OnTakePhoto(lBoolValue);
                        }
                        break;
                    case "rawPhotoProfile":
                        Debug.Log("photo profiler : " + lMessage.propertyValue);
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
            InitRTM();
            yield return GetToken(mBuddyId);
            //WAIT WALID
            string lId = TeleBuddyQuatreDeuxBehaviour.EncodeToSHA256(TeleBuddyQuatreDeuxBehaviour.EncodeToMD5(mBuddyId));
            Buddy.WebServices.Agoraio.Login(lId, mToken);
            IsInitialised = true;
        }

        private IEnumerator RenewTokenAsync()
        {
            yield return GetToken(mBuddyId);
            Buddy.WebServices.Agoraio.RenewToken(mToken);
        }

        private IEnumerator GetToken(string lId)
        {
            //TODO WALID: remplacer la requete par la requete zoho pour le token rtm //TODO MC : token rtm récup dans ConnectingState Buttonclick()
            yield return new WaitForSeconds(0.1F);
            //mToken = DBManager.Instance.ListUserStudent[TeleBuddyQuatreDeuxData.Instance.IndexTablet].RTMToken;
            mToken = DBManager.Instance.mRobotTokenRTM;


            //string request = GET_TOKEN_URL+ lId;
            //using (UnityWebRequest www = UnityWebRequest.Get(request))
            //{
            //    yield return www.SendWebRequest();
            //    if (www.isHttpError || www.isNetworkError)
            //    {
            //        Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
            //    }
            //    else
            //    {
            //        string lRecoJson = www.downloadHandler.text;
            //        Newtonsoft.Json.Linq.JObject lJsonNode = Utils.UnserializeJSONtoObject(lRecoJson);
            //        Debug.LogError("token: " + lJsonNode["key"]);
            //        mToken = (string)lJsonNode["key"];
            //    }
            //}
        }

        void OnApplicationQuit()
        {
            Debug.Log("ON APPLICATION QUIT");
            Logout();
        }

        private void OnDestroy()
        {
            Debug.Log("ON DESTROY LOGOUT RTM");
            Logout();
        }
    }
}
