using Buddy;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Your script app namespace. You must keep BuddyApp.YOUR_APP namespace for every script file
/// </summary>
namespace BuddyApp.MemoryGame
{
    /// <summary>
    /// If you planned to use a state machine for your application, and want to use provided AStateMachineBehaviour,
    /// you will need to put the StateMachineAppLinker as component of a gameobject in your scene.
    /// You can have as many Linker as you have state machine.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class MemoryGameStateMachineManager : MonoBehaviour
    {
		//public AnimManager mAnimationManager;

		//private ClickDelegate clickFace;
		
		//private Face mFace;


		[SerializeField]
        private List<GameObject> gameObjects = new List<GameObject>();

        internal Animator mAnimator;

        private Dictionary<string, int> mCommonIntegers;
        private Dictionary<string, float> mCommonSingles;
        private Dictionary<string, string> mCommonStrings;
        internal Dictionary<string, object> mCommonObjects;

        internal List<GameObject> GameObjects { get { return gameObjects; } }

        void Start()
        {
			
			mAnimator = GetComponent<Animator>();
            if (mAnimator != null) {
                mAnimator.enabled = true;

                AStateMachineBehaviour[] lStates = mAnimator.GetBehaviours<AStateMachineBehaviour>();
                mCommonIntegers = new Dictionary<string, int>();
                mCommonSingles = new Dictionary<string, float>();
                mCommonStrings = new Dictionary<string, string>();
                mCommonObjects = new Dictionary<string, object>();

				mCommonObjects["gameLevels"] = new MemoryGameRandomLevel(MemoryGameData.Instance.Difficulty, MemoryGameData.Instance.FullBody);
				mCommonIntegers["isPlayerTurn"] = 0;

				foreach (AStateMachineBehaviour lState in lStates) {
                    lState.Init();
                    lState.CommonIntegers = mCommonIntegers;
                    lState.CommonSingles = mCommonSingles;
                    lState.CommonStrings = mCommonStrings;
                    lState.CommonObjects = mCommonObjects;
                    lState.Animator = mAnimator;
                    lState.Manager = this;
                    lState.Start();
                }
            }
            else
                Utils.LogE(LogContext.APP, LogInfo.NULL_VALUE,
                    "Animator of the state machine manager is not set", true);


		}

		internal void AddComponentLink<T>() where T : Component
        {
            gameObject.AddComponent<T>();
        }

        internal void StopAllCoroutinesLink()
        {
            StopAllCoroutines();
        }

        internal void StopCoroutineLink(IEnumerator iFunc)
        {
            StopCoroutine(iFunc);
        }

        internal void StartCoroutineLink(IEnumerator iFunc)
        {
            StartCoroutine(iFunc);
        }
    }
}