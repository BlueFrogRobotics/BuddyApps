using UnityEngine.UI;
using UnityEngine;

using Buddy;
using System;

namespace BuddyApp.RemoteControl
{
	[Serializable]
	class RemoteUser
	{
	    public string Email;
	    public bool AlwaysAccept;
	}

	[Serializable]
	class RemoteUsers
	{
	    public RemoteUser[] Users;
	}

	[RequireComponent(typeof(Animator))]

    /* A basic monobehaviour as "AI" behaviour for your app */
    public class RemoteControlBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private RemoteControlData mAppData;

		[SerializeField]
	    private Animator receiveCallAnim;

	    [SerializeField]
	    private Animator receiveCallTimeAnim;

	    [SerializeField]
	    private Animator backgroundAnim;

	    [SerializeField]
	    private Animator callAnimator;

	    [SerializeField]
	    private Webrtc webRTC;

	    [SerializeField]
	    private Text userCalling;

	    [SerializeField]
	    private Text userCallingTime;

	    [SerializeField]
	    private Dropdown choiceDropdown;

	    private bool mIncomingCallHandled;

	    public void backToLobby()
	    {
	        if (webRTC.ConnectionState == Webrtc.CONNECTION.CONNECTING)
	            webRTC.HangUp();
			AAppActivity.QuitApp();
	    }

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
			//callAnimator.SetTrigger("Open_WCall");
			userCalling.text = Buddy.WebRTCListener.RemoteID;
	        receiveCallAnim.SetTrigger("Open_WReceiveCall");
	        backgroundAnim.SetTrigger("Open_BG");
	        //RemoteUsers lUserList = new RemoteUsers();

	        //StreamReader lstreamReader = new StreamReader(BuddyTools.Utils.GetStreamingAssetFilePath("callRights.txt"));
	        //string lTemp = lstreamReader.ReadToEnd();
	        //lstreamReader.Close();

	        //lUserList = JsonUtility.FromJson<RemoteUsers>(lTemp);
	        //string lRemoteCaller = BuddyOS.Net.WebRTCListener.RemoteID;

	        ////TODO : faire la gestion des droits pour un appel entrant
	        //// il faut lire la liste des users, l'enregistrer après acceptation de l'appel, si changement des autorisations
	        //// Faire une fonction qui confirme et check tout au lieu des trigger toussa toussa

	        //receiveCallAnim.SetTrigger("Open_WReceiveCall");
	        //backgroundAnim.SetTrigger("Open_BG");
	        ////receiveCallTimeAnim.SetTrigger("Close_WReceiveCallTime");
	        ////callAnimator.SetTrigger("Close_WCall");
	        //mIncomingCallHandled = false;
	    }

	    public void GetIncomingCall()
	    {
	        if (mIncomingCallHandled)
	            return;

	        mIncomingCallHandled = true;
	        receiveCallAnim.SetTrigger("Open_WReceiveCall");
	    }

	    public void LaunchCall()
	    {
	        webRTC.gameObject.SetActive(true);
	    }

	    public void StopCall()
	    {
	        if (!mIncomingCallHandled)
	            return;

	        mIncomingCallHandled = false;
	        callAnimator.SetTrigger("Close_WCall");
	    }

	    public void CloseApp()
	    {
			AAppActivity.QuitApp();
	    }
    }
}