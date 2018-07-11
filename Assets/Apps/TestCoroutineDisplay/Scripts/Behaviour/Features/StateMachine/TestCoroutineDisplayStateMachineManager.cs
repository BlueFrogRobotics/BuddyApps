using BlueQuark;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Your script app namespace. You must keep BuddyApp.TestCoroutineDisplay namespace for every script file
/// </summary>
namespace BuddyApp.TestCoroutineDisplay
{
    /// <summary>
    /// If you planned to use a state machine for your application, and want to use provided AStateMachineBehaviour,
    /// you will need to put the StateMachineAppLinker as component of a gameobject in your scene.
    /// You can have as many Linker as you have state machine.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class TestCoroutineDisplayStateMachineManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> gameObjects = new List<GameObject>();

        private Animator mAnimator;

        internal List<GameObject> GameObjects { get { return gameObjects; } }

        void Start()
        {
            mAnimator = GetComponent<Animator>();
            if (mAnimator != null) {
                mAnimator.enabled = true;

                AStateMachineBehaviour[] lStates = mAnimator.GetBehaviours<AStateMachineBehaviour>();

                foreach (AStateMachineBehaviour lState in lStates) {
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