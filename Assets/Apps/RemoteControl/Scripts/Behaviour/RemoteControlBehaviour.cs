using UnityEngine.UI;
using UnityEngine;

using Buddy;
using System;
using System.Collections;

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

        [SerializeField]
        private AudioClip musicCall;

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
            //StartCoroutine(Call());
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
            receiveCallAnim.SetTrigger("Close_WReceiveCall");
            backgroundAnim.SetTrigger("Close_BG");
            callAnimator.SetTrigger("Open_WCall");
            webRTC.gameObject.SetActive(true);
	    }

	    public void StopCall()
	    {
            receiveCallAnim.SetTrigger("Close_WReceiveCall");
            if (!mIncomingCallHandled)
	            return;

	        mIncomingCallHandled = false;
	        callAnimator.SetTrigger("Close_WCall");
	    }

	    public void CloseApp()
	    {
			AAppActivity.QuitApp();
	    }

        public IEnumerator Call()
        {
            receiveCallAnim.SetTrigger("Open_WReceiveCall");
            backgroundAnim.SetTrigger("Open_BG");
            if (!RemoteControlData.Instance.DiscreteMode)
            {
                //BYOS.Instance.Primitive.Speaker.FX.Play(FXSound.BEEP_1);
                BYOS.Instance.Primitive.Speaker.Media.Play(musicCall);
                yield return new WaitForSeconds(1.5F);
                string lReceiver = "";
                foreach (UserAccount lUser in BYOS.Instance.DataBase.GetUsers())
                {
                    if(Buddy.WebRTCListener.RemoteID.Trim()==lUser.Email)
                    {
                        lReceiver = lUser.FirstName;
                    }
                }
                string lTextToSay = BYOS.Instance.Dictionary.GetString("incomingcall");
                if (lReceiver == "")
                {
                    lTextToSay = lTextToSay.Replace("[user]", Buddy.WebRTCListener.RemoteID);
                    userCalling.text = Buddy.WebRTCListener.RemoteID;
                }
                else
                {
                    lTextToSay = lTextToSay.Replace("[user]", lReceiver);
                    userCalling.text = lReceiver;
                }
                BYOS.Instance.Interaction.TextToSpeech.Say(lTextToSay);
                
            }
            yield return null;
        }
    }
}