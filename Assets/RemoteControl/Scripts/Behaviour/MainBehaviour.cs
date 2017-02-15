using UnityEngine;
using BuddyOS;


[RequireComponent(typeof(Animator))]

public class MainBehaviour : MonoBehaviour
{
    [SerializeField]
    private Animator callAnimator;

    [SerializeField]
    private Webrtc mWebRTC;

    public void backToLobby()
    {
        if (mWebRTC.ConnectionState == Webrtc.CONNECTION.CONNECTING)
            mWebRTC.HangUp();
        BYOS.Instance.AppManager.Quit();
    }

    // Use this for initialization
    void Start()
    {
        callAnimator.SetTrigger("Open_WCall");
    }

}
