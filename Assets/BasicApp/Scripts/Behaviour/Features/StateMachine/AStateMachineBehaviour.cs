using Buddy;
using Buddy.UI;

using UnityEngine;

using System.Collections.Generic;

using IEnumerator = System.Collections.IEnumerator;

/// <summary>
/// Your script app namespace. You must keep BuddyApp.YOUR_APP namespace for every script file
/// </summary>
namespace BuddyApp.BasicApp
{
    /// <summary>
    /// If you planned to make a State Machine for your application, you will probably need to make your states inherit from this class.
    /// Add the possibility to use Coroutines, easy access to the Buddy API and retrieve Game Objects and Components on the StateMachineAppLinker.
    /// </summary>
    public abstract class AStateMachineBehaviour : StateMachineBehaviour
    {
        protected const int API_PANEL_IDX = 0;

        protected const string TRIGGER_END_STATE = "EndState";
        protected const string TRIGGER_YES_CLICK = "YesClick";
        protected const string TRIGGER_NO_CLICK = "NoClick";

        /// <summary>
        /// Simple access to the face robot component
        /// </summary>
        protected Face mFace;

        /// <summary>
        /// Simple access to the RGB camera robot component
        /// </summary>
        protected RGBCam mRGBCam;

        /// <summary>
        /// Simple access to the depth camera robot component
        /// </summary>
        protected DepthCam mDepthCam;

        /// <summary>
        /// Simple access to all infrared sensor robot components
        /// </summary>
        protected IRSensors mIRSensors;

        /// <summary>
        /// Simple access to all ultrasound sensor robot components
        /// </summary>
        protected USSensors mUSSensors;

        /// <summary>
        /// Simple access to all cliff sensor robot components
        /// </summary>
        protected CliffSensors mCliffSensors;

        /// <summary>
        /// Simple access to the yes hinge robot component
        /// </summary>
        protected YesHinge mYesHinge;

        /// <summary>
        /// Simple access to the no hinge robot component
        /// </summary>
        protected NoHinge mNoHinge;

        /// <summary>
        /// Simple access to wheels robot component
        /// </summary>
        protected Wheels mWheels;

        /// <summary>
        /// Simple access to the text to speech
        /// </summary>
        protected TextToSpeech mTTS;

        /// <summary>
        /// Simple access to the speech to text
        /// </summary>
        protected SpeechToText mSTT;

        /// <summary>
        /// Simple access to the sphinx trigger
        /// </summary>
        protected SphinxTrigger mSphinx;

        /// <summary>
        /// Simple access to the battery robot component
        /// </summary>
        protected Battery mBattery;

        /// <summary>
        /// Simple access to the micro robot component
        /// </summary>
        protected Micro mMicro;

        /// <summary>
        /// Simple access to the speaker robot component
        /// </summary>
        protected Speaker mSpeaker;

        /// <summary>
        /// Simple access to the LED robot component
        /// </summary>
        protected LED mLED;

        /// <summary>
        /// Simple access to the thermal sensor robot component
        /// </summary>
        protected ThermalSensor mThermalSensor;

        /// <summary>
        /// Simple access to tablet parameters
        /// </summary>
        protected TabletParameters mTabletParameters;

        /// <summary>
        /// Vocal activation wrapper for Sphinx/STT facilities
        /// </summary>
        protected VocalManager mVocalManager;

        /// <summary>
        /// The current language dictionnary
        /// </summary>
        protected Dictionary mDictionary;

        /// <summary>
        /// Notification will be printed at the screen top
        /// </summary>
        protected Notifier mNotifier;

        /// <summary>
        /// Toast will be printed at the screen center
        /// </summary>
        protected Toaster mToaster;

        /// <summary>
        /// Helper in order to easily change the robot mood (LED and Face [and moving behaviour])
        /// </summary>
        protected Mood mMood;

        private Dictionary<string, int> mCommonIntegers;
        private Dictionary<string, float> mCommonSingles;
        private Dictionary<string, string> mCommonStrings;
        private Dictionary<string, object> mCommonObjects;
        private BasicAppStateMachineManager mManager;
        private Animator mAnimator;

        /// <summary>
        /// Common integers through the animator
        /// </summary>
        public Dictionary<string, int> CommonIntegers { get { return mCommonIntegers; } internal set { mCommonIntegers = value; } }

        /// <summary>
        /// Common float through the animator
        /// </summary>
        public Dictionary<string, float> CommonSingles { get { return mCommonSingles; } internal set { mCommonSingles = value; } }

        /// <summary>
        /// Common strings through the animator
        /// </summary>
        public Dictionary<string, string> CommonStrings { get { return mCommonStrings; } internal set { mCommonStrings = value; } }

        /// <summary>
        /// Common objects through the animator
        /// </summary>
        public Dictionary<string, object> CommonObjects { get { return mCommonObjects; } internal set { mCommonObjects = value; } }

        internal Face Face { set { mFace = value; } }
        internal RGBCam RGBCam { set { mRGBCam = value; } }
        internal DepthCam DepthCam { set { mDepthCam = value; } }
        internal IRSensors IRSensors { set { mIRSensors = value; } }
        internal USSensors USSensors { set { mUSSensors = value; } }
        internal CliffSensors CliffSensors { set { mCliffSensors = value; } }
        internal YesHinge YesHinge { set { mYesHinge = value; } }
        internal NoHinge NoHinge { set { mNoHinge = value; } }
        internal Wheels Wheels { set { mWheels = value; } }
        internal TextToSpeech TextToSpeech { set { mTTS = value; } }
        internal SpeechToText SpeechToText { set { mSTT = value; } }
        internal SphinxTrigger SphinxTrigger { set { mSphinx = value; } }
        internal Battery Battery { set { mBattery = value; } }
        internal Micro Micro { set { mMicro = value; } }
        internal Speaker Speaker { set { mSpeaker = value; } }
        internal LED LED { set { mLED = value; } }
        internal ThermalSensor ThermalSensor { set { mThermalSensor = value; } }
        internal TabletParameters TabletParameters { set { mTabletParameters = value; } }
        internal Mood Mood { set { mMood = value; } }

        internal VocalManager VocalManager { set { mVocalManager = value; } }
        internal Dictionary Dictionary { set { mDictionary = value; } }
        internal Notifier Notifier { set { mNotifier = value; } }
        internal Toaster Toaster { set { mToaster = value; } }

        internal BasicAppStateMachineManager Manager { set { mManager = value; } }
        internal Animator Animator { set { mAnimator = value; } }

        /// <summary>
        /// Trigger the named trigger in the current animator
        /// </summary>
        /// <param name="iTriggerName">Trigger name</param>
        protected void Trigger(string iTriggerName)
        {
            mAnimator.SetTrigger(iTriggerName);
        }

        /// <summary>
        /// Reset the named trigger in the current animator
        /// </summary>
        /// <param name="iTriggerName"></param>
        protected void ResetTrigger(string iTriggerName)
        {
            mAnimator.ResetTrigger(iTriggerName);
        }

        /// <summary>
        /// Set the bool value of the named boolean
        /// </summary>
        /// <param name="iBoolName">The bool name to set</param>
        /// <param name="iVal">Value of the boolean</param>
        protected void SetBool(string iBoolName, bool iVal)
        {
            mAnimator.SetBool(iBoolName, iVal);
        }

        /// <summary>
        /// Retrieve the value of the named boolean
        /// </summary>
        /// <param name="iBoolName">Boolean name</param>
        /// <returns>The value of the named boolean</returns>
        protected bool GetBool(string iBoolName)
        {
            return mAnimator.GetBool(iBoolName);
        }

        /// <summary>
        /// Set the bool value of the named integer
        /// </summary>
        /// <param name="iIntName">The integer name to set</param>
        /// <param name="iVal">Value of the integer</param>
        protected void SetInteger(string iIntName, int iVal)
        {
            mAnimator.SetInteger(iIntName, iVal);
        }

        /// <summary>
        /// Retrieve the value of the named integer
        /// </summary>
        /// <param name="iIntName">Integer name</param>
        /// <returns>The value of the named integer</returns>
        protected int GetInteger(string iIntName)
        {
            return mAnimator.GetInteger(iIntName);
        }

        /// <summary>
        /// Set the bool value of the named float
        /// </summary>
        /// <param name="iFloatName">The float name to set</param>
        /// <param name="iVal">Value of the float</param>
        protected void SetFloat(string iFloatName, float iVal)
        {
            mAnimator.SetFloat(iFloatName, iVal);
        }

        /// <summary>
        /// Retrieve the value of the named foat
        /// </summary>
        /// <param name="iFloatName">Foat name</param>
        /// <returns>The value of the named foat</returns>
        protected float GetFloat(string iFloatName)
        {
            return mAnimator.GetFloat(iFloatName);
        }

        /// <summary>
        /// Begin a coroutine.
        /// </summary>
        /// <param name="iFunc">Your coroutine to start</param>
        protected void StartCoroutine(IEnumerator iFunc)
        {
            mManager.StartCoroutineLink(iFunc);
        }

        /// <summary>
        /// Stop the coroutine
        /// </summary>
        /// <param name="iFunc">Your coroutine to stop</param>
        protected void StopCoroutine(IEnumerator iFunc)
        {
            mManager.StopCoroutineLink(iFunc);
        }

        /// <summary>
        /// Stop all started coroutines
        /// </summary>
        protected void StopAllCoroutines()
        {
            mManager.StopAllCoroutinesLink();
        }

        /// <summary>
        /// Cancels all Invoke calls on this MonoBehaviour.
        /// </summary>
        protected void CancelInvoke()
        {
            mManager.CancelInvoke();
        }

        /// <summary>
        /// Cancels all Invoke calls with name methodName on this behaviour.
        /// </summary>
        /// <param name="iMethodName"></param>
        protected void CancelInvoke(string iMethodName)
        {
            mManager.CancelInvoke(iMethodName);
        }

        /// <summary>
        /// Invokes the method methodName in time seconds.
        /// </summary>
        /// <param name="iMethodName"></param>
        /// <param name="iTime"></param>
        protected void Invoke(string iMethodName, float iTime)
        {
            mManager.Invoke(iMethodName, iTime);
        }

        /// <summary>
        /// Invokes the method methodName in time seconds, then repeatedly every repeatRate
        /// </summary>
        /// <param name="iMethodName"></param>
        /// <param name="iTime"></param>
        /// <param name="iRepeatRate"></param>
        protected void InvokeRepeating(string iMethodName, float iTime, float iRepeatRate)
        {
            mManager.InvokeRepeating(iMethodName, iTime, iRepeatRate);
        }

        /// <summary>
        /// Is any invoke pending on this MonoBehaviour?
        /// </summary>
        /// <returns>True if invoking</returns>
        protected bool IsInvoking()
        {
            return mManager.IsInvoking();
        }

        /// <summary>
        /// Retrieve the component (Monobehaviour) on the gameObject where the StateMachineAppLinker belongs.
        /// If the component doens't exist, it adds the component.
        /// </summary>
        /// <typeparam name="T">Type component</typeparam>
        /// <returns>The component</returns>
        protected T GetComponent<T>() where T : Component
        {
            if (mManager.GetComponent<T>() == null)
                mManager.AddComponentLink<T>();
            return mManager.GetComponent<T>();
        }

        /// <summary>
        /// Retrieve the linked gameObject to the StateMachineAppLinker at the iIndex index.
        /// </summary>
        /// <param name="iIndex">The index of the linked gameobject</param>
        /// <returns>The gameobject</returns>
        protected GameObject GetGameObject(int iIndex)
        {
            return mManager.GameObjects[iIndex];
        }

        /// <summary>
        /// Retrieve the linked gameObject to the StateMacheAppLinker by its name.
        /// </summary>
        /// <param name="iName">Name of the gameobject</param>
        /// <returns>The gameobject</returns>
        protected GameObject GetGameObject(string iName)
        {
            List<GameObject> lGOs = mManager.GameObjects;
            int lCount = lGOs.Count;
            for (int i = 0; i < lCount; ++i)
                if (lGOs[i].name == iName)
                    return lGOs[i];

            return null;
        }

        /// <summary>
        /// Retrieve the component in the linked gameObject to the StateMachineAppLinker at the iIndex index.
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <param name="iIndex">The index of the linked gameobject</param>
        /// <returns></returns>
        protected T GetComponentInGameObject<T>(int iIndex) where T : Component
        {
			if (iIndex < 0 || iIndex >= mManager.GameObjects.Count)
				return default(T);
            return mManager.GameObjects[iIndex].GetComponent<T>();
        }

        /// <summary>
        /// Quit the current application and get back to the default app
        /// </summary>
        protected void QuitApp()
        {
            AAppActivity.QuitApp();
        }

        /// <summary>
        /// Method called once at the start of the application.
        /// </summary>
        public virtual void Start() { }
    }
}