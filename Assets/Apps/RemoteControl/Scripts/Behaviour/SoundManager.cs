﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.RemoteControl
{
	public class SoundManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject microOn;

	    [SerializeField]
	    private GameObject microOff;

		[SerializeField]
		private GameObject webRTC = null;

		/*[SerializeField]
		private PoolManager mPoolManager = null;*/

		private bool mActive = true;

		public void onToggleSound()
		{
			if (webRTC.activeSelf) {
				mActive = !mActive;
	            microOn.SetActive(mActive);
	            microOff.SetActive(!mActive);

				using (AndroidJavaClass cls = new AndroidJavaClass("com.bfr.unityrtc.Webrtc")) {
					cls.CallStatic("setSoundActive", mActive);
				}
			}
		}
	}
}