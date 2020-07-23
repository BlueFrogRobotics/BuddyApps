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

        private Animator mAnimator;

        private const string GET_TOKEN_URL = "https://teamnet-bfr.ey.r.appspot.com/rtmToken?account=";

        public bool IsInitialised { get; internal set; }

        public int IndexTablet { get; set; }


        // Use this for initialization
        void Start()
        {
            // TODO: get it from DB
            //SetTabletId("tablette123456");
            mBuddyId = Buddy.Platform.RobotUID;
            mMovingYes = false;
            mMovingNo = false;
            Buddy.Actuators.Wheels.Locked = true;
            IsInitialised = false;
            Login();
            mPingId = 0;
            mStaticSteering = true;
            OnAskSteering = InformStaticSteering;
            OnActivateObstacle = SensorsBroadcast;

            mCallRequest = new CallRequest("", "");
            mAnimator = GetComponent<Animator>();
            Debug.LogError("robot id: " + Buddy.Platform.RobotUID);
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
                iImage.sprite = Buddy.Resources.Get<Sprite>("Atlas_Education_Toggle_OFF");
                iImage.color = new Color(0.2F, 0.2F, 0.2F, 1F);
            }
            InformStaticSteering();
        }

        public void SwapSteering(bool iValue)
        {
            mStaticSteering = iValue;
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
                lValue = -Buddy.Actuators.Head.No.Angle / NoHeadHinge.MAX_LEFT_ANGLE;
            else
                lValue = Buddy.Actuators.Head.No.Angle / NoHeadHinge.MAX_RIGHT_ANGLE;


            SendRTMMessage(Utils.SerializeJSON(new JsonMessage("informNoAngle", lValue.ToString())));
            Debug.LogWarning("inform no: " + lValue.ToString());
        }

        private void SendYesAngle()
        {
            float lValue = 0F;
            Debug.LogWarning("angle yes: "+Buddy.Actuators.Head.Yes.Angle);
            if (Buddy.Actuators.Head.Yes.Angle > 0)
                lValue = Buddy.Actuators.Head.Yes.Angle / YesHeadHinge.MAX_UP_ANGLE;
            else
                lValue = Buddy.Actuators.Head.Yes.Angle / 10F;//YesHeadHinge.MAX_DOWN_ANGLE;

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
            StartCoroutine(LoginAsync());
        }

        public void Logout()
        {
            Buddy.WebServices.Agoraio.Logout();
            Debug.Log("logout");
        }

        private void SendRTMMessage(string iMessage)
        {
            Debug.Log("message: " + iMessage);
            Debug.Log("Sent to " + mIdTablet);
            if (string.IsNullOrEmpty(mIdTablet)) {
                Debug.LogWarning("Can't send a message, no tablet ID");
                return;
            }
            Buddy.WebServices.Agoraio.SendPeerMessage(mIdTablet, iMessage);
        }

        private void SendRTMMessage(string iMessage, string iIdTablet)
        {
            Debug.Log("message: " + iMessage);
            Debug.Log("Sent to " + iIdTablet);
            if (string.IsNullOrEmpty(iIdTablet))
            {
                Debug.LogWarning("Can't send a message, no tablet ID");
                return;
            }
            Buddy.WebServices.Agoraio.SendPeerMessage(iIdTablet, iMessage);
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
                    Debug.LogError("Can't move wheels while static steering is on!");
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
                    //OnWheelsMotion(Utils.UnserializeJSON<WheelsMotion>(iMessage));
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
                            if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("INCOMING CALL") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("CALL") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("CALLING"))
                                lAvailable = false;
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
                            // Set the mood
                            //BehaviourMovementPattern BIMotion = BehaviourMovementPattern.BODY_LOCAL_DISPLACEMENT;

                            //if (mStaticSteering)
                            //    BIMotion = BehaviourMovementPattern.HEAD;

                            //Buddy.Behaviour.Interpreter.RunRandom(lMood, BIMotion, Buddy.Behaviour.SetMood(lMood));
                            Debug.LogWarning("moodBI wheels locked " + Buddy.Actuators.Wheels.Locked);
                            //if (lMood == Mood.NEUTRAL)
                            //    Buddy.Behaviour.Interpreter.Run("Neutral07");
                            // else
                            //Buddy.Behaviour.Interpreter.RunRandom(lMood);
                            Buddy.Behaviour.Interpreter.RunRandom(lMood.ToString().ToLower());//, lPattern);

                            // Triggers Callback
                            if (OnMoodBI != null)
                                OnMoodBI(lMood);
                        }

                        break;

                    case "headYes":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into an int");
                        } else {
                            if(OnHeadYes!=null)
                                OnHeadYes(lFloatValue * 10F);
                            Debug.LogWarning("head yes " + lFloatValue);
                        }

                        break;

                    case "headNo":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else {
                            if(OnHeadNo!=null)
                                OnHeadNo(lFloatValue * -15F);
                            Debug.LogWarning("head no " + lFloatValue);
                        } 

                        break;

                    case "headYesAbsolute":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into an int");
                        } else if(OnHeadYesAbsolute!=null){
                            if (lFloatValue > 0)
                                OnHeadYesAbsolute(lFloatValue * YesHeadHinge.MAX_UP_ANGLE);
                            else
                                OnHeadYesAbsolute(lFloatValue * YesHeadHinge.MAX_DOWN_ANGLE);

                        }

                        break;

                    case "headNoAbsolute":
                        if (!float.TryParse(lMessage.propertyValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lFloatValue)) {
                            Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        } else if(OnHeadNoAbsolute!=null){
                            OnHeadNoAbsolute(lFloatValue * NoHeadHinge.MAX_LEFT_ANGLE);
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

                    case "rawPhotoProfile":
                        //if (!string.TryParse(lMessage.propertyValue, out lBoolValue))
                        //{
                        //    Debug.LogWarning(lMessage.propertyName + "value can't be parsed into a bool");
                        //}
                        Debug.LogError("photo profiler");
                        Debug.LogError(lMessage.propertyValue);
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
            Debug.Log("login");
            //Buddy.WebServices.Agoraio.Login(Buddy.Platform.RobotUID);
            //Buddy.WebServices.Agoraio.Login(mBuddyId, mToken);
            Buddy.WebServices.Agoraio.Login(mBuddyId);
            IsInitialised = true;
        }

        private IEnumerator RenewTokenAsync()
        {
            //InitRTM();
            yield return GetToken(mBuddyId);
            //Buddy.WebServices.Agoraio.RenewToken(mToken);
            //Debug.Log("login");
            ////Buddy.WebServices.Agoraio.Login(Buddy.Platform.RobotUID);
            //Buddy.WebServices.Agoraio.Login(mBuddyId);
            //IsInitialised = true;
        }

        private IEnumerator GetToken(string lId)
        {
            string request = GET_TOKEN_URL+ lId;
            using (UnityWebRequest www = UnityWebRequest.Get(request))
            {
                yield return www.SendWebRequest();
                if (www.isHttpError || www.isNetworkError)
                {
                    Debug.Log("Request error " + www.error + " " + www.downloadHandler.text);
                }
                else
                {
                    string lRecoJson = www.downloadHandler.text;
                    Newtonsoft.Json.Linq.JObject lJsonNode = Utils.UnserializeJSONtoObject(lRecoJson);
                    Debug.LogError("token: " + lJsonNode["key"]);
                    mToken = (string)lJsonNode["key"];
                }
            }
        }

        void OnApplicationQuit()
        {
            Logout();
        }
    }
}
