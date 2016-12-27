using UnityEngine;
using UnityEngine.UI;
using System;


public class Webrtc : MonoBehaviour
{
    public enum CONNECTION { CONNECTING = 0, DISCONNECTING = 1 };
    private CONNECTION ConnectionState = CONNECTION.DISCONNECTING;


    public CONNECTION connectionState { get { return ConnectionState; } }

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
    private string mWebrtcReceiverObjectName = "UnityWebrtc";

    private bool mIsWebrtcConnectionActive = false;

    public bool WebrtcConnectionActive { get { return mIsWebrtcConnectionActive; }}



    [Header("GUI")]
    public RawImage mRawImage;
    public Text mTextLog;
    public NativeTexture mNativeTexture = null;

    public void SetupWebRTC()
    {
        mTextLog.text += "Setting up Webrtc" + "\n";
        mTextLog.text += "CrossbarIO uri is : "+ mCrossbarUri + "\n";
        mTextLog.text += "Local name is : " + mLocalUser +" Realm is : "+ mRealm + "\n";

        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                cls.CallStatic("SetupWebrtc", mCrossbarUri, mRealm, jo, mLocalUser, mWebrtcReceiverObjectName);
            }
        }
    }

    public void StartWebRTC()
    {
        mTextLog.text += "Starting webRTC" + "\n";
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            cls.CallStatic("StartWebrtc");
        }
    }

    //For now Startwebrtc is called at init but in the future it will only be 
    //called when receiving a call request or trying to call someone.
    // StartWebrtc tries to acquire the camera resource and so the camera
    // must be released beforehand.
    void Start()
    {
        // Setup and start webRTC
        SetupWebRTC();
        StartWebRTC();

        mNativeTexture = new NativeTexture(640, 480);
        // Just for debugging and be sure all is ok
        mNativeTexture.setTextureColor(255, 255, 0);
        // Show the android texture in a Unity raw image
        mRawImage.texture = mNativeTexture.texture;
    }

    void Update()
    {
        // Ask update of android texture
        mNativeTexture.Update();
    }

    /// <summary>
    /// Used to call another user who is listening on channel iChannel.
    /// </summary>
    /// <param name="iChannel">The channel the user you want to call is subscribed to</param>
    public void call()
    {
        Debug.Log("Call : " + mRemoteUser);
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
    public void hangup()
    {
        Debug.Log("Hang Up : " + mRemoteUser);
        mTextLog.text += "Hang Up : " + mRemoteUser + "\n";
        if (mIsWebrtcConnectionActive)
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
    public void sendWithDataChannel(string iMessage)//, string iChannel,bool iThroughDataChannel=true)
    {
        bool iThroughDataChannel = true;
       // Debug.Log("sending message : " + iMessage + " to : " + mRemoteUser);
       // mTextLog.text += "sending message : " + iMessage + " to : " + mRemoteUser + "\n";

        if (this.mIsWebrtcConnectionActive && iThroughDataChannel)
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
            {
                cls.CallStatic("SendMessage", iMessage, mRemoteUser);
            }
        }
        else if (!iThroughDataChannel)
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
            {
                cls.CallStatic("SendMessage", iMessage, mRemoteUser, false);
            }
        }
    }

    /// <summary>
    /// Used by the webrtc library to tell this script a webrtc direct connection
    /// has been opened or closed.
    /// </summary>
    /// <param name="iValue">1 for opened, 0 for closed.</param>
    public void setMIsWebrtcConnectionActive(string iValue)
    {
        Debug.Log("changing connection value:" + iValue);
        if (iValue.Equals("1"))
        {
            this.mIsWebrtcConnectionActive = true;
            ConnectionState = CONNECTION.CONNECTING;
            mTextLog.text += "Webrtc connection is ON" + "\n";
        }
        else {
            this.mIsWebrtcConnectionActive = false;
            ConnectionState = CONNECTION.DISCONNECTING;
            mTextLog.text += "Webrtc connection OFF" + "\n";
        }
    }

    public void ToggleConnection()
    {
        if (ConnectionState == CONNECTION.DISCONNECTING)
        {
            call();
        }
        else
        {
            hangup();
        }
    }


    /// <summary>
    /// This function is called by the RTC library when it receives a message.
    /// </summary>
    /// <param name="iMessage">The message that has been received.</param>
    public void onMessage(string iMessage)
    {
        Debug.Log(iMessage);
        mTextLog.text += "Receive message : " + iMessage + "\n";
    }

    /// <summary>
    /// This function is called by the RTC library when it receives a message.
    /// </summary>
    /// <param name="iMessage">The message that has been received.</param>
    public void onAndroidDebugLog(string iMessage)
    {
        Debug.Log(iMessage);
        mTextLog.text += "Android Debug : " + iMessage + "\n";
    }
}

