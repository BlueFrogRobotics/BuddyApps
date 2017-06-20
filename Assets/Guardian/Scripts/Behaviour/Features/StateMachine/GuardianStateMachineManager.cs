using Buddy;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Your script app namespace. You must keep BuddyApp.YOUR_APP namespace for every script file
/// </summary>
namespace BuddyApp.Guardian
{
    /// <summary>
    /// If you planned to use a state machine for your application, and want to use provided AStateMachineBehaviour,
    /// you will need to put the StateMachineAppLinker as component of a gameobject in your scene.
    /// You can have as many Linker as you have state machine.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class GuardianStateMachineManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> gameObjects = new List<GameObject>();

        private Animator mAnimator;

        private Dictionary<string, int> mCommonIntegers;
        private Dictionary<string, float> mCommonSingles;
        private Dictionary<string, string> mCommonStrings;
        private Dictionary<string, object> mCommonObjects;

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

                foreach (AStateMachineBehaviour lState in lStates) {
                    lState.Face = BYOS.Instance.Face;
                    lState.Battery = BYOS.Instance.Battery;
                    lState.RGBCam = BYOS.Instance.RGBCam;
                    lState.DepthCam = BYOS.Instance.DepthCam;
                    lState.IRSensors = BYOS.Instance.IRSensors;
                    lState.USSensors = BYOS.Instance.USSensors;
                    lState.CliffSensors = BYOS.Instance.CliffSensors;
                    lState.YesHinge = BYOS.Instance.Motors.YesHinge;
                    lState.NoHinge = BYOS.Instance.Motors.NoHinge;
                    lState.Wheels = BYOS.Instance.Motors.Wheels;
                    lState.TextToSpeech = BYOS.Instance.TextToSpeech;
                    lState.SpeechToText = BYOS.Instance.SpeechToText;
                    lState.SphinxTrigger = BYOS.Instance.SphinxTrigger;
                    lState.Micro = BYOS.Instance.Micro;
                    lState.Speaker = BYOS.Instance.Speaker;
                    lState.LED = BYOS.Instance.LED;
                    lState.ThermalSensor = BYOS.Instance.ThermalSensor;
                    lState.TabletParameters = BYOS.Instance.TabletParameters;
                    lState.VocalActivation = BYOS.Instance.VocalManager;
                    lState.Dictionary = BYOS.Instance.Dictionary;
                    lState.Notifier = BYOS.Instance.Notifier;
                    lState.Toaster = BYOS.Instance.Toaster;
                    lState.Mood = BYOS.Instance.Mood;
                    lState.DetectionManager = GetComponent<DetectionManager>();
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