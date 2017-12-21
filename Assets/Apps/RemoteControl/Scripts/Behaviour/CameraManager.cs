using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.RemoteControl
{
	public class CameraManager : MonoBehaviour
	{		
		[SerializeField]
		private RawImage localVideo;

		[SerializeField]
		private GameObject videoOn;

		[SerializeField]
		private GameObject videoOff;

		[SerializeField]
		private GameObject webRTC = null;

		private bool mActive = true;

		public void onToggleCamera()
		{
			if (webRTC.activeSelf) {
				mActive = !mActive;
				videoOn.SetActive(mActive);
				videoOff.SetActive(!mActive);
				localVideo.gameObject.SetActive(mActive);
			}
		}
	}
}
