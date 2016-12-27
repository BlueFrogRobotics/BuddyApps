using UnityEngine;
using BuddyOS;
using System.Collections.Generic;

public class RobotController : MonoBehaviour
{

    public enum KindOfControl { None = 0, Remote = 1 };
    public enum ControllerMode { Simplify = 0, Medium = 1, Expert = 2 };

    public KindOfControl mActualKindOfControl = KindOfControl.Remote;
    public ControllerMode mControllerMode = ControllerMode.Simplify;

    private float mYesposition = 0.0f;
    private float mNoposition = 0.0f;

    public Webrtc mWebrtc = null;

    private TextToSpeech TTS;

    private Dictionary mDictionary;

    //public static readonly string[] mSimplifyCmdList = { "l", "r", "f", "b" };
    Dictionary<string, float[]> mCmdSimplifyList = new Dictionary<string, float[]>()
    {
        {"l", new float[3] {-75.0f, 75.0f, 300.0f } },
        {"r", new float[3] { 75.0f, -75.0f, 300.0f } },
        {"f", new float[3] {180.0f, 180.0f, 500.0f } },
        {"b", new float[3] {-180.0f, -180.0f, 500.0f } }
    };

    Dictionary<string[], float> mCmdHeadSimplifyList = new Dictionary<string[], float>()
    {
        {new string[2] { "u", "Yes"}, -5.0f },
        {new string[2] { "t", "Yes"}, 5.0f },
        {new string[2] { "s", "No"}, 5.0f },
        {new string[2] { "d", "No"}, -5.0f }
    };

    Motors mMotors = null;

    private float timer = 0.0f;

    // This function is called when a webrtc data is received
    public void onMessage(string iMessage)
    {


        foreach (var item in mCmdSimplifyList)
        {
            if (iMessage.Equals(item.Key))
                mMotors.Wheels.SetWheelsSpeed(item.Value[0], item.Value[1], (int)item.Value[2]);
        }
        foreach (var item in mCmdHeadSimplifyList)
        {
            if (iMessage.Equals(item.Key[0]))
            {
                if (item.Key[1].Equals("Yes"))
                {
                    mYesposition += item.Value;
                    mMotors.YesHinge.SetPosition(mYesposition);
                }
                else if (item.Key[1].Equals("No"))
                {
                    mNoposition += item.Value;
                    mMotors.NoHinge.SetPosition(mNoposition);
                }
            }
        }

        if (iMessage.Equals("z"))
        {
            mMotors.NoHinge.SetPosition(0);
            mMotors.YesHinge.SetPosition(50);

        }
    }

    // Use this for initialization
    void Start()
    {
        mMotors = BYOS.Instance.Motors;
        TTS = BYOS.Instance.TextToSpeech;
        mDictionary = BYOS.Instance.Dictionary;
    }

    bool sayWho = false;
    // Update is called once per frame
    void Update()
    {
        if (!sayWho && (mWebrtc.connectionState == Webrtc.CONNECTION.CONNECTING))
        {
            TTS.Say(mDictionary.GetString("AlertControl"));
            sayWho = true;
        }

        /*   timer += Time.deltaTime;
           if (timer > 0.5f)
           {
               timer = 0.0f;
               if (mWebrtc.connectionState == Webrtc.CONNECTION.CONNECTING)
               {
                   mWebrtc.sendWithDataChannel(mMotors.NoHinge.CurrentAnglePosition.ToString());
               }
           }*/

    }
}
