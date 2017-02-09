using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using BuddyOS;


namespace BuddyApp.Memory
{
	public class LinkHandler : MonoBehaviour
	{

		public Animator animator;
		//public MemoryGameLevels gameLevels;
		//public MemoryGameLevel currentLevel;
		public MemoryGameRandomLevel gameLevels;
		public AnimManager mAnimationManager;

		private ClickDelegate clickFace;

		public bool isPlayerTurn;

		public bool mUnloadingScene;
		private Face mFace;

		// Use this for initialization
		void Start()
		{
			Debug.Log("Start Link Handler");
			mFace = BYOS.Instance.Face;

			isPlayerTurn = false;

			//if (BYOS.Instance.LanguageManager.CurrentLang == Language.FRA) {
			//	gameLevels = MemoryGameLevels.Load("Lang/levelsFr.json");
			//} else {
			//	gameLevels = MemoryGameLevels.Load("Lang/levelsEn.json");
			//}

			gameLevels = new MemoryGameRandomLevel();

			mUnloadingScene = false;

			LinkStateMachineBehavior[] b = animator.GetBehaviours<LinkStateMachineBehavior>();

			for (int i = 0; i < b.Length; i++) {
				b[i].link = this;
			}

			Debug.Log("End Link Handler");
		}

		public bool UpdateLevel()
		{
			gameLevels.mCurrentLevel++;
			if (gameLevels.mCurrentLevel > (gameLevels.NbLevels)) {
				// no next level to load
				return false;
			}
			return true;
		}

		public void ResetLevel()
		{
			gameLevels.mCurrentLevel = 1;
		}

		public void ClickBtn(int value)
		{
			switch (value) {
				case 0:
					if (isPlayerTurn) {
						Debug.Log("Click Left Eye");
						mFace.SetEvent(FaceEvent.BLINK_LEFT);
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.SURPRISED_1);
					}
					break;
				case 1:
					if (isPlayerTurn) {
						Debug.Log("Click Right Eye");
						mFace.SetEvent(FaceEvent.BLINK_RIGHT);
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.SURPRISED_2);
					}
					break;
				case 2:
					if (isPlayerTurn) {
						Debug.Log("Click Mouth");
						mFace.SetEvent(FaceEvent.SMILE);
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.SURPRISED_3);
					}
					break;
			}
			//if (isPlayerTurn) {
			//	// play sound
			//	switch (UnityEngine.Random.Range(0, 5)) {
			//		case 0:
			//			BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
			//			break;
			//		case 1:
			//			BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			//			break;
			//		case 2:
			//			BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
			//			break;
			//		default:
			//			Debug.Log("Don't play a sound");
			//			break;
			//	}
			//}
			if (clickFace != null && isPlayerTurn) {
				clickFace(value);
			}
		}

		public delegate void ClickDelegate(int index);

		public void SetClickFace(ClickDelegate delg)
		{
			clickFace = delg;
		}

		public void UnLoadScene()
		{
			Debug.Log("Unloading handler");
			mUnloadingScene = true;
		}
	}
}