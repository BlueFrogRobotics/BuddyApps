using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using BuddyOS;


namespace BuddyApp.Memory
{
	public class LinkHandler : MonoBehaviour
	{

		public Animator animator;
		public MemoryGameLevels gameLevels;
		public MemoryGameLevel currentLevel;
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

			if (BYOS.Instance.LanguageManager.CurrentLang == Language.FRA) {
				gameLevels = MemoryGameLevels.Load("Lang/levelsFr.json");
			} else {
				gameLevels = MemoryGameLevels.Load("Lang/levelsEn.json");
			}
			mUnloadingScene = false;

			LinkStateMachineBehavior[] b = animator.GetBehaviours<LinkStateMachineBehavior>();

			for (int i = 0; i < b.Length; i++) {
				b[i].link = this;
			}

			Debug.Log("End Link Handler");
		}

		public bool UpdateLevel(int level)
		{
			if (level.Equals(gameLevels.levels.Count)) {
				// no next level to load
				return false;
			}
			Debug.Log("CurrentLevel init");
			currentLevel = gameLevels.levels[level];
			Debug.Log("CurrentLevel ok");
			return true;
		}

		public void ClickBtn(int value)
		{
			switch (value) {
				case 0:
					if (isPlayerTurn) {
						Debug.Log("Click Left Eye");
						mFace.SetEvent(FaceEvent.BLINK_LEFT);
					}
					break;
				case 1:
					if (isPlayerTurn) {
						Debug.Log("Click Right Eye");
						mFace.SetEvent(FaceEvent.BLINK_RIGHT);
					}
					break;
				case 2:
					if (isPlayerTurn) {
						Debug.Log("Click Mouth");
						mFace.SetEvent(FaceEvent.SMILE);
					}
					break;
			}
			if (isPlayerTurn) {
				// play sound
				switch (UnityEngine.Random.Range(0, 5)) {
					case 0:
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
						break;
					case 1:
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
						break;
					case 2:
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
						break;
					default:
						Debug.Log("Don't play a sound");
						break;
				}
			}
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