﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using Buddy;
using Buddy;
using Buddy.Command;

namespace BuddyApp.RemoteControl
{
	public class Webrtc : MonoBehaviour
	{
	    public enum CONNECTION { CONNECTING = 0, DISCONNECTING = 1 };

	    public CONNECTION ConnectionState { get { return mConnectionState; } }

	    [Header("WebRTC")]
	    /// <summary>
	    /// URI locating the crossbar server.
	    /// </summary>
	    [SerializeField]
	    private string mCrossbarUri;

	    /// <summary>
	    /// Crossbar realm used.
	    /// </summary>
	    [SerializeField]
	    private string mRealm;

	    /// <summary>
	    /// Channel to subscribe to on the messaging service.
	    /// </summary>
	    [SerializeField]
	    private string mLocalUser;

	    [SerializeField]
	    private string mRemoteUser;

	    [SerializeField]
	    private string mWebrtcReceiverObjectName;

	    [Header("GUI")]
	    public RawImage mRemoteRawImage = null;
	    public RawImage mLocalRawImage = null;
	    public Text mTextLog = null;

	    /// <summary>
	    /// Android Texture object
	    /// </summary>
		public BuddyApp.RemoteControl.RemoteNativeTexture mRemoteNativeTexture = null;
		public BuddyApp.RemoteControl.LocalNativeTexture mLocalNativeTexture = null;
	    private CONNECTION mConnectionState = CONNECTION.DISCONNECTING;

	    private Mutex mTextureMutex = new Mutex();
	    private char[] mResolutionSeparator = new char[] { '*' };

	    private bool mLocalStreamAdded = false;
	    private bool mIsConnected = false;

	    //For now Startwebrtc is called at init but in the future it will only be 
	    //called when receiving a call request or trying to call someone.
	    // StartWebrtc tries to acquire the camera resource and so the camera
	    // must be released beforehand.
	    void OnEnable()
	    {
	        // Setup and start webRTC
	        SetupWebRTC();
	        StartWebRTC();

			mRemoteNativeTexture = new BuddyApp.RemoteControl.RemoteNativeTexture(640, 480);
			mLocalNativeTexture = new BuddyApp.RemoteControl.LocalNativeTexture(640, 480);

	        // Show the android texture in a Unity raw image
	        mRemoteRawImage.texture = mRemoteNativeTexture.texture;
	        mLocalRawImage.texture = mLocalNativeTexture.texture;
	    }

	    void Update()
	    {
	        // Ask update of android texture

	        mTextureMutex.WaitOne();

	        if (mRemoteNativeTexture != null)
	        {
	            mRemoteNativeTexture.Update();
	        }

	        if (mLocalNativeTexture != null)
	        {
	            mLocalNativeTexture.Update();
	        }

	        mTextureMutex.ReleaseMutex();
	    }

	    void InitLocalTexture(int width, int height)
	    {
	        Debug.Log("WebRTC.InitLocalTexture " + width + "*" + height);

	        mTextureMutex.WaitOne();

	        if (mLocalNativeTexture != null)
	        {
	            mLocalNativeTexture.Destroy();
	        }

			mLocalNativeTexture = new BuddyApp.RemoteControl.LocalNativeTexture(width, height);
	        mLocalRawImage.texture = mLocalNativeTexture.texture;

	        mTextureMutex.ReleaseMutex();
	    }

	    void InitRemoteTexture(int width, int height)
	    {
	        Debug.Log("WebRTC.InitRemoteTexture " + width + "*" + height);

	        mTextureMutex.WaitOne();

	        if (mRemoteNativeTexture != null)
	        {
	            mRemoteNativeTexture.Destroy();
	        }

			mRemoteNativeTexture = new BuddyApp.RemoteControl.RemoteNativeTexture(width, height);
	        mRemoteRawImage.texture = mRemoteNativeTexture.texture;

	        mTextureMutex.ReleaseMutex();
	    }

	    /// <summary>
	    /// Set the settings of webRTC
	    /// Crossbar URI
	    /// Realm
	    /// Unity name object for get webrtc messages
	    /// </summary>
	    public void SetupWebRTC()
	    {
			mLocalUser = BitConverter.ToString(BYOS.Instance.Primitive.Arduino.GetSerialNumber);
			mRemoteUser = WebRTCListener.RemoteID;
	        Debug.Log("Remote caller is " + WebRTCListener.RemoteID);
	        if (mTextLog)
	            mTextLog.text += "setup webrtc" + "\n";

	        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
	        {
	            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
	            {
	                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

	                string file = Application.streamingAssetsPath + "/client_cert";
	                cls.CallStatic("SetupWebrtc", mCrossbarUri, mRealm, jo, mLocalUser, mWebrtcReceiverObjectName, file);
	            }
	        }
	    }

	    /// <summary>
	    /// Start WebRTC connection
	    /// </summary>
	    public void StartWebRTC()
	    {
	        if (mTextLog)
	            mTextLog.text += "Starting webRTC" + "\n";
	        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
	        {
	            cls.CallStatic("StartWebrtc");
	        }
	    }

	    /// <summary>
	    /// Stop WebRTC connection
	    /// </summary>
	    public void StopWebRTC()
	    {
	        mRemoteRawImage.transform.localScale = new Vector3(1, 1, 0);
	        mLocalRawImage.transform.localScale = new Vector3(1, 1, 0);

	        mRemoteNativeTexture.Destroy();
	        mLocalNativeTexture.Destroy();

	        Debug.Log("Stop WebRTC");
	        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
	        {
	            cls.CallStatic("StopWebrtc");
	        }
	    }

	    /// <summary>
	    /// Used to call another user who is listening on channel iChannel.
	    /// </summary>
	    /// <param name="iChannel">The channel the user you want to call is subscribed to</param>
	    public void Call()
	    {

	        Debug.Log("Call : " + mRemoteUser);
	        if (mTextLog)
	            mTextLog.text += "Call : " + mRemoteUser + "\n";
	        // mTextSend.text += "\nCall : " + iChannel;
	        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
	        {
	            cls.CallStatic("Call", mRemoteUser);
	        }

	    }

	    /// <summary>
	    /// Closes a connection to a user.
	    /// </summary>
	    /// <param name="iChannel">The channel the user you want to close the connection with is listening to</param>
	    public void HangUp()
	    {
	        Debug.Log("Hang Up : " + mRemoteUser);
	        if (mTextLog)
	            mTextLog.text += "Hang Up : " + mRemoteUser + "\n";
	        if (mConnectionState == CONNECTION.CONNECTING)
	        {
	            using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
	            {
	                cls.CallStatic("Hangup", mRemoteUser);
	            }
	        }
	    }

	    /// <summary>
	    /// Sends a message to another user through a webrtc datachannel
	    /// (unreliable/unordered) or through the messaging service (reliable)
	    /// If you try to send a message through the datachannel and there is no
	    /// Webrtc direct connection active the message will not be sent.
	    /// </summary>
	    /// <param name="iMessage">The message to send</param>
	    /// <param name="iChannel">The channel of the user you want to send a message to</param>
	    /// <param name="iThroughDataChannel">True if this message is to be sent
	    /// through a webrtc datachannel, false for the messaging service.
	    /// </param>
	    public void SendWithDataChannel(string iMessage)//, string iChannel,bool iThroughDataChannel=true)
	    {
	        bool iThroughDataChannel = true;
	        //Debug.Log("sending message : " + iMessage + " to : " + mRemoteUser);
	        if (mTextLog)
	            mTextLog.text += "sending message : " + iMessage + " to : " + mRemoteUser + "\n";

	        if ((mConnectionState == CONNECTION.CONNECTING) && iThroughDataChannel)
	        {
	            using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
	            {
	                Debug.Log("sending message : " + iMessage + " to : " + mRemoteUser + " through data channel");
	                cls.CallStatic("SendMessage", iMessage, mRemoteUser);
	            }
	        }
	        else if (!iThroughDataChannel)
	        {
	            using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
	            {
	                Debug.Log("sending message : " + iMessage + " to : " + mRemoteUser + " through messaging channel");
	                cls.CallStatic("SendMessage", iMessage, mRemoteUser, false);
	            }
	        }
	    }

	    /// <summary>
	    /// Used by the webrtc library to tell this script wether a direct webrtc connection
	    /// has been opened or closed.
	    /// </summary>
	    /// <param name="iValue">1 for opened, 0 for closed.</param>
	    //DO NOT TOUCH AT THE NAME OF THE FUNCTION, it has direct influence over the effective functioning of sending stuffs
	    public void setMIsWebrtcConnectionActive(string iValue)
	    {
	        if (iValue.Equals("1"))
	        {
	            Debug.Log("Webrtc status : CONNECTING");
	            mConnectionState = CONNECTION.CONNECTING;
	            if (mTextLog)
	                mTextLog.text += "Webrtc connection is ON" + "\n";
	        }
	        else {
	            Debug.Log("Webrtc status : DISCONNECTING");
	            mConnectionState = CONNECTION.DISCONNECTING;
	            if (mTextLog)
	                mTextLog.text += "Webrtc connection OFF" + "\n";
	        }
	    }

	    /// <summary>
	    /// Call or hangup the remote channel in function of the actual state
	    /// </summary>
	    public void ToggleConnection()
	    {
	        if (mConnectionState == CONNECTION.DISCONNECTING)
	            Call();
	        else
	            HangUp();
	    }

	    /// <summary>
	    /// This function is called by the RTC library when it receives a message.
	    /// </summary>
	    /// <param name="iMessage">The message that has been received.</param>
	    public void onMessage(string iMessage)
	    {
	        Debug.Log(iMessage);
	        if (mTextLog)
	            mTextLog.text += "Receive message : " + iMessage + "\n";
	    }

	    /// <summary>
	    /// This function is called by the RTC library when it receives a message.
	    /// </summary>
	    /// <param name="iMessage">The message that has been received.</param>
	    public void onAndroidDebugLog(string iMessage)
	    {
	        Debug.Log("Android Debug : " + iMessage);
	        if (mTextLog)
	            mTextLog.text += "Android Debug : " + iMessage + "\n";

	        if (iMessage.Contains("onStateChange: CLOSING"))
	        {
	            mIsConnected = false;
	        }
	        if (iMessage.Contains("onStateChange: CLOSED"))
	        {
	            mIsConnected = false;
	            //HangUp();
	            Debug.Log("Hung up");
	            //this.gameObject.SetActive(false);
	            GameObject.Find(mWebrtcReceiverObjectName).SetActive(false);
	            Debug.Log("Deactivated RTC Controller");
	            StopWebRTC();
	            Debug.Log("Stopped WebRTC protocol");
				AAppActivity.QuitApp();
	            Debug.Log("Going back home");
	        }

	        if (iMessage.Contains("On Local Stream"))
	            mLocalStreamAdded = true;
	        else if (iMessage.Contains("CONNECTED"))
	            mIsConnected = true;

	        if (mIsConnected && mLocalStreamAdded)
	        {
	            Debug.Log("[RTC] Making call to " + mRemoteUser);
	            Call();
	        }
	    }

	    public void onLocalTextureSizeChanged(string size)
	    {
	        Debug.Log("WebRTC.onLocalTextureSizeChanged " + size);

	        string[] cuts = size.Split(mResolutionSeparator);
	        int width = Int32.Parse(cuts[0]);
	        int height = Int32.Parse(cuts[1]);

	        InitLocalTexture(width, height);
	    }

	    public void onRemoteTextureSizeChanged(string size)
	    {
	        Debug.Log("WebRTC.onRemoteTextureSizeChanged " + size);

	        string[] cuts = size.Split(mResolutionSeparator);
	        int width = Int32.Parse(cuts[0]);
	        int height = Int32.Parse(cuts[1]);

	        InitRemoteTexture(width, height);
	    }
	}
}
