using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.Command;
using BuddyOS;


[RequireComponent(typeof(Animator))]

public class MainBehaviour : MonoBehaviour {

    [SerializeField]
    private Animator callAnimator;

    [SerializeField]
    private Webrtc mWebRTC;

    public void backToLobby()
    {
        Debug.Log("quit application ");
        mWebRTC.hangup();
        BYOS.Instance.AppManager.Quit();
    }

    // Use this for initialization
    void Start () {
        callAnimator.SetTrigger("Open_WCall");
	}
	
}
