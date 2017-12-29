using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Buddy;
using System.IO;
using Buddy.UI;

namespace BuddyApp.TakePhoto
{
	public class SendTwitter : AStateMachineBehaviour
	{
		private bool mNeedListen;
		private bool mFirst;

		bool mPressedYes;
		bool mPressedNo;

		private string mLastSpeech;
		private short mErrorCount;

		//robotHuehuehue
		private const string mToken = "3107499058-DtOkSKQVm9aXk7g8DsT9ZyNKeixWCdQ5bnkuB5y";
		private const string mTokenSecret = "tszMyp6cFjeBb9k9raT7fxuHTCsw0g70eiMhJOmZYeJAG";
		private const string mConsumerKey = "HbjgvAlxXb4F9vPcDHKtxOC6t";
		private const string mConsumerSecret = "PQQrjxJcTs40QA9h5Rwr8rpQuoMp1J6gexgfjNXfJS8wTlC1Ey";

		//buddyrobotEvent
		//private const string mToken = "815872173902598144-hkmlCGIoFySurjBNltb0fRR1LTJCiRM";
		//private const string mTokenSecret = "cE7Ncw8GkRLXzU3Xy0LEAm0u2hXoWSkJOp45rYdKlMkUM";
		//private const string mConsumerKey = "kHvJ5gVnhex7zKcGbB1456Wrz";
		//private const string mConsumerSecret = "BZnxvSyYgo7fRoBYtI8WWfKH9G8lfhVeJo3KdfTUW7ZdekkTdI";

		private List<string> mAcceptSpeech;
		private List<string> mAnOtherSpeech;
		private List<string> mQuitSpeech;
		private List<string> mRefuseSpeech;
		private List<string> mDidntUnderstandSpeech;
		private List<string> mTweetMsg;

		private const string mHashtag = "Mon hashtag";

		private GameObject mCanvasYesNo;
		private GameObject mCanvasBackGround;


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("OnEnter SendTwitter");

			Notifier.Display<SimpleNot>().With(mHashtag,
			BYOS.Instance.Resources.GetSpriteFromAtlas("Ico_Twitter"), Color.blue);

			SendTweet(Dictionary.GetRandomString("tweet") + " " + mHashtag);
			Interaction.TextToSpeech.SayKey("tweetpublished", true);
			Interaction.TextToSpeech.Say(mHashtag, true);

			Trigger("AskPhotoAgain");
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//TODO: deal with failure?
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

		}


		public void SendTweet(string iMsg)
		{
			Debug.Log("Sending tweet");
			byte[] bytes = File.ReadAllBytes(CommonStrings["photoPath"]);
			Texture2D texture = new Texture2D(1, 1);
			texture.LoadImage(bytes);

			Twitter.AccessTokenResponse accessToken = new Twitter.AccessTokenResponse();
			accessToken.Token = mToken;
			accessToken.TokenSecret = mTokenSecret;
			StartCoroutine(Twitter.API.UploadMedia(texture, iMsg, mConsumerKey, mConsumerSecret, accessToken,
													 new Twitter.PostTweetCallback(this.OnPostTweet)));
		}



		void OnPostTweet(bool success)
		{
			Debug.Log("OnPostTweet - " + (success ? "succeeded." : "failed."));
		}


		
		
	}
}